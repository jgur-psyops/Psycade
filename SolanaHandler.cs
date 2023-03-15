using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SolanaHandler : MonoBehaviour {

    public float sol;
    /// <summary>
    /// mint: E6Z6zLzk8MWY3TY8E87mr88FhGowEPJTeMWzkqtL6qkF
    /// </summary>
    public float usdc;
    /// <summary>
    /// mint: C6kYXcaRUMqeBF5fhg165RWU7AnpT9z92fvKNoMqjmz6
    /// </summary>
    public float btc;
    public bool walletConnected;

    private bool playEnabled = false;

    public TextMeshProUGUI toastText;
    public GameObject playButton;
    public GameObject mainCanvas;
    public GameObject walletHolder;
    public SpawnEnemies spawner;

    void Start(){
        toastText.text = "Connect a wallet to play";
    }

    void Update() {
        
    }

    public void checkState() {
        if (!walletConnected) {
            playEnabled = false;
            toastText.text = "Connect a wallet to play";
        }else if (sol < 1) {
            playEnabled = false;
            toastText.text = "You must have a balance of 1 lamport to play";
        }else if(usdc < 1) {
            playEnabled = true;
            toastText.text = "You can play, but you must hold 1 usdc to use fireballs. Try refreshing!";
        } else {
            playEnabled = true;
            toastText.text = "Ready to play!";
        }

        if (playEnabled) {
            playButton.SetActive(true);
        } else {
            playButton.SetActive(false);
        }
    }

    public void onPlayButton() {
        if (playEnabled) {
            walletHolder.SetActive(false);
            mainCanvas.SetActive(false);
            spawner.isPaused = false;
        }
    }

    
}
