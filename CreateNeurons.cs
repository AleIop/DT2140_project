using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Linq;


public class CreateNeurons : MonoBehaviour {


	//Prefabs = "generic templates" for every type of structure, by instantiating one you "create" an object of that type
	public GameObject myHC;
	public GameObject myMC;
	public GameObject myPyramid;
	public GameObject myBasket;
	public GameObject myBigBasket;
    public static Vector3 pyramidScale;
    public static Vector3 basketScale;


    public static List<GameObject> listHC; //hypercolumns
	public static List<GameObject> listMC; //minicolumns
	public static List<GameObject> listNI; //inhibitory = baskets
    public static List<GameObject> listNE; //exitatory = pyramids
    public static List<GameObject> listBB; //big basket
    public static GameObject[] listN; //all neurons

	//Identification number
	protected static int idHC=0;
	protected static int idMC=0;
	private int tempID;

	//MC object data
  	private float colliderRadius;
  	public float mcRadius;
  	private Vector3 scale;
  	private float a;
  	private float b;

	private int[] test;

	public bool vView = false;
	public bool hView = true;
	public GameObject hcContainer; //empty gameobject act as parent for all the hc
	public GameObject dummyGameObject; // this one is a dummy gameobject doing what hccontainer should be doing but since the positioning of the hc are off on the first place, this is the easiest solution.
 
	private Color[] myColor; //array of colors for the MCs gradient --> from red to green

	/*
		this method instantiates the lists, resizes the generic MC and sets the HCs displacements
		it is called in Controlla.cs, after DataReader.Initiate()
	*/
	public void Initiate(){
		hcContainer = new GameObject("HC-Cointainer");
		dummyGameObject = new GameObject("DummyGameObject");
		listHC = new List<GameObject>();
		listMC = new List<GameObject>();
		listNI = new List<GameObject>();
        listNE = new List<GameObject>();
        listBB = new List<GameObject>();
        listN = new GameObject[DataReader.n_neurons+3]; //6146 neurons as game objects... plus 3
		colliderRadius = myMC.GetComponent<CapsuleCollider>().radius;
    	scale = myMC.transform.localScale*4; //makes the minicolumn bigger
    	mcRadius = colliderRadius * scale.x * 1.2f; //makes the collider bigger
		test = new int[DataReader.n_HC]; //array of 16 ints, because 16 HCs... duh

		//initial displacement values for the 4 rows of 4 HCs
		test [0] = 0;
		test [1] = 0;
		test [2] = 0;
		test [3] = 0;
		test [4] = 7;
		test [5] = 7;
		test [6] = 7;
		test [7] = 7;
		test [8] = 14;
		test [9] = 14;
		test [10] = 14;
		test [11] = 14;
		test [12] = 21;
		test [13] = 21;
		test [14] = 21;
		test [15] = 21;
        StartHC();
    }

	/*
		this method calls HC.testingMe() for each one of the 16 HCs
	*/
    public void StartHC()
    {
        foreach(var hc in listHC)
        {
            hc.GetComponent<HC>().testingMe();
        }
    }

	void Update()
	{
		if (Input.GetKey(KeyCode.V)){
			ScaleUp();
		}
		if (Input.GetKey(KeyCode.B)){
			ScaleDown();
		}

		if (Input.GetKey(KeyCode.X)){
				VerticalView();
		}
		if (Input.GetKey(KeyCode.Z)){
				HorizontalView();
		}
	}

