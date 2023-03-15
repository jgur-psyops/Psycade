using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //note the extra import to do AI!

/*
 * Changelog
 * 3/28 - Now suicides when out of fuel instead of just vanishing.
 * 3/28 - Added animation functionality, script is now very specific to Hunter-Killers.
 * 2/23 - stop and resume Navigating functions, deactivating the navmesh and allowing this unit to ragdoll
 * 
 * TODO if split from the navmesh for whatever reason, suicide.
 */
public class NavtoPlayer : MonoBehaviour{

    public GameObject player;
    public Animator animator;
    private NavMeshAgent agent;
    //False until the spider finishes deploying.
    private bool isNavigating = false;

    void Start(){
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player").transform.root.gameObject;
        }
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine("activateMovement");
        StartCoroutine("outOfFuel");

    }

    void Update(){
        if (!agent.isActiveAndEnabled || player == null || !isNavigating) {
            return;
        }
        agent.SetDestination(player.transform.position);

        if (animator.GetBool("Halted")) {
            agent.speed = 0;
            return;
        }

        if (isFacingTarget()) {
            animator.SetBool("IsReadyToWalk", true);
            animator.SetBool("IsFacingTarget", true);
            agent.speed = EnemySettings.getInstance().HunterKillerRunMoveSpeed;
            isNavigating = true;
        } else {
            animator.SetBool("IsFacingTarget", false);
            agent.speed = EnemySettings.getInstance().HunterKillerTurnMoveSpeed;
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

    IEnumerator outOfFuel() {
        yield return new WaitForSeconds(EnemySettings.getInstance().HunterKillerLifespan);
        stopNavigating();
        EnemyExplodeInRange exploder = GetComponentInChildren<EnemyExplodeInRange>();
        exploder.explode();

    }

    IEnumerator activateMovement() {
        yield return new WaitForSeconds(EnemySettings.getInstance().HunterKillerStartMoveDelay);
        if (animator != null && isFacingTarget()) {
            animator.SetBool("IsReadyToWalk", true);
            isNavigating = true;
        } else {
            animator.SetBool("IsReadyToTurn", true);
            isNavigating = true;
        }
    }
}
