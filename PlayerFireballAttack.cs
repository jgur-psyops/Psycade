using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFireballAttack : MonoBehaviour{

    public SolanaHandler solanaHandler;
    private PlayerSettings playerSettings;
    public GenericSkillCooldown cooldownManager;
    public GameObject fireballPrefab;
    public Transform bulletSpawn;

    private bool canAttack = true;
    private float nextAttackPossible = 0f;
    private int charges = 0;

    void Start(){
        if (solanaHandler == null) {
            solanaHandler =  GameObject.Find("SolanaHandler").GetComponent<SolanaHandler>();
        }
        playerSettings = PlayerSettings.getInstance();
        charges = 0;
        cooldownManager.updateSkillCooldown(Time.time, 0, charges, playerSettings.MaxFireballCharges);
    }

    void Update(){
        if (Time.time > nextAttackPossible) {
            canAttack = true;
            if(charges < playerSettings.MaxFireballCharges) {
                charges += 1;
                nextAttackPossible = Time.time + playerSettings.FireballCooldown;
                cooldownManager.updateSkillCooldown(Time.time, nextAttackPossible, charges, playerSettings.MaxFireballCharges);
            }
        }
    }

    public void fire() {
        if (solanaHandler.usdc < 1) {
            Debug.Log("no usdc.");
            return;
            // do nothing
        } 

        if (!canAttack) {
            return;
        }

        // fire!
        Vector3 shotDirection = bulletSpawn.transform.forward.normalized;
        GameObject bullet = Instantiate(fireballPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(shotDirection * playerSettings.FireballAttackForce);

        charges -= 1;
        nextAttackPossible = Time.time + playerSettings.FireballCooldown;

        if (charges == 0) {
            canAttack = false;
        }

        cooldownManager.updateSkillCooldown(Time.time, nextAttackPossible, charges, playerSettings.MaxFireballCharges);
    }
}
