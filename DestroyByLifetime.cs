using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByLifetime : MonoBehaviour{

    public float lifetime;
    void Start(){
        Destroy(gameObject, lifetime);
    }

    void Update() {
        
    }
}
