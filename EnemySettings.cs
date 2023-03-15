using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A singleton that tracks various properties of enemies, such as cooldowns
/// </summary>
public class EnemySettings{

    /*
     * Enemy projectile settings
     */
    [Tooltip("Damage dealt by standard enemy projectile")]
    public int SmallBulletDamage = 20;

    /*
     * Hunter Killer settings
     */
    [Tooltip("Cooldown, in seconds, for enemies to respawn hunter-killers")]
    public float HunterKillerSpawnInterval = 20f;
    [Tooltip("Time, in seconds, hunter killers live before suiciding")]
    public float HunterKillerLifespan = 20f;
    [Tooltip("Damage done by hunter killers")]
    public int HunterKilledExplosionDamage = 100;
    [Tooltip("Radius of hunter killer explosion")]
    public float HunterKillerExplosionRadius = 5f;
    [Tooltip("Delay, in seconds, before hunter killers explode once in range")]
    public float HunterKilledExplosionDelay = 1f;
    [Tooltip("Delay, in seconds, before hunter killers start moving after spawning in")]
    public float HunterKillerStartMoveDelay = 3f;
    [Tooltip("Speed when in the turning towards target state (must be >0")]
    public float HunterKillerTurnMoveSpeed = 1f;
    [Tooltip("Speed when in the running towards target state (must be >0")]
    public float HunterKillerRunMoveSpeed = 4f;
    [Tooltip("Force of the push back from hunter killer explosion")]
    public float HunterKilledExplosionForce = 20000f;

    /*
     * AK settings
     */
    [Tooltip("Force given to each bullet fired by the AK")]
    public float AKBulletForce = 25000f;

    /*
     * Gun turret settings
     */
    [Tooltip("Delay, in seconds, before shots")]
    public float TurretAttackDelay = .2f;
    [Tooltip("Force given to each bullet fired by the Turret")]
    public float TurretBulletForce = 25000f;
    [Tooltip("Turret accuracy (0 is perfect, higher is worse).")]
    public float TurretShotErrorRate = .2f;
    [Tooltip("Turret rotation speed.")]
    public float TurretRotationSpeed = 2f;

    /*
     * Big Spider Settings
     */
    [Tooltip("Delay, in seconds, before spiders start moving after spawning in")]
    public float SpiderStartMoveDelay = 3f;
    [Tooltip("Damage dealt by spider beam/projectile per pulse")]
    public int SpiderBulletDamage = 40;
    [Tooltip("Spider engagement distance (weapon starts to charge)")]
    public float SpiderEngagementDistance = 15f;
    [Tooltip("Spider turret rotation speed (uses rotateTowards)")]
    public float SpiderTurretRotationSpeed = .75f;
    [Tooltip("Delay between weapon damage pulses")]
    public float SpiderAttackDelay = .2f;
    [Tooltip("Speed when in the turning towards target state (must be >0)")]
    public float SpiderTurnMoveSpeed = 1f;
    [Tooltip("Speed when in the running towards target state (must be >0)")]
    public float SpiderRunMoveSpeed = 2f;

    [Tooltip("Ragdoll corpses are destroyed after this time")]
    public float corpseDespawnTime = 60f;

    [Tooltip("Ragdoll parts are destroyed (typically with a kaboom) after this time")]
    public float corpsePartsDespawnTime = 10f;
    [Tooltip("Walls of the drop pod are destroyed after this time")]
    public float dropPodPartsDespawnTime = 30f;
    [Tooltip("Drop pods start this high in the sky")]
    public float dropPodDropHeight = 100f;

    public enum EnemyType {
        HunterKiller,
        AK,
        GunTurret,
        Spider,
    }

    private static EnemySettings instance;

    public static EnemySettings getInstance() {
        if (instance == null) {
            instance = new EnemySettings();
        }
        return instance;
    }
}
