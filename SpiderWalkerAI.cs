using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //note the extra import to do AI!

/*
 * Changelog
 * 3/14/23 - Split from NavToPlayer, now handles AI for the big spiders
 * 3/28 - Now suicides when out of fuel instead of just vanishing.
 * 3/28 - Added animation functionality, script is now very specific to Hunter-Killers.
 * 2/23 - stop and resume Navigating functions, deactivating the navmesh and allowing this unit to ragdoll
 * 
 * TODO if split from the navmesh for whatever reason, suicide.
 */
public class SpiderWalkerAI: MonoBehaviour{

    [Tooltip("Best if this is the player's center of mass")]
    public GameObject player;
    public Animator animator;
    [Tooltip("The rotator for the turret")]
    public GameObject turret;
    [Tooltip("The turret's shot spawn location")]
    public Transform bulletSpawn;
    private NavMeshAgent agent;
    //False until the spider finishes deploying.
    private bool isNavigating = false;

    private EnemySettings enemySettings;

    void Start(){
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player").transform.root.Find("CenterOfMass").gameObject;
        }
        agent = GetComponent<NavMeshAgent>();
        enemySettings = EnemySettings.getInstance();
        StartCoroutine("activateMovement");
    }

    void Update(){
        if (!agent.isActiveAndEnabled || player == null) {
            return;
        }

        agent.SetDestination(player.transform.position);

        if (animator.GetBool("Halted") || !isNavigating) {
            agent.speed = 0;
        } else {
            if (isFacingTarget()) {
                agent.speed = enemySettings.SpiderRunMoveSpeed;
                animator.SetBool("IsFacingTarget", true);
                animator.SetBool("IsReadyToWalk", true);
            } else {
                agent.speed = enemySettings.SpiderTurnMoveSpeed;
                animator.SetBool("IsFacingTarget", false);
            }
        }

        // Turret rotates towards target and spider halts when in range.
        if(Vector3.Distance(transform.position, player.transform.position) < enemySettings.SpiderEngagementDistance) {
            animator.SetBool("Halted", true);
            Vector3 lookDirection = player.transform.position - bulletSpawn.position;
            Quaternion toTarget = Quaternion.LookRotation(lookDirection);
            turret.transform.rotation = Quaternion.RotateTowards(turret.transform.rotation, toTarget, enemySettings.SpiderTurretRotationSpeed * 100f * Time.deltaTime);
        } else {
            // If not in range, rotate turret to forwards
            Quaternion toTarget = Quaternion.LookRotation(transform.forward);
            turret.transform.rotation = Quaternion.RotateTowards(turret.transform.rotation, toTarget, enemySettings.SpiderTurretRotationSpeed * 100f * Time.deltaTime);
            animator.SetBool("Halted", false);
        }

        if(agent.isActiveAndEnabled && !agent.isOnNavMesh) {
            Debug.Log("off mesh!");
        }
    }

    /// <summary>
    /// True if the unit is roughly facing the player target
    /// </summary>
    /// <returns></returns>
    private bool isFacingTarget() {
        if(player == null) {
            return false;
        }
        float diff = Vector3.Angle(transform.forward, player.transform.position - transform.position);
        if (diff < 10f) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Disable the navmesh, make the unit non-kinematic
    /// </summary>
    public void stopNavigating() {
        isNavigating = false;
        agent.enabled = false;
        animator.SetBool("Halted", true);
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = false;
        }
    }

    /// <summary>
    /// Enable the navmesh, make the unit kinematic
    /// </summary>
    public void resumeNavigating() {
        isNavigating = true;
        agent.enabled = true;
        animator.SetBool("Halted", false);
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = true;
        }
    }

    IEnumerator activateMovement() {
        yield return new WaitForSeconds(enemySettings.SpiderStartMoveDelay);
        if (isFacingTarget()) {
            animator.SetBool("IsFacingTarget", true);
            animator.SetBool("IsReadyToWalk", true);
            isNavigating = true;
        } else {
            animator.SetBool("IsReadyToTurn", true);
            isNavigating = true;
        }
    }
}
