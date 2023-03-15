using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour{

    [Tooltip("Locations where enemies can be dropped in")]
    public Transform[] spawnLocations;
    [Tooltip("Types of units that can spawn, in ascending order of difficulty")]
    public GameObject[] spawnTypes;
    [Tooltip("For each unit in spawnTypes, assign a power rating, which will determine spawn rate")]
    public int[] spawnValues;

    [Tooltip("Time, in seconds between wave spawns")]
    public float spawnTime;
    [Tooltip("Max enemies spawned per wave. Must be smaller or equal to spawn points available")]
    public int maxSpawns;

    [Tooltip("Higher seed = more, stronger spawns")]
    public int initialSeed;
    [Tooltip("Higher max = more time to expand difficulty")]
    public int maxSeed;
    [Tooltip("Faster increase = more, stronger spawns, sooner")]
    public int seedIncreasePerSecond;

    [Tooltip("Current spawn seed, will be overriden by script")]
    public int currentSeed;

    [Tooltip("Completely stop the spawner until false")]
    public bool isPaused = true;


    void Start() {
        currentSeed = initialSeed;
        StartCoroutine(nameof(IncreaseDifficulty));
        StartCoroutine(nameof(SpawnWave));
    }

    void Update() {
        
    }

    /// <summary>
    /// Resets to initial seed value
    /// </summary>
    public void ResetInitial() {
        currentSeed = initialSeed;
    }

    /// <summary>
    /// Picks how many enemies will spawn this wave
    /// </summary>
    /// <returns>How many enemies will spawn this wave</returns>
    int pickQuantity() {
        float random = Random.Range(currentSeed, maxSeed + 1);
        int spawns = (int) (random / maxSeed * maxSpawns);
        return spawns;
    }

    /// <summary>
    /// Picks an index from the spawnTypes array
    /// </summary>
    /// <returns>An index from the spawnTypes array</returns>
    int pickSpawn() {
        float random = Random.Range(0, currentSeed);
        for(int i=spawnValues.Length-1; i>=0; i--) {
            if (spawnValues[i] < random) {
                return i;
            }
        }
        return 0;
    }

    IEnumerator IncreaseDifficulty() {
        while (true) {
            yield return new WaitForSeconds(1f);
            if (!isPaused) {
                if (currentSeed + seedIncreasePerSecond < maxSeed) {
                    currentSeed += seedIncreasePerSecond;
                } else {
                    currentSeed = maxSeed;
                }
            }
        }
    }

    IEnumerator SpawnWave() {
        while (true) {
            yield return new WaitForSeconds(spawnTime);
            if (!isPaused) {
                int spawns = pickQuantity();
                List<Transform> locationsUnpicked = new List<Transform>(spawnLocations);
                for (int i = 0; i < spawns; i++) {
                    int index = Random.Range(0, locationsUnpicked.Count);
                    Transform location = locationsUnpicked[index];
                    locationsUnpicked.RemoveAt(index);
                    int spawnType = pickSpawn();
                    Instantiate(spawnTypes[spawnType], location.position + new Vector3(0f, 100f, 0f), Quaternion.identity);
                }
            }
        }
    }
}
