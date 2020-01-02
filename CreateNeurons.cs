using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Linq;


public class CreateNeurons : MonoBehaviour {


	//Prefabs
	public GameObject myHC;
	public GameObject myMC;
	public GameObject myPyramid;
	public GameObject myBasket;
	public GameObject myBigBasket;
    public static Vector3 pyramidScale;
    public static Vector3 basketScale;


    //List
    public static List<GameObject> listHC;
	public static List<GameObject> listMC;
	public static List<GameObject> listNI;
    public static List<GameObject> listNE;
    public static List<GameObject> listBB;
    public static GameObject[] listN;

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
	public GameObject dummyGameObject; // this one is a dummy gameobject doing what hccontainer should be doing but
	//since the positioning of the hc are off on the first place, this is the easiest solution.
 
	private Color[] myColor;

	public void Initiate(){
		hcContainer = new GameObject("HC-Cointainer");
		dummyGameObject = new GameObject("DummyGameObject");
		listHC = new List<GameObject>();
		listMC = new List<GameObject>();
		listNI = new List<GameObject>();
        listNE = new List<GameObject>();
        listBB = new List<GameObject>();
        listN = new GameObject[DataReader.n_neurons+3]; //it was 6146 before i called it from datareader. It breaks so I have to add by 2.
		colliderRadius = myMC.GetComponent<CapsuleCollider>().radius; //get radius of radius
    	scale = myMC.transform.localScale*4;
    	mcRadius = colliderRadius * scale.x * 1.2f;//la till 1.5 för att
		test = new int[DataReader.n_HC];
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
    public void StartHC()
    {
        foreach(var hc in listHC)
        {
            hc.GetComponent<HC>().testingMe();
        }
    }

	//creats a list of rainbow gradient colors for Minicolumns
	void CreateColor(){
		Color firstColor= Color.HSVToRGB(0,1,0.5f);
		Color lastColor = Color.HSVToRGB(0.77f,1,0.5f);
		float myFloat = (float)1f/(DataReader.n_MC_per_HC+3);
		myColor[0] = firstColor;
		myColor[DataReader.n_MC_per_HC-1]= lastColor;
		for(int i=1;i<DataReader.n_MC_per_HC-1;i++){
			myColor[i] = Color.HSVToRGB(((float)i*myFloat),1,0.5f);
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

	//Creates MCs, Inhibs and Exhibs
 	public void Create () {
		myColor = new Color[DataReader.n_MC_per_HC];
	 	CreateColor();
		float theta = 2*Mathf.PI/DataReader.n_MC_per_HC; //n_MC_per_HC

		//Debug.Log(DataReader.n_HC);
		//creates Hypercolumns (minicolumns in a circle.)
		for (int i=0; i<DataReader.n_HC; i++){
			int modulo = (i % 4);
			a=test[i];
			b= 7 * modulo;
			GameObject newHC= (GameObject) Instantiate (myHC, new Vector3 (a, 0, b), Quaternion.identity);
			GameObject bigBasket = (GameObject) Instantiate (myBigBasket, new Vector3 (a, 2, b), Quaternion.identity);
			//listBB.Add(bigBasket);
			newHC.name = "Hypercolumn " + idHC;
			newHC.GetComponent<HC>().ID=idHC;
            newHC.transform.parent = hcContainer.transform;
            bigBasket.transform.parent = hcContainer.transform;

            Color whitecolor = Color.white;
            whitecolor.a = 0.05f;
            listHC.Add(newHC);
            listBB.Add(bigBasket);
            for (int j=0; j<DataReader.n_MC_per_HC; j++){
				GameObject tempMC = (GameObject)Instantiate (myMC, new Vector3 (a + mcRadius * Mathf.Cos (theta*j), 2, b + mcRadius * Mathf.Sin (theta*j)), Quaternion.identity);
				tempMC.name = "Minicolumn " + idMC;
				tempMC.transform.parent= listHC[i].transform;
				tempMC.GetComponent<MC>().ID = idMC;
				tempMC.GetComponent<MC>().COLOR = myColor[j];
                tempMC.GetComponent<Renderer>().material.color = whitecolor;
				listMC.Add(tempMC);
				idMC++;
			}
			idHC++;
        }
		//create Pyramid
		for(int j=0; j<DataReader.n_MC; j++){
			List<int> eRecMC = DataReader.e_rec_MC[j];
			for(int k=0; k<eRecMC.Count(); k++){//30 ska vara antalet neuroner (exhib) per mc
				//här skapar neuroner per MC
				//tempID = eRecMC[k]-1;
				GameObject tempPyramid = (GameObject)Instantiate (myPyramid, new Vector3 (listMC[j].transform.position.x+(Random.Range(-0.32f,0.32f)),Random.Range(0.2f,3.7f),listMC[j].transform.position.z+(Random.Range(-0.32f,0.32f))), Quaternion.identity);
				tempPyramid.name = "Pyramidcell " + eRecMC[k]; //excitatory
				tempPyramid.transform.parent = listMC[j].transform;
				tempPyramid.transform.parent.transform.parent.GetComponent<HC>().myExcitatory.Add(eRecMC[k]);
				tempPyramid.GetComponent<Excitatory>().id = eRecMC[k];
				tempPyramid.GetComponent<Excitatory>().COLOR = tempPyramid.transform.parent.GetComponent<MC>().COLOR;

				listN[eRecMC[k]] = tempPyramid;
				listNE.Add(tempPyramid);
			}
		}
		//create basket
		for(int i=0; i<DataReader.n_HC; i++){
			List<int> iPopHC = DataReader.i_pop_HC[i];
			for( int k=0; k<iPopHC.Count(); k++){
				//tempID = iPopHC[k]-1;
				GameObject tempBasket = (GameObject)Instantiate (myBasket, new Vector3 (listHC[i].transform.position.x+(Random.Range(-1f,1f)),Random.Range(1.0f,3f), listHC[i].transform.position.z+(Random.Range(-1f,1f))), Quaternion.identity);
				tempBasket.name = "Basketcell " +iPopHC[k]; //inhibitory
				tempBasket.transform.parent = listHC[i].transform;
				tempBasket.transform.parent.GetComponent<HC>().myInhibitory.Add(iPopHC[k]);
				tempBasket.GetComponent<Inhibitory>().id = iPopHC[k];
				tempBasket.GetComponent<Inhibitory>().COLOR = Color.blue;
				//tempBasket.GetComponent<Renderer>().enabled = false;
				//print(iPopHC[k] + " yo wtf "+listN[iPopHC[k]-1]);
				listN[iPopHC[k]] = tempBasket;
				listNI.Add(tempBasket);
			}
		}
		hcContainer.transform.localPosition = new Vector3(-7.5f,0,-7.5f);
		hcContainer.transform.parent = dummyGameObject.transform;
        dummyGameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        pyramidScale = listNE[0].transform.localScale;
        basketScale = listNI[0].transform.localScale;

        print("initialization done");
	}
    public static void UnactivateAll()
    {
        foreach (GameObject excitatory in listNE)
        { //all neurons
            excitatory.GetComponent<Excitatory>().IsActive = false;
            excitatory.GetComponent<Excitatory>().latestActivationTime = 0;

            excitatory.GetComponent<Renderer>().material.color = Color.white;
        }
        foreach (GameObject inhibitory in listNI)
        { //all neurons

            inhibitory.GetComponent<Inhibitory>().IsActive = false;
            inhibitory.GetComponent<Inhibitory>().latestActivationTime = 0;

            inhibitory.GetComponent<Renderer>().material.color = Color.white;
        }
    }
    public void ToggleMC(bool BOOLEAN){
		foreach (GameObject mc in listMC)
		{
			mc.GetComponent<Renderer>().enabled = BOOLEAN;
		}
    }
	public void ToggleExhibitory(bool BOOLEAN){
		foreach(GameObject exhib in listNE){ //all Pyramid
          	exhib.GetComponent<Renderer>().enabled= BOOLEAN;
        }
    }
	public void ToggleInhibitory(bool BOOLEAN){
		foreach(GameObject inhib  in listNI){ //all Basket
          	inhib.GetComponent<Renderer>().enabled= BOOLEAN;
        }
    }

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
//change to vertical view
		dummyGameObject.transform.rotation = Quaternion.Euler(90,0,0);
		vView = true;
		hView=false;
	}	
	public void HorizontalView(){
//change to vertical view
		dummyGameObject.transform.rotation = Quaternion.Euler(0,0,0);
		vView = false;
		hView=true;
	}


	public void blabla(int NeuronID){
		for (int i=0; i<DataReader.n_HC;i++){
			if(i!=NeuronID){
				for (int j=0; j<DataReader.n_MC_per_HC;j++){
					//listHC[i].GetChild(j).GetComponent<GameObject>().GetComponent<Renderer>().enabled = false;
				}
			}
		}
		for (int j=0; j<DataReader.n_MC_per_HC;j++){
			//listHC[NeuronID].GetChild(j).GetComponent<Renderer>().enabled = true;
		}
	}
}
