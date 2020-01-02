using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Excitatory : MonoBehaviour {
	public int id;
	public Color color;
    //public Material activeShader;
    //public Material[] pyramidShader;
    public bool activation;
    public int latestActivationTime;
    //private Color excitatoryColor;

    /*
        this method initiates the single pyramid cell
    */
    void Start()
    {
        /*
        Color myColor = pyramidShader[0].color;
        myColor.a = 0.1f;
        Color myColor2 = transform.parent.GetComponent<MC>().COLOR;
        myColor2.a = 1f;
        pyramidShader[0].color = myColor;
        pyramidShader[1].color = myColor2;
        */

        activation = false;
        latestActivationTime = 0;

        //GetComponent<Renderer>().material = pyramidShader[0];
        //GetComponent<Renderer>().material = pyramidShader[1];
    }


    /*
        properties for color, previous activation time, activation status of the single pyramid cell
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
                //StartCoroutine(DeactivateExc());
            } else {
                //StartCoroutine(ActivateExc());
            }
            this.activation = value;
        }
    }

}
