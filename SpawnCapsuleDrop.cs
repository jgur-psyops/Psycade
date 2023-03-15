using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCapsuleDrop : MonoBehaviour { 

    [Tooltip("Walls will gain a kinematic rb and become parentless on death")]
    public GameObject[] ragdollParts;
    [Tooltip("Prefab to spawn inside the box upon landing")]
    public GameObject spawnOnLanding;
    [Tooltip("Inherits the x/z location of this location, uses the y location of the floor")]
    public Transform spawnLocation;
    [Tooltip("Specify the exact top of the floor so enemies can spawn on the navmesh correctly")]
    public float floorY;
    [Tooltip("Walls of the drop pod shoot off with this much force")]
    public float dropPodDropForce;
    [Tooltip("Line renderer that tracks drop area can be turned off after landing")]
    public GameObject dropIndicator;

    private bool isDropped = false;

    void Start(){
        // Clean drop pods that fail to reach the surface
        Destroy(gameObject, 120f);
    }

    void Update(){
        
    }

    private void OnCollisionEnter(Collision collision) {
        if (!isDropped) {
            isDropped = true;
            GetComponent<BoxCollider>().enabled = false;
            dropIndicator.SetActive(false);
            foreach (GameObject ragdollPart in ragdollParts) {
                ragdollPart.SetActive(true);
                ragdollPart.transform.parent = null;
                ragdollPart.AddComponent<Rigidbody>();
                ragdollPart.GetComponent<BoxCollider>().enabled = true;
                Rigidbody rb = ragdollPart.GetComponent<Rigidbody>();
                rb.AddExplosionForce(dropPodDropForce, collision.contacts[0].point, 15f);
            }
            Instantiate(spawnOnLanding, new Vector3(spawnLocation.position.x, floorY, spawnLocation.position.z), Quaternion.identity);
            StartCoroutine(nameof(KillRagdolls));
        } else {
            // do nothing
        }
    }

    IEnumerator KillRagdolls() {
        while (true) {
            yield return new WaitForSeconds(EnemySettings.getInstance().dropPodPartsDespawnTime);
            foreach (GameObject ragdollPart in ragdollParts) {
                Destroy(ragdollPart);
            }
            Destroy(gameObject);
        }
    }
}
