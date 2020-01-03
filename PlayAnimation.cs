using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayAnimation : MonoBehaviour {
	//Animation anim;
	//AnimationClip clip;
	//AnimationCurve curve;
	//public bool isActive = false;
    private Color excitatoryColor; //color of the pyramid cells

    /* 
    these methods were supposed to manage the color-change animation through keyframes

	void Start()
	{
		anim = GetComponent<Animation>();
		clip = new AnimationClip();
		clip.legacy = true;

	}

	public void ChangeCol(){
		//print("bruh");
		Keyframe[] keys = new Keyframe[3];
        keys[0] = new Keyframe(0f, 1f);
    	keys[1] = new Keyframe(speed*5, 0f);
    	keys[2] = new Keyframe(speed*10, 1f);
    	//keys[2] = new Keyframe(speed*X, 1f);
		curve = new AnimationCurve(keys);
    	clip.SetCurve("", typeof(Material), "_Color.g", curve);
    	clip.SetCurve("", typeof(Material), "_Color.b", curve);
		anim.AddClip(clip, clip.name);
		anim.Play(clip.name);
       	//transform.GetComponent<Renderer>().material.color = Color.red;
	}
    */

    //this method was supposed to manage the color-change of basket cells as a function of the change speed
    public void ActivatedInhibitory(float speed){
		//StartCoroutine(ActivateInh(speed));
		//Invoke("InhibitoryNormalState", 1.1f-speed);
		//isActive = true;
		//transform.GetComponent<Renderer>().material.color = Color.blue;
		/*for(float i=0; i<1; i=i+0.1f){
			StartCoroutine("ActivateInh");
		}*/
	}
    
    //these methods respectively changes color of basket cells from white to blue and enlarges them, and vice versa, when they are activated/deactivated, then wait for a given amount of time
    public IEnumerator ActivateInh(float speed) {
        for (float i = 1; i <=10; i++) {
            transform.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, Color.blue, i*0.1f);
            transform.localScale = Vector3.Lerp(CreateNeurons.basketScale, (CreateNeurons.basketScale * 2f), (i*0.1f));
            yield return new WaitForSeconds((speed));
        }
    }
    public IEnumerator DeactivateInh(float speed) {
		for(float i=1; i<=10; i++) {
			transform.GetComponent<Renderer>().material.color = Color.Lerp(Color.blue,Color.white,i*0.1f);
            transform.localScale = Vector3.Lerp(CreateNeurons.basketScale * 2f, CreateNeurons.basketScale, (i * 0.1f));
            yield return new WaitForSeconds((speed));
        }
	}
    
    //these methods respectively changes color of pyramid cells from white to the color of the MC and enlarges them, and vice versa, when they are activated/deactivated, then wait for a given amount of time
    public IEnumerator ActivateExc(float speed) {
        //float alpha = 0f;
        for (float i = 1; i <= 10; i++) {
            /* 
            if (i < 1)
                alpha = 0.1f;
            else
                alpha = Mathf.Lerp(transform.GetComponent<Excitatory>().pyramidShader[0].color.a, 1f, i * 0.1f);
            excitatoryColor = transform.parent.GetComponent<MC>().COLOR;
            excitatoryColor.a = alpha;
            */

            //transform.GetComponent<Renderer>().material.color = Color.Lerp(transform.GetComponent<Excitatory>().pyramidShader[0].color, excitatoryColor, i * 0.1f);
            
            transform.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, transform.parent.GetComponent<MC>().COLOR, i * 0.1f);
            transform.localScale = Vector3.Lerp(CreateNeurons.pyramidScale, (CreateNeurons.pyramidScale * 2.5f), (i*0.1f));
            yield return new WaitForSeconds((speed));
        }
    }
	public IEnumerator DeactivateExc(float speed) {
        // float alpha = 0f;
        for (float i = 1; i <= 10; i++) {
            /*
            if (i < 1)
                alpha = 0.1f;
            else
                alpha = Mathf.Lerp(transform.GetComponent<Excitatory>().pyramidShader[0].color.a, 1f, i * 0.1f);
            excitatoryColor = transform.parent.GetComponent<MC>().COLOR;
            excitatoryColor.a = alpha;
            */

            transform.GetComponent<Renderer>().material.color = Color.Lerp(transform.parent.GetComponent<MC>().COLOR, Color.white, i * 0.1f);
            transform.localScale = Vector3.Lerp(CreateNeurons.pyramidScale * 2.5f, CreateNeurons.pyramidScale, (i * 0.1f));

            //transform.parent.GetComponent<Renderer>().material.color = Color.Lerp(transform.parent.GetComponent<MC>().COLOR, Color.white, i * 0.1f);

            yield return new WaitForSeconds((speed));
        }
	}
}
