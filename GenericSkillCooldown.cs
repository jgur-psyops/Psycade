using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generic container for anything that can have a cooldown that should be displayed on the UI
/// </summary>
/// 
[System.Serializable]
public struct Cooldown {
    public float startTime;
    public float endTime;
    public int currentCharges;
    /// <summary>
    /// If this skill can hold charges, set to some number. If zero, will assume skill has just one charge, and not display the charges images
    /// </summary>
    public int maxCharges;
}

public class GenericSkillCooldown : MonoBehaviour{

    [Tooltip("The event/skill/etc being tracked")]
    private Cooldown toTrack;
    public Image display;
    [Tooltip("Optional, omit if the skill is a single charge")]
    public Image[] charges;


    void Start(){
        toTrack = new Cooldown();
        display.fillAmount = 0;
        if (charges != null){
            for (int i = 0; i < toTrack.currentCharges; i++) {
                charges[i].fillAmount = 0;
            }
        }
    }

    void Update(){
        float now = Time.time;
        float progress = (now - toTrack.startTime) / (toTrack.endTime - toTrack.startTime);
        //Single cooldown skill, simple display
        if (toTrack.maxCharges == 0) { 
            display.fillAmount = progress;
        } else {

            if(toTrack.currentCharges == toTrack.maxCharges) {
                //Whatever you want to animate at max charges
                display.fillAmount = 100;
            } else {
                display.fillAmount = progress;
            }

            //fill in an image for each charge, hide the rest
            for(int i=0; i<toTrack.currentCharges; i++) {
                charges[i].fillAmount = 100;
            }
            for(int i=toTrack.currentCharges; i<charges.Length; i++) {
                charges[i].fillAmount = 0;
            }
        }
    }

    public void updateSkillCooldown(float startTime, float endTime, int currentCharges, int maxCharges) {
        toTrack.startTime = startTime;
        toTrack.endTime = endTime;
        toTrack.currentCharges = currentCharges;
        toTrack.maxCharges = maxCharges;
    }

    /// <summary>
    /// Updates charges only
    /// </summary>
    /// <param name="currentCharges"></param>
    public void updateCharges(int currentCharges) {
        toTrack.currentCharges = currentCharges;
    }

}
