using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class excitatory2d : MonoBehaviour {
    public int id;
    public Color color;
    public bool activation;
    public int latestActivationTime;
    private Color excitatoryColor;

    void Start() {
        Color myColor2 = transform.parent.GetComponent<MC>().COLOR;
        activation = false;
        latestActivationTime = 0;
    }
    
    //attributes of the pyramid cells
    public Color COLOR {
        get {
            return this.color;
        }
        set {
            this.color = value;
        }
    }
    public int PreviousTime {
        get {
            return this.latestActivationTime;
        }
        set {
            this.latestActivationTime = value;
        }
    }
    public bool IsActive {
        get {
            return this.activation;
        }
        set {
            if (this.activation) {
                //the animation call here
                // StartCoroutine(DeactivateExc());
            } else {
                //StartCoroutine(ActivateExc());
            }
            this.activation = value;
        }
    }
}