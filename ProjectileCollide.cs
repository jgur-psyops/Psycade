using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollide : MonoBehaviour{

    [Tooltip("Will only attempt to do damage if colliding with something with a matching tag (Excludes shield!)")]
    public string[] tagsToCollideWith;
    [Tooltip("A child object that activates on this object's death and does something, typically plays some animation.")]
    public GameObject activateOnDeath;
    public bool isEnemy;
    public bool isFireball;
    private int damage;

    private Rigidbody rb;

    void Start(){
        rb = GetComponent<Rigidbody>();
        if (isEnemy) {
            damage = EnemySettings.getInstance().SmallBulletDamage;
        } else {
            if (isFireball) {
                damage = PlayerSettings.getInstance().FireballMainDamage;
            } else {
                damage = PlayerSettings.getInstance().RangedAttackDamage;
            }
        }
    }

    void Update(){
        
    }

    private void OnCollisionEnter(Collision collision) {
        //Debug.Log("hit base " + collision.gameObject.name);
        //Only collision with a given tag triggers a "real" collision event
        if (matchesSomeTag(collision.transform)){
           //Debug.Log("hit " + collision.gameObject.name);
            HealthManager health = collision.transform.root.GetComponent<HealthManager>();
            if (health != null) {
                Vector3 direction = rb.velocity.normalized;
                Vector3 hitPosition = collision.contacts[0].point;
                health.TakeDamage(hitPosition, direction, damage);
            }
        }
        if (collision.transform.CompareTag("PlayerShield")) {
      //      Debug.Log("hit shield " + collision.gameObject.name);
            DirectionalEnergyShield shield = collision.transform.GetComponent<DirectionalEnergyShield>();
            shield.damageShield(damage);
        }

        if (isFireball) {
            int layerMask = LayerMask.GetMask("Enemy");
            Collider[] colliders = Physics.OverlapSphere(collision.contacts[0].point, 8f, layerMask);
            foreach (Collider c in colliders) {
                HealthManager h = c.transform.root.GetComponent<HealthManager>();
                if (h != null) {
                    Vector3 direction = rb.velocity.normalized;
                    Vector3 hitPosition = collision.contacts[0].point;
                    h.TakeDamage(hitPosition, direction, PlayerSettings.getInstance().FireballAoeDamage);
                }
            }
        }
        Destroy(gameObject);
    }

    private void OnDestroy() {
        //Collision with anything makes the projectile die and spawn a death object
        activateOnDeath.SetActive(true);
        Destroy(activateOnDeath, 5);
        activateOnDeath.transform.SetParent(null);
    }

    private bool matchesSomeTag(Transform t) {
        foreach(string s in tagsToCollideWith) {
            if (t.CompareTag(s)) {
                return true;
            }
        }
        return false;
    }
}
