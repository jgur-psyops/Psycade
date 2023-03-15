using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Changelog
 * TODO display to indicate when character is in "locked" state and can't move/shoot
 */
public class PlayerAimedShot : MonoBehaviour {

    [Tooltip("Animation created when the shot fires")]
    public GameObject onShot;
    [Tooltip("Gameobject rendered when the aimedshot is active")]
    public GameObject aimIndicator;
    [Tooltip("Line of sight origin")]
    public Transform lineOfSightOrigin;
    [Tooltip("True if the player is attempting an aimed shot right now")]
    public bool aimedShotActive = false;
    [Tooltip("If used, shows the range of the aimed shot when engaged")]
    public GameObject debugSphere;
   
    [Tooltip("Closest target to the screen point being aimed")]
    private GameObject target;
    private PlayerCharacterController playerCharacterController;
    private int layerMask;
    private int aimTick = 0;
    private int disengageTick = 0;

    private enum ShotState {
        Aiming,
        Firing,
        Disengaging,
        Inactive
    }
    private ShotState state = ShotState.Inactive;

    private PlayerSettings playerSettings;

    void Start() {
        playerSettings = PlayerSettings.getInstance();
        playerCharacterController = GetComponent<PlayerCharacterController>();
        layerMask = LayerMask.GetMask("Enemy");
        aimTick = playerSettings.TicksToLockOn;
        disengageTick = playerSettings.TicksToDisengage;
        aimIndicator.SetActive(false);
        StartCoroutine("handleAimedShot");
    }


    void Update() {
       // Debug.Log("look indicator: " + playerCharacterController.lookTargetPoint.transform.position);
    }

    public void initiateShot() {
        if(state == ShotState.Inactive) {
            state = ShotState.Aiming;
            aimedShotActive = true;
        }
        //Debug.Log("Shot initiate!");
    }

    public void disengageShot() {
        if(state != ShotState.Inactive) {
            state = ShotState.Disengaging;
        }
        //Debug.Log("Shot disengage!");
    }

    public GameObject findNearestViableTarget() {
        Vector3 targetPoint = playerCharacterController.lookTargetPoint.transform.position;
        Collider[] hits = Physics.OverlapSphere(
          targetPoint,
          playerSettings.AimedShotDetectionRange,
          layerMask);
        if (debugSphere != null) {
            debugSphere.transform.position = targetPoint;
       //     Debug.Log("look indicator: " + playerCharacterController.lookTargetPoint.transform.position);
            debugSphere.transform.localScale = new Vector3(playerSettings.AimedShotDetectionRange*2, playerSettings.AimedShotDetectionRange*2, playerSettings.AimedShotDetectionRange*2);
        }
        if (hits.Length < 1) {
            return null;
        }

        float closestDistance = Mathf.Infinity;
        GameObject closest = null;
        foreach (Collider hit in hits) {
            HealthManager health = hit.transform.root.gameObject.GetComponent<HealthManager>();
            if(health==null || health.dead) {
                continue;
            }
            //TODO check line of sight
            float distance = Vector3.Distance(targetPoint, hit.transform.position);
            if (distance < closestDistance) {
                closestDistance = distance;
                closest = hit.transform.root.gameObject;
            }
        }
        return closest;
    }

    IEnumerator handleAimedShot() {
        while (true) {
            yield return new WaitForSeconds(playerSettings.aimedShotTickRate);
          //  Debug.Log("State: " + state);
            if (aimedShotActive) {
                switch (state) {
                    case ShotState.Aiming:
                        //TODO play disengage animation, inform animator to begin aiming
                        if (target == null) {
                            GameObject newTarget = findNearestViableTarget();
                            target = newTarget;
                            aimTick = playerSettings.TicksToLockOn;
                        } else {
                            //TODO pick a new target if the lock target dies
                            aimedShotActive = true;
                            aimIndicator.SetActive(true);
                            aimIndicator.transform.position = new Vector3(target.transform.position.x, -0.5f, target.transform.position.z);
                            if (aimTick > 0) {
                                //TODO check line of sight is intact
                                aimTick--;
                            } else {
                                aimTick = playerSettings.TicksToLockOn;
                                state = ShotState.Firing;
                            }
                        }
                        break;
                    case ShotState.Firing:
                        GameObject shot = Instantiate(onShot, transform.position, Quaternion.identity);
                        LineRenderer line = shot.GetComponent<LineRenderer>();
                        line.SetPositions(new Vector3[] {transform.position, target.transform.position});
                        HealthManager health = target.GetComponent<HealthManager>();
                        Vector3 direction = (transform.position - target.transform.position).normalized;
                        if (health != null) {
                            health.TakeDamage(target.transform.position, direction, playerSettings.AimedShotDamage);
                        }
                        state = ShotState.Disengaging;
                        break;
                    case ShotState.Disengaging:
                        //TODO play disengage animation, inform animator to stand
                        aimIndicator.SetActive(false);
                        target = null;
                        if (disengageTick > 0) {
                            disengageTick--;
                        } else {
                            disengageTick = playerSettings.TicksToDisengage;
                            state = ShotState.Inactive;
                        }
                        break;
                    case ShotState.Inactive:
                        aimIndicator.SetActive(false);
                        aimedShotActive = false;
                        //do nothing
                        break;
                    default:
                        break;
                }

            }
        }
    }
}
