using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnHunterKiller : MonoBehaviour{

    public GameObject HunterKiller;
    [Tooltip("Enemies will only spawn HKs when in this state")]
    public EnemyAI.State spawnState;

    private float baseSpawnCooldown;
    /// <summary>
    /// True if ready to spawn a new HK
    /// </summary>
    private bool isReady = true;

    private EnemySettings settings;
    private EnemyAI.StateController aiController;

    void Start(){
        aiController = GetComponent<EnemyAI.StateController>();
        settings = EnemySettings.getInstance();
        baseSpawnCooldown = settings.HunterKillerSpawnInterval;
    }


    void Update(){
        if (isReady && aiController.currentState == spawnState) {
            GameObject HK = Instantiate(HunterKiller, transform.position, Quaternion.identity);
            NavtoPlayer nav = HK.GetComponent<NavtoPlayer>();
            nav.player = aiController.aimTarget.gameObject;
            isReady = false;
            StartCoroutine("HunterKillerCooldown");
        }
    }

    IEnumerator HunterKillerCooldown() {
        yield return new WaitForSeconds(baseSpawnCooldown);
        isReady = true;
        yield return null;
    }
}
