using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Changelog
 * 3/8 - Now searches for the health module on the root
 */

public class EnemyExplodeInRange : MonoBehaviour{

    [Tooltip("Once in range, wait this long to explode")]
    private float delay;
    [Tooltip("Range where explosion will deal damage")]
    private float damageRadius;
    [Tooltip("Damage dealt if within the explosion area")]
    private int damage;
   
    private EnemySettings enemySettings;
    public EnemySettings.EnemyType type;

    [Tooltip("Contains the particle system that animates while the delay is active")]
    public GameObject delayAnimation;
    [Tooltip("Animation, etc built when the object explodes")]
    public GameObject explosion;
    [Tooltip("If true, explosion hurts player")]
    public bool damagePlayer;
    [Tooltip("If true, explosion hurts enemies")]
    public bool damageEnemies;

    private bool isExploding = false;

    private HealthManager health;


    void Start(){
        enemySettings = EnemySettings.getInstance();
        if (type == EnemySettings.EnemyType.HunterKiller) {
            delay = enemySettings.HunterKilledExplosionDelay;
            damage = enemySettings.HunterKilledExplosionDamage;
            damageRadius = enemySettings.HunterKillerExplosionRadius;
        }
        health = transform.root.GetComponent<HealthManager>();
        
    }

    void Update(){
        if (health.dead) {
            explode();
        }
        
    }

    public void explode() {
        if (!isExploding) {
            isExploding = true;
            StartCoroutine("Kaboom");
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag("Player")) {
            explode();
        }
    }

    private void OnDrawGizmos() {
        if (isExploding) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, damageRadius);
        }
    }

    IEnumerator Kaboom() {
        if (delayAnimation != null) {
            delayAnimation.SetActive(true);
        }

        //Navigation unit is always attached to the top-level object for simplicity
        NavtoPlayer nav = transform.root.GetComponent<NavtoPlayer>();
        if (nav != null) {
            nav.stopNavigating();
        }

        yield return new WaitForSeconds(delay);

        GameObject kaboom = Instantiate(explosion, transform.position, Quaternion.identity);
        int mask = 0;
        if (damagePlayer && damageEnemies) {
            mask = LayerMask.GetMask("Player", "Enemy");
        } else if(damagePlayer){
            mask = LayerMask.GetMask("Player");
        } else if(damageEnemies){
            mask = LayerMask.GetMask("Enemy");
        } else {
            Debug.Log("Explosion spawned that damages neither players nor enemies");
            yield return null;
        }

        Collider[] inRange = Physics.OverlapSphere(transform.position, damageRadius, mask);
        for(int i=0; i<inRange.Length; i++) {
            //ignore self
            if (inRange[i].transform.root == this.transform.root) {
                continue;
            }

            //Health unit is always attached to the top-level object for simplicity
            HealthManager health = inRange[i].transform.root.gameObject.GetComponent<HealthManager>();
            if (health != null) {
                Vector3 direction = (transform.position - inRange[i].transform.position).normalized;
                Vector3 hitPosition = inRange[i].ClosestPoint(transform.position);
                health.TakeDamage(hitPosition, direction, damage);
            }
            Rigidbody rb = inRange[i].GetComponent<Rigidbody>();
            if (rb != null) {
                //Force applied is proportional to how far away the object is from the center of the blast
                rb.AddExplosionForce(enemySettings.HunterKilledExplosionForce, transform.position, damageRadius, .05f);
            }
        }

        Destroy(transform.root.gameObject);

        yield return null;
    }
}
