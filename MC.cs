using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC:MonoBehaviour{

	private int id;
	public Color color;
    public bool isActive;
    public Dictionary<int, List<GameObject>> activation;
    public Dictionary<int, List<GameObject>> deactivation;
//	public AnimationClip myAnimClip;
//	AnimationCurve curve;
/* 
	public AnimationClip ANIMCLIP{
		get{
			return this.myAnimClip;
		}set{
			return this.myAnimClip = value;
		}
	}
	private void CreateAnimationClip(){

	}*/
	public int ID
	{
		get
		{
			return this.id;
		}
		set
		{
			this.id = value;
		}
	}

	public Color COLOR
	{
		get
		{
			return this.color;
		}
		set
		{
			this.color = value;
		}
	}

}