	/*
		this method creates and positions HCs, MCs, pyramid and basket cells
		it is called in Controlla.cs, after CreateNeurons.Initiate()
	*/
 	public void Create () {
		myColor = new Color[DataReader.n_MC_per_HC];
	 	CreateColor();
		float theta = 2*Mathf.PI/DataReader.n_MC_per_HC; //angle of displacement for MCs within a HC around a circle

		//Debug.Log(DataReader.n_HC);
		//creates a 4x4 matrix of HCs, made of minicolumns in a circle
		for (int i=0; i<DataReader.n_HC; i++){
			int modulo = (i % 4);
			a=test[i]; //x displacement for rows of HCs
			b= 7 * modulo; //y displacement for rows of HCs

			//Quaternion.identity is the identity rotation matrix, it doesn't apply any rotation
			GameObject newHC= (GameObject)Instantiate(myHC, new Vector3 (a, 0, b), Quaternion.identity);
			GameObject bigBasket = (GameObject)Instantiate(myBigBasket, new Vector3 (a, 2, b), Quaternion.identity);
			//listBB.Add(bigBasket);

			newHC.name = "Hypercolumn " + idHC;
			newHC.GetComponent<HC>().ID=idHC;
            newHC.transform.parent = hcContainer.transform;
            bigBasket.transform.parent = hcContainer.transform;

            Color whitecolor = Color.white;
            whitecolor.a = 0.05f;
            listHC.Add(newHC); //adds every HC in the list
            listBB.Add(bigBasket);

			//creates and instantiates MCs for each HCs
            for (int j=0; j<DataReader.n_MC_per_HC; j++){
				GameObject tempMC = (GameObject)Instantiate(myMC, new Vector3 (a + mcRadius * Mathf.Cos (theta*j), 2, b + mcRadius * Mathf.Sin (theta*j)), Quaternion.identity);
				tempMC.name = "Minicolumn " + idMC;
				tempMC.transform.parent= listHC[i].transform;
				tempMC.GetComponent<MC>().ID = idMC;
				tempMC.GetComponent<MC>().COLOR = myColor[j];
                tempMC.GetComponent<Renderer>().material.color = whitecolor;
				listMC.Add(tempMC); //adds every MC in the list
				idMC++; //increments id for next MC in the loop
			}
			idHC++; //increments id for next HC in the loop
        }

		//creates Pyramid cells within each MC and places them randomly
		for(int j=0; j<DataReader.n_MC; j++){
			List<int> eRecMC = DataReader.e_rec_MC[j]; //list of excitatory cell ids in every MC
			for(int k=0; k<eRecMC.Count(); k++){
				//tempID = eRecMC[k]-1;
				GameObject tempPyramid = (GameObject)Instantiate(myPyramid, new Vector3 (listMC[j].transform.position.x+(Random.Range(-0.32f,0.32f)),Random.Range(0.2f,3.7f),listMC[j].transform.position.z+(Random.Range(-0.32f,0.32f))), Quaternion.identity);
				tempPyramid.name = "Pyramidcell " + eRecMC[k];
				tempPyramid.transform.parent = listMC[j].transform;
				tempPyramid.transform.parent.transform.parent.GetComponent<HC>().myExcitatory.Add(eRecMC[k]);
				tempPyramid.GetComponent<Excitatory>().id = eRecMC[k];
				tempPyramid.GetComponent<Excitatory>().COLOR = tempPyramid.transform.parent.GetComponent<MC>().COLOR;

				listN[eRecMC[k]] = tempPyramid; //adds every Pyramid in the list of all the cells... why?
				listNE.Add(tempPyramid); //adds every Pyramid in the list of pyramids
			}
		}

		//creates basket cells within each HC and places them randomly
		for(int i=0; i<DataReader.n_HC; i++){
			List<int> iPopHC = DataReader.i_pop_HC[i]; //list of excitatory cell ids in every HC
			for( int k=0; k<iPopHC.Count(); k++){
				//tempID = iPopHC[k]-1;
				GameObject tempBasket = (GameObject)Instantiate (myBasket, new Vector3 (listHC[i].transform.position.x+(Random.Range(-1f,1f)),Random.Range(1.0f,3f), listHC[i].transform.position.z+(Random.Range(-1f,1f))), Quaternion.identity);
				tempBasket.name = "Basketcell " +iPopHC[k]; //inhibitory
				tempBasket.transform.parent = listHC[i].transform;
				tempBasket.transform.parent.GetComponent<HC>().myInhibitory.Add(iPopHC[k]);
				tempBasket.GetComponent<Inhibitory>().id = iPopHC[k];
				tempBasket.GetComponent<Inhibitory>().COLOR = Color.blue;
				//tempBasket.GetComponent<Renderer>().enabled = false;
				listN[iPopHC[k]] = tempBasket;
				listNI.Add(tempBasket);
			}
		}

		//tranlates the parent game objects
		hcContainer.transform.localPosition = new Vector3(-7.5f,0,-7.5f);
		hcContainer.transform.parent = dummyGameObject.transform;
        dummyGameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        pyramidScale = listNE[0].transform.localScale;
        basketScale = listNI[0].transform.localScale;

        print("initialization done");
	}

