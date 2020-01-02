using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inhibitory : MonoBehaviour {

    public int id;
    public Color color;
    public bool activation;
    public int latestActivationTime;

    /*
        properties for color, previous activation time, activation status of the single basket cell
    */
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
                //StartCoroutine(DeactivateInh());
            } else {
                //StartCoroutine(ActivateInh());

            }
            this.activation = value;
        }
    }
}
