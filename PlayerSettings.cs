using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A singleton that tracks various properties of the player, such as attack damage
/// </summary>
public class PlayerSettings {

    /*
     * GENERAL PLAYER STATS
     */
    [Tooltip("Max health the player can obtain via health regeneration")]
    public int maxPlayerHealth = 1000;
    [Tooltip("Amount of health regenerated per second")]
    public int healthRegenPerTick = 20;

    /*
     * MELEE ATTACK SETTINGS
     */
    [Tooltip("Cooldown, in seconds, between player melee attack swings")]
    public float MeleeAttackCooldown = .4f;
    [Tooltip("Width (player left/right) of the melee attack area, in relative scale")]
    public float MeleeAttackWidth = 1.5f;
    [Tooltip("Length (player forward) of the melee attack area, in relative scale")]
    public float MeleeAttackLength = 2f;
    [Tooltip("Melee Attack Damage")]
    public int MeleeAttackDamage = 10;
    [Tooltip("Push strength of melee attacks (on ragdolls, etc)")]
    public float MeleeAttackForce = 50100f;
    [Tooltip("If true, melee attacks from the player go through walls")]
    public bool PlayerMeleeIgnoresWalls = false;
    [Tooltip("If true, melee attacks from the player can pass through one enemy and hit the one behind them. If player ignores walls, automatically" +
        "has infinite cleave as well, so this value does nothing.")]
    public bool PlayerMeleeCleavesThroughEnemies = true;

    /*
    * RANGED ATTACK SETTINGS
    */
    [Tooltip("Cooldown, in seconds, between player quick ranged attack shots")]
    public float RangeAttackCooldown = .2f;
    [Tooltip("Force of bullet coming out of player's gun")]
    public float RangeAttackForce = 45000f;
    [Tooltip("Player ranged attack damage")]
    public int RangedAttackDamage = 20;

    /*
     * AIMED SHOT SETTINGS
     */
    [Tooltip("Player aimed shot maximum detection range")]
    public float AimedShotDetectionRange = 5;
    [Tooltip("How often, in seconds, the aim mechanism updates state")]
    public float aimedShotTickRate = .1f;
    [Tooltip("State ticks required to line up an aimed shot")]
    public int TicksToLockOn = 20;
    [Tooltip("State ticks required to line up an aimed shot")]
    public int TicksToDisengage = 10;
    [Tooltip("Player aimed shot damage")]
    public int AimedShotDamage = 200;

    /*
     * FIREBALL SETTINGS
     */
    [Tooltip("Time between fireball cooldowns recharging")]
    public float FireballCooldown = 5f;
    [Tooltip("Max number of fireballs that can be stored")]
    public int MaxFireballCharges = 2;
    [Tooltip("Damage from the main zone of the fireball")]
    public int FireballMainDamage = 100;
    [Tooltip("Damage in the area of the fireball")]
    public int FireballAoeDamage = 40;
    [Tooltip("Force of fireball coming out of player's gun")]
    public float FireballAttackForce = 55000f;

    /*
     * DASH SETTINGS
     */
    [Tooltip("Time between dash cooldowns recharging")]
    public float DashCooldown = 5f;
    [Tooltip("Max number of dashes that can be stored")]
    public int MaxDashCharges = 2;
    [Tooltip("Dash duration")]
    public float DashDuration = .15f;

    /*
     * SHIELD SETTINGS
     */
    [Tooltip("Amount of damage the shield absorbs at max effectiveness (e.g., .9 = shield takes 10% of damage)")]
    public float maxShieldEffectiveness = .7f;
    [Tooltip("Amount of damage the shield absorbs at minimum effectiveness (e.g., .1 = shield takes 90% of damage)")]
    public float minShieldEffectiveness = .1f;
    [Tooltip("Effectiveness gained per charge when forming shield")]
    public float shieldEffectivenessGain = .05f;
    [Tooltip("Effectiveness gained per charge when shield is down")]
    public float shieldEffectivenessLoss = .1f;
    [Tooltip("How often, in seconds, the shield gains/loses effectiveness")]
    public float shieldTickRate = .05f;
    [Tooltip("Shield max health")]
    public int shieldMaxHealth = 150;
    [Tooltip("Shield health regenerated per tick")]
    public int shieldHealthRegen = 1;

    private static PlayerSettings instance;

    public static PlayerSettings getInstance() {
        if (instance == null) {
            instance = new PlayerSettings();
        }
        return instance;
    }
}