	/*
		this method creates a list of rainbow gradient colors for Minicolumns
		it is called in this.Create()
	*/
	void CreateColor(){
		Color firstColor= Color.HSVToRGB(0,1,0.5f); //dark red
		Color lastColor = Color.HSVToRGB(0.77f,1,0.5f); //dark green
		float myFloat = (float)1f/(DataReader.n_MC_per_HC+3); //to equally distribute colors between MCs within each HC 
		myColor[0] = firstColor;
		myColor[DataReader.n_MC_per_HC-1]= lastColor;
		for(int i=1;i<DataReader.n_MC_per_HC-1;i++){
			myColor[i] = Color.HSVToRGB(((float)i*myFloat),1,0.5f);
		}
	}

	/*
		this method "deactivates" all pyramid and basket cells, separately
	*/
    public static void UnactivateAll()
    {
        foreach (GameObject excitatory in listNE){
            excitatory.GetComponent<Excitatory>().IsActive = false;
            excitatory.GetComponent<Excitatory>().latestActivationTime = 0;

            excitatory.GetComponent<Renderer>().material.color = Color.white;
        }

        foreach (GameObject inhibitory in listNI){
            inhibitory.GetComponent<Inhibitory>().IsActive = false;
            inhibitory.GetComponent<Inhibitory>().latestActivationTime = 0;

            inhibitory.GetComponent<Renderer>().material.color = Color.white;
        }
    }

	/*
		this method "activates"/"deactivates" only the MCs
	*/
    public void ToggleMC(bool BOOLEAN){
		foreach (GameObject mc in listMC)
		{
			mc.GetComponent<Renderer>().enabled = BOOLEAN;
		}
    }

	/*
		this method "activates"/"deactivates" only the pyramid/excitatory cells
	*/
	public void ToggleExhibitory(bool BOOLEAN){
		foreach(GameObject exhib in listNE){
          	exhib.GetComponent<Renderer>().enabled= BOOLEAN;
        }
    }

	/*
		this method "activates"/"deactivates" only the basket/inhibitory cells
	*/
	public void ToggleInhibitory(bool BOOLEAN){
		foreach(GameObject inhib  in listNI){
          	inhib.GetComponent<Renderer>().enabled= BOOLEAN;
        }
    }


	/*
		methods zoom in/out and rotate view to vertical/horizontal
	*/
	public void ScaleUp(){
		if(dummyGameObject.transform.localScale.x <10){
			dummyGameObject.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
		}
	}
	public void ScaleDown(){
		if(dummyGameObject.transform.localScale.x >0.15f){
			dummyGameObject.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
		}
	}
	public void VerticalView(){
		dummyGameObject.transform.rotation = Quaternion.Euler(90,0,0);
		vView = true;
		hView=false;
	}	
	public void HorizontalView(){
		dummyGameObject.transform.rotation = Quaternion.Euler(0,0,0);
		vView = false;
		hView=true;
	}

	/*
		this method was supposed to keep only a selected HC active while it deactivates all the others
	*/
	public void blabla(int NeuronID){
		for (int i=0; i<DataReader.n_HC;i++){
			if(i!=NeuronID){
				for (int j=0; j<DataReader.n_MC_per_HC;j++){
					//listHC[i].GetChild(j).GetComponent<GameObject>().GetComponent<Renderer>().enabled = false;
				}
			}
		}
		for (int j=0; j<DataReader.n_MC_per_HC;j++){ //THIS IS REDUNDANT
			//listHC[NeuronID].GetChild(j).GetComponent<Renderer>().enabled = true;
		}
	}
}
