using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyAI;
/*
 * Changelog
 * 2/23 - Adds force to the attack, ragdolling dead enemies. Added shield to list of things to ignore for LoS check
 * 2/21 - For line of sight comparisons, now compares ROOT transform, so any child of the root can be in LoS to pass the check
 * 
 * TODO check if enemyAI requires a NOTIFY when dealing damage
 */

public class PlayerMeleeAttack : MonoBehaviour {

    private PlayerSettings playerSettings;
    [Tooltip("The box that extends to create the player's attack will use this point as its center. Note that forward position will automatically be" +
        " additionally offset by half the attack area's length")]
    public Transform attackOrigin;
    [Tooltip("Checks for sight obstructions originate at this point")]
    public Transform lineOfSightOrigin;
    [Tooltip("The particle emitter that plays when an attack is issued")]
    public ParticleSystem slashAnimator;

    [Tooltip("Cube to draw the attack area when debug mode is enabled")]
    public GameObject debugAreaCube;
    [Tooltip("Prefab to draw hit indicators when debug mode is enabled")]
    public GameObject debugHitCube;

    private bool enableDebugMode = true;
    private bool canAttack = true;
    /// <summary>
    /// The time (relative to Time.time) when we can attack again. A simpler alternative to cooldowns
    /// when you all you want is a simple periodic event with no associated UI (a cooldown wheel, etc)
    /// and don't mind paying a small performance hit (very small)
    /// </summary>
    private float nextAttackPossible = 0f;

    void Start() {
        playerSettings = PlayerSettings.getInstance();
        updateSlashAnimatorSize();
    }

    void Update() {
        //Position the debug cube where the attack zone will be
        if (enableDebugMode) {
            debugAreaCube.transform.position = attackOrigin.position + transform.forward * playerSettings.MeleeAttackLength / 2;
            //Exactly double the size used for the physics event. The debug cube must be a root object (no parent)
            debugAreaCube.transform.localScale = new Vector3(playerSettings.MeleeAttackWidth, 5f, playerSettings.MeleeAttackLength);
            debugAreaCube.transform.rotation = attackOrigin.transform.rotation;
        }
        if (Time.time > nextAttackPossible) {
            canAttack = true;
        }
    }

    /// <summary>
    /// Call this whenever the size of the player's attack field changes to update the corresponding animation
    /// </summary>
    public void updateSlashAnimatorSize() {
        slashAnimator.transform.localScale = new Vector3(playerSettings.MeleeAttackWidth, 5f, playerSettings.MeleeAttackLength) / 1.5f;
    }

    /// <summary>
    /// Executes a melee attack in front of the character
    /// </summary>
    public void Attack() {
        if (!canAttack) {
            return;
        }
        canAttack = false;
        nextAttackPossible = Time.time + playerSettings.MeleeAttackCooldown;

        int layerMask = LayerMask.GetMask("Enemy");
        Collider[] hits = Physics.OverlapBox(
            attackOrigin.position + transform.forward * playerSettings.MeleeAttackLength / 2,
            //Note that some smoothbrain decided that overlapBox would take a HALF size as the second arg!
            new Vector3(playerSettings.MeleeAttackWidth, 5f, playerSettings.MeleeAttackLength) / 2,
            attackOrigin.transform.rotation,
            layerMask
            );
        //The particle animator has to move to accomodate the actual position of the physics cast
        slashAnimator.transform.position = attackOrigin.position + transform.forward * playerSettings.MeleeAttackLength / 2;
        slashAnimator.Emit(2);
        for (int i = 0; i < hits.Length; i++) {
            //Anything that gets hit should have force applied to it, even dead things (for ragdoll)
            Rigidbody rb = hits[i].GetComponent<Rigidbody>();
            if (rb != null) {
                //Force applied is proportional to how far away the object is from the player
                //(Simulating explosive force applied at the player's location)
                rb.AddExplosionForce(playerSettings.MeleeAttackForce, transform.position, playerSettings.MeleeAttackLength, .15f);
            }
            //No line of sight found, this target is obstructed and should be ignored
            if (!checkLoSWithCleave(hits[i])) {
                continue;
            }

            if (enableDebugMode) {
                GameObject hitCube = Instantiate(debugHitCube, hits[i].transform.position + new Vector3(0f, 3f, 0f), Quaternion.identity);
                hitCube.transform.parent = hits[i].transform; //allows markers to follow the thing that was hit
            }
            //Health component should always be attached to the root for simplicity
            EnemyHealth health = hits[i].transform.root.GetComponent<EnemyHealth>();
            if (health != null) {
                //    Debug.Log("damaging: " + hits[i].transform.root.gameObject.name);
                Vector3 direction = (transform.position - hits[i].transform.position).normalized;
                health.TakeDamage(hits[i].transform.position, direction, playerSettings.MeleeAttackDamage);
            }
        }
    }

    /// <summary>
    /// Checks if a line of sight is available to the target from the attached lineOfSightOrigin
    /// </summary>
    /// <param name="toCheck"></param>
    /// <returns>true if LoS exists, or cleave is enabled and only an enemy obstructs it, otherwise false</returns>
    private bool checkLoSWithCleave(Collider toCheck) {
        if (playerSettings.PlayerMeleeIgnoresWalls) {
            return true;
        }

        //Check if a line of sight exists to the target, preventing the player from hitting through walls
        int notPlayer = ~LayerMask.GetMask("Player", "PlayerRotationTarget", "PlayerShield");
        RaycastHit hit;
        Vector3 direction = (toCheck.transform.position - lineOfSightOrigin.position).normalized;
        if (Physics.Raycast(lineOfSightOrigin.position, direction, out hit, playerSettings.MeleeAttackLength * 2, notPlayer)) {
          //   Debug.Log("hit ray " + hit.transform.gameObject.name + " expected " + toCheck.transform.gameObject.name);
            if (enableDebugMode) {
                Debug.DrawLine(lineOfSightOrigin.position, hit.point, Color.green, 2f);
            }
            //hit something other than the target..
            if (hit.transform.root != toCheck.transform.root) {
                //if it was an enemy, and cleave is enabled, we can cleave through it
                if (playerSettings.PlayerMeleeCleavesThroughEnemies && hit.transform.CompareTag("Enemy")) {
                    //fire a new ray...
                    RaycastHit secondaryHit;
                    Vector3 redirection = (toCheck.transform.position - hit.transform.position).normalized;
                    if (Physics.Raycast(hit.transform.position, redirection, out secondaryHit, playerSettings.MeleeAttackLength, notPlayer)) {
                        if (enableDebugMode) {
                            Debug.DrawLine(hit.transform.position, secondaryHit.point, Color.blue, 2f);
                        }
                        //hit something other than the target (again)...
                        if (secondaryHit.transform.root != toCheck.transform.root) {
                            return false;
                            //This could be repeated as many times as you like for multiple cleaves....
                        }
                    }
                } else {
                    //anything other than an enemy, we hit an obstruction, so this target is obstructed
                    return false;
                }
            }

        } else {
            //ray missed completely (sanity check, should be impossible)
            return false;
        }
        
        return true;
    }
}
