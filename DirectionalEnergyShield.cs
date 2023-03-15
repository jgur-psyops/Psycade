using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Changelog
 * 3/08 - Fixed collapse/regen state management issues, added GUI
 * 3/07 - Shield collapse and regeneration states added, now also takes damage
 */
public class DirectionalEnergyShield : MonoBehaviour{

    public GameObject activeAnimation;

    public bool shieldActive = false;
    public int health;
    public new Collider collider;
    private Light shieldGlow;
    private float maxLightIntensity = 2.5f;
    private float effectiveness = 0f;
    /// <summary>
    /// Shield regeneration ticks until the collapse animation ends
    /// </summary>
    const int TICKS_TO_COLLAPSE = 10;
    private bool isCollapsed = false;
    private int tick = 0;

    private PlayerSettings playerSettings;

    public Image shieldBar;

    private enum ShieldState {
        Raising,
        Active,
        Collapsing,
        Regenerating,
        Off
    }
    private ShieldState state = ShieldState.Off;


    void Start(){
        playerSettings = PlayerSettings.getInstance();
        shieldGlow = GetComponentInChildren<Light>();

        if (shieldActive) {
            activeAnimation.SetActive(true);
            collider.enabled = true;
        } else {
            activeAnimation.SetActive(false);
            collider.enabled = false;
        }
        health = playerSettings.shieldMaxHealth;
        StartCoroutine("handleChargeLevel");
    }


    void Update(){

    }

    private void shieldDamageAnimation(Vector3 position) {
        //TODO?
    }

    public void damageShield(int damage) {
        int damageAfterMitigation = (int)((float) damage * (1f - effectiveness));
        if ( health > 0) {
            health -= damageAfterMitigation;
        } else {
            health = 0;
            state = ShieldState.Collapsing;
            collider.enabled = false;
            tick = TICKS_TO_COLLAPSE;
        }
    }

    public void activateShield() {
        shieldActive = true;
        activeAnimation.SetActive(true);
        if (!isCollapsed) {
            collider.enabled = true;
            shieldGlow.color = Color.cyan;
            state = ShieldState.Raising;
        } else {
            collider.enabled = false;
            state = ShieldState.Regenerating;
        }
    }

    public void deactivateShield() {
        shieldActive = false;
        activeAnimation.SetActive(false);
        collider.enabled = false;
        state = ShieldState.Off;
    }

    private void collapseShield() {

    }

    /// <summary>
    /// Increases or decreases shield effectiveness depending on shield state, handles changes to light and base shield animation
    /// </summary>
    /// <returns></returns>
    IEnumerator handleChargeLevel() {
        while (true) {
            yield return new WaitForSeconds(playerSettings.shieldTickRate);
            switch (state) {
                case ShieldState.Raising:
                    if(effectiveness + playerSettings.shieldEffectivenessGain < playerSettings.maxShieldEffectiveness) {
                        effectiveness += playerSettings.shieldEffectivenessGain;
                        shieldGlow.intensity = maxLightIntensity * effectiveness / playerSettings.maxShieldEffectiveness;
                    } else {
                        effectiveness = playerSettings.maxShieldEffectiveness;
                        //shield "sparks" with super bright light until next tick
                        shieldGlow.intensity = maxLightIntensity * 2;
                        shieldGlow.color = Color.green;
                        state = ShieldState.Active;
                    }
                    break;
                case ShieldState.Active:
                    //stay at full strength
                    shieldGlow.intensity = maxLightIntensity;
                    break;
                case ShieldState.Off:
                    if (effectiveness - playerSettings.shieldEffectivenessLoss > playerSettings.minShieldEffectiveness) {
                        effectiveness -= playerSettings.shieldEffectivenessLoss;
                        shieldGlow.intensity = maxLightIntensity * effectiveness / playerSettings.maxShieldEffectiveness;
                        shieldGlow.color = Color.cyan;
                    } else {
                        effectiveness = playerSettings.minShieldEffectiveness;
                        shieldGlow.intensity = 0;
                    }
                    break;
                case ShieldState.Collapsing:
                    shieldGlow.color = Color.red;
                    shieldGlow.intensity = Random.Range(0f, maxLightIntensity);
                    effectiveness = playerSettings.minShieldEffectiveness;
                    if (tick > 0) {
                        tick--;
                    } else {
                        state = ShieldState.Regenerating;
                        deactivateShield();
                    }
                    isCollapsed = true;
                    break;
                case ShieldState.Regenerating:
                    shieldGlow.color = Color.yellow;
                    shieldGlow.intensity = maxLightIntensity * (float) health / (float) playerSettings.shieldMaxHealth;
                    isCollapsed = true;
                    if (health == playerSettings.shieldMaxHealth) {
                        state = ShieldState.Raising;
                        collider.enabled = true;
                    }
                    break;
                default:
                    break;
            }
            //health always regnerates...
            if (health < playerSettings.shieldMaxHealth) {
                health += playerSettings.shieldHealthRegen;
            } else {
                health = playerSettings.shieldMaxHealth;
            }
            //Shield recovers from collapse when hitting max health, regardless of state
            if (health == playerSettings.shieldMaxHealth) {
                isCollapsed = false;
            }
            shieldBar.fillAmount = (float) health / (float) playerSettings.shieldMaxHealth;
        }
    }
}
