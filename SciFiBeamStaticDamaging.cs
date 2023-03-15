using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemySettings;

public class SciFiBeamStaticDamaging : MonoBehaviour
{

    [Header("Prefabs")]
    public GameObject beamLineRendererPrefab; //Put a prefab with a line renderer onto here.
    public GameObject beamStartPrefab; //This is a prefab that is put at the start of the beam.
    public GameObject beamEndPrefab; //Prefab put at end of beam.

    [Tooltip("Type of enemy using this beam")]
    public EnemyType ownerType;
    [Tooltip("Will only attempt to do damage if colliding with something with a matching tag (Excludes shield!)")]
    public string[] tagsToCollideWith;

    private GameObject beamStart;
    private GameObject beamEnd;
    private GameObject beam;
    private LineRenderer line;
    private EnemySettings enemySettings;
    private int damage = 5;
    private float interval = .1f;
    /// <summary>
    /// Root of gameobject struck by the end of the beam
    /// </summary>
    public GameObject targetHit;
    /// <summary>
    /// Point struck for collision animation purposes
    /// </summary>
    private Vector3 hitPoint;

    [Header("Beam Options")]
    public bool alwaysOn = true; //Enable this to spawn the beam when script is loaded.
    public bool beamCollides = true; //Beam stops at colliders
    public float beamLength = 100; //Ingame beam length
    public float beamEndOffset = 0f; //How far from the raycast hit point the end effect is positioned
    public float textureScrollSpeed = 0f; //How fast the texture scrolls along the beam, can be negative or positive.
    public float textureLengthScale = 1f;   //Set this to the horizontal length of your texture relative to the vertical. 
                                            //Example: if texture is 200 pixels in height and 600 in length, set this to 3

    void Start(){
        enemySettings = EnemySettings.getInstance();
        if( ownerType == EnemyType.Spider){
            damage = enemySettings.SpiderBulletDamage;
            interval = enemySettings.SpiderAttackDelay;
        }
        StartCoroutine("WeaponDamage");
    }

    private void OnEnable(){
        if (alwaysOn) //When the object this script is attached to is enabled, spawn the beam.
            SpawnBeam();
    }

    private void OnDisable() {
        RemoveBeam();
    }

    void FixedUpdate(){
        if (beam) //Updates the beam
        {
            line.SetPosition(0, transform.position);

            Vector3 end;
            RaycastHit hit;
            int ignoredLayers = ~LayerMask.GetMask("PlayerRotationTarget", "EnemyRangefinder", "EnemyProjectile", "PlayerProjectile");
            if (beamCollides && Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, ignoredLayers)) {
                // hitting the shield marks the shield as the hit target, all other hits record the root (to seek Health)
                if (hit.collider.transform.CompareTag("PlayerShield")) {
                    targetHit = hit.collider.gameObject.transform.gameObject;
                } else {
                    targetHit = hit.collider.gameObject.transform.root.gameObject;
                }
                hitPoint = hit.point;
                end = hit.point - (transform.forward * beamEndOffset);
            } else {
                end = transform.position + (transform.forward * beamLength);
            }
            line.SetPosition(1, end);

            if (beamStart)
            {
                beamStart.transform.position = transform.position;
                beamStart.transform.LookAt(end);
            }
            if (beamEnd)
            {
                beamEnd.transform.position = end;
                beamEnd.transform.LookAt(beamStart.transform.position);
            }

            float distance = Vector3.Distance(transform.position, end);
            line.material.mainTextureScale = new Vector2(distance / textureLengthScale, 1); //This sets the scale of the texture so it doesn't look stretched
            line.material.mainTextureOffset -= new Vector2(Time.deltaTime * textureScrollSpeed, 0); //This scrolls the texture along the beam if not set to 0
        }
    }

    public void SpawnBeam() {
        if (beamLineRendererPrefab)
        {
            if (beamStartPrefab)
                beamStart = Instantiate(beamStartPrefab);
            if (beamEndPrefab)
                beamEnd = Instantiate(beamEndPrefab);
            beam = Instantiate(beamLineRendererPrefab);
            beam.transform.position = transform.position;
            beam.transform.parent = transform;
            beam.transform.rotation = transform.rotation;
            line = beam.GetComponent<LineRenderer>();
            line.useWorldSpace = true;
            #if UNITY_5_5_OR_NEWER
			line.positionCount = 2;
			#else
			line.SetVertexCount(2); 
			#endif
        }
        else
            print("Add a hecking prefab with a line renderer to the SciFiBeamStatic script on " + gameObject.name + "! Heck!");
    }

    public void RemoveBeam() {
        if (beam)
            Destroy(beam);
        if (beamStart)
            Destroy(beamStart);
        if (beamEnd)
            Destroy(beamEnd);
    }

    private bool MatchesSomeTag(Transform t) {
        for (int i=0; i< tagsToCollideWith.Length; i++) {
            if (t.CompareTag(tagsToCollideWith[i])) {
                return true;
            }
        }
        return false;
    }

    IEnumerator WeaponDamage() {
        while (true) {
            yield return new WaitForSeconds(interval);
            if (targetHit != null && MatchesSomeTag(targetHit.transform)) {
                HealthManager health = targetHit.GetComponent<HealthManager>();
                if (health != null) {
                    // Safe bet that the direction is a straight line from the emitter to the target
                    Vector3 direction = (transform.position - hitPoint).normalized;
                    Vector3 hitPosition = hitPoint;
                    health.TakeDamage(hitPosition, direction, damage);
                }
            }
            else if (targetHit != null && targetHit.transform.CompareTag("PlayerShield")) {
                DirectionalEnergyShield shield = targetHit.transform.GetComponent<DirectionalEnergyShield>();
                shield.damageShield(damage);
            }
        }
    }
}