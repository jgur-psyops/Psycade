using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBeamContract : MonoBehaviour{

    private LineRenderer line;

    void Start(){
        line = GetComponent<LineRenderer>();
        Destroy(this.gameObject, 3f);
    }


    void Update(){
        if(line==null || line.positionCount < 2) {
            return;
        }
        //The end of the beam (the enemy)
        Vector3 origin = line.GetPosition(0);
        //The "start" of the beam (the player)
        Vector3 terminus = line.GetPosition(1);
        //TODO deltatime smooth the beam
        Vector3 newOrigin = Vector3.Lerp(terminus, origin, .97f);
        line.SetPosition(0, newOrigin);
    }
}
