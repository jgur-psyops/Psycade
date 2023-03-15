using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Changelog: 3/8: Initial commit
 */
public class EnemyTurretController : MonoBehaviour{
    [Tooltip("The part that actually spins to face target, should be a child")]
    public GameObject rotator;
    [Tooltip("Location where shots come out of")]
    public Transform bulletSpawn;
    [Tooltip("Bullet created by turret (bullet mode only)")]
    public GameObject bulletPrefab;

    private GameObject target;
    private Transform targetCenterOfMass;
    private EnemySettings enemySettings;

    void Start() {
        enemySettings = EnemySettings.getInstance();
        StartCoroutine("fireWeapon");
    }


    void Update() {
        if (target != null) {
            Vector3 lookDirection = targetCenterOfMass.position - bulletSpawn.position;
            Quaternion toTarget = Quaternion.LookRotation(lookDirection);
            rotator.transform.rotation = Quaternion.Slerp(rotator.transform.rotation, toTarget, enemySettings.TurretRotationSpeed * Time.deltaTime);
        }
    }

    //Rangefinder should be on a layer that only collides with the player
    private void OnTriggerStay(Collider other) {
        //Has a target already, rangefinder doesn't need to do anything
        if (target != null) {
            return;
        }
        if (other.transform.root.CompareTag("Player")) {
            target = other.transform.root.gameObject;
            targetCenterOfMass = other.transform.root.Find("CenterOfMass");
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.transform.root.CompareTag("Player")) {
            target = null;
        }
    }

    IEnumerator fireWeapon() {
        while (true) {
            yield return new WaitForSeconds(enemySettings.TurretAttackDelay);
            if (target != null) {
                float accuracy = enemySettings.TurretShotErrorRate;
                // Get shot imprecision vector.
                Vector3 imprecision = Random.Range(-accuracy, accuracy) * bulletSpawn.right;
                imprecision += Random.Range(-accuracy, accuracy) * bulletSpawn.up;

                // Get shot desired direction. Adjust up slightly to not shoot at the character's feet
                Vector3 shotDirection = targetCenterOfMass.position - bulletSpawn.position;

                GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody>().AddForce((shotDirection.normalized + imprecision) * enemySettings.TurretBulletForce);
            }
        }
    }

}
