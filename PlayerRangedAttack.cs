using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRangedAttack : MonoBehaviour{

    [Tooltip("Bullet created by player")]
    public GameObject bullet;
    [Tooltip("Location where shots come out of")]
    public Transform bulletSpawn;

    private PlayerSettings playerSettings;
    private float nextShotAllowed = 0;
 
    void Start(){
        playerSettings = PlayerSettings.getInstance();
        
    }

    void Update(){
        
    }

    public void shoot() {
        if(Time.time > nextShotAllowed) {
            GameObject shot = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
            Vector3 direction = bulletSpawn.transform.forward.normalized;
            Rigidbody rb = shot.GetComponent<Rigidbody>();
            rb.AddForce(direction * playerSettings.RangeAttackForce);
            nextShotAllowed = Time.time + playerSettings.RangeAttackCooldown;
        }
        
    }
}
