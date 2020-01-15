using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
using System.Linq;

//This class is where all the UI actions are performed, also the real time animation
public class UIActions : MonoBehaviour
{
	public Slider speedSlider;
	public Slider timeSlider;
	//public Slider timeSlider2;

	public float startTime=0f;
	public float speed;
	private float maxSpeed = 1f;
	private float minSpeed = 0.001f;
    //	private float offset=100f;//old stuff
    public Text myText;

    public Text myTextVR;

    public Text phase;
    public Text phaseVR;

    public Dropdown dropdown;

    public Text timeText;
    public Text speedText;
    public Queue<int> deactivationQueue;
    public Queue<int> QueueBasket;

    public Queue<int> activeExitatory;

    int currentNeuron;
	//public Image myFill;
	//public Image myHandle;

	private List<int> tempListInh;
    private List<int> tempListExc;
    private List<int> tempListExc2;

    private List<int> tempUnactivateListExc;
    private List<int> nextTempListExc;
    private List<int> tempListActiveMC;
	private float runningTime;
	private List<GameObject> activatedNeurons;

	private float fps;
	private int nextPhaseIndex;
	private int currentPhaseIndex;
    public float myDeltaTime;

	public static bool isPlaying=false;
    int prevTime;
    //private Queue<int> deactivation= new Queue<int>();
    //private Dictionary<int, List<int>> unactivatePhase;

    GameObject graph;
    GameObject pointer;
    float endGraph, initPointer, distance;

    public GameObject cameraRig;

    //property associated to the variable speed
    public float Speed {
		get {
            return speed;
        }
		set {
            speed = value;
        }
	}

    public float animationTime
    {
        get { return timeSlider.value; }
    }
    private Color excitatoryColor;


    // Use this for initialization
    public void Initiate(){
		timeSlider.minValue = 0f; //lower bound of the slider
                                  //timeSlider2.minValue = 0f;
                                  //timeSlider.maxValue = 44450.1f;


        //index and time of the last excitatory spike
		int lastIndex = DataReader.e_newTimes.Count();
		float lastTime = DataReader.e_newTimes[lastIndex-1];

		timeSlider.maxValue = lastTime; //upper bound of the slider
		//timeSlider2.maxValue = lastTime;

		timeSlider.value=0f; //initial value of the slider
		//timeSlider2.value=0f;

        //lower bound, upper bound and initial speed of the slider
		speedSlider.minValue = minSpeed;
		speedSlider.maxValue = maxSpeed;
		speedSlider.value = minSpeed;
        
        PopulateList();
        deactivationQueue = new Queue<int>();
        QueueBasket = new Queue<int>();
        activeExitatory = new Queue<int>();
        //unactivatePhase = new Dictionary<int, List<int>>();

        graph = GameObject.Find("Graph");
        pointer = GameObject.Find("Pointer");
        initPointer = pointer.transform.position.x;
        endGraph = graph.GetComponent<RectTransform>().rect.width;
        distance = endGraph - initPointer;

    }

    //this method adds the phase labels to the dropdown menu on the VR controllers 
    public void PopulateList() {
        dropdown.AddOptions(DataReader.myNames);
        speed = speed*0.003f; 
    }

    //Updates and check the FPS so timeSlider adapts to it, and plays animation in real time
	void Update() {		
		if(isPlaying) {
			if(startTime==0f) { //at the beginning, sets current time as the initial one
				startTime = Time.time;
			}
			fps = 1/Time.deltaTime;
            /* 
			runningTime += Time.deltaTime;
			print("runtime" + runningTime); //ran for 1 minute
			*/
            myDeltaTime = (1000/fps)*speed; //increment for the slider value, which determines its physical translation in space

            timeSlider.value += myDeltaTime; //1/fps, this makes the animation run in real time if speed is 1. 1000 is in [ms]
            //timeSlider2.value = timeSlider.value;

            
            
            speedText.text = "Speed: " + speed + " X";
			//timeText.text = timeSlider.value.ToString("0 ms");
            //speedText.text = speedSlider.value.ToString("0.001");

            if(isPlaying) pointer.transform.Translate(speed*0.00033549f,0,0);

        }

        // Hide and show 2D graph
        /*
        if (Input.GetKeyDown(KeyCode.K))c{
			timeSlider2.gameObject.SetActive(false);
		}
		if(Input.GetKeyDown(KeyCode.L))c{
			timeSlider2.gameObject.SetActive(true);
		}
        if (!isPlaying)
        {
            CreateNeurons.UnactivateAll();
        }
        */
        //timeSlider.OnDrag(bruv());
    }
   

    /*
        this method is supposed to progressively deactivate pyramid cells while playing an animation
    */
    public void ExcitatoryDeactivation(float currentTime) {
        /* 
        var selectedValues = DataReader.phase.Where(x => (x.Key < Mathf.RoundToInt(currentTime) -myDeltaTime) && (x.Key > Mathf.RoundToInt(currentTime) - (myDeltaTime*2))).Select(x => x.Key);
        
        foreach (var timeStamp in selectedValues) {
            //deactivationQueue.Enqueue(timeStamp);
            tempListExc = DataReader.phase[timeStamp];
            
            foreach (var index in tempListExc) {
                int n = DataReader.e_src[index];
                
                //if (Mathf.RoundToInt(currentTime) - CreateNeurons.listN[n].GetComponent<Excitatory>().PreviousTime > 15 && !CreateNeurons.listN[n].GetComponent<Excitatory>().IsActive)
                
                //if (!CreateNeurons.listN[n].GetComponent<Excitatory>().IsActive && timeStamp >CreateNeurons.listN[n].GetComponent<Excitatory>().latestActivationTime) {
                    CreateNeurons.listN[n].GetComponent<Excitatory>().IsActive = false;
                    CreateNeurons.listN[n].GetComponent<Excitatory>().latestActivationTime = timeStamp;
                    StartCoroutine(CreateNeurons.listN[n].GetComponent<PlayAnimation>().DeactivateExc(speed * 0.01f));
                }
            }
        }
        */

        if (deactivationQueue.Count > 0) {
            var timeIndex = deactivationQueue.Peek();
            
            if (timeIndex < (Mathf.RoundToInt(currentTime) - 15)) {
                deactivationQueue.Dequeue();
                tempListExc = DataReader.phase[timeIndex];
                foreach (var index in tempListExc)
                {
                    int n = DataReader.e_src[index];
                    //if (Mathf.RoundToInt(currentTime) - CreateNeurons.listN[n].GetComponent<Excitatory>().PreviousTime > 15 && !CreateNeurons.listN[n].GetComponent<Excitatory>().IsActive)
                    if (CreateNeurons.listN[n].GetComponent<Excitatory>().IsActive)
                    {
                        StartCoroutine(CreateNeurons.listN[n].GetComponent<PlayAnimation>().DeactivateExc(speed));
                        CreateNeurons.listN[n].GetComponent<Excitatory>().IsActive = false;
                        GameObject tempParent = CreateNeurons.listN[n].GetComponent<Excitatory>().transform.parent.gameObject;

                        /*
                        GameObject farfar = tempParent.transform.parent.gameObject;
                        if (Vector3.Distance(new Vector3(0,0,0), cameraRig.transform.position) > 20) {
                            tempParent.GetComponent<MC>().GetComponent<AudioSource>().enabled = true;
                            farfar.GetComponent<HC>().IsActive = false;
                        }
                        */ 

                        tempParent.GetComponent<MC>().IsActive = false;
                    }
                }
            }

        }
    }


    /*
        this method is supposed to check current time and activate the pyramid cells that correspond to it
    */
    public void ExcitatoryActivation(float currentTime)
    {

        //Excitatory
        if (DataReader.phase.ContainsKey(Mathf.RoundToInt(currentTime) + 1) && prevTime != Mathf.RoundToInt(currentTime) + 1)
        {
            deactivationQueue.Enqueue((Mathf.RoundToInt(currentTime) + 1));
            tempListExc = DataReader.phase[Mathf.RoundToInt(currentTime) + 1];

            foreach (var index in tempListExc) {
                int n = DataReader.e_src[index];
                if ((Mathf.RoundToInt(currentTime) + 1) - CreateNeurons.listN[n].GetComponent<Excitatory>().PreviousTime > 15 && !CreateNeurons.listN[n].GetComponent<Excitatory>().IsActive) {
                    CreateNeurons.listN[n].GetComponent<Excitatory>().IsActive = true;
                    GameObject tempParent = CreateNeurons.listN[n].GetComponent<Excitatory>().transform.parent.gameObject;

                    /*
                    GameObject farfar = tempParent.transform.parent.gameObject;
                    if (Vector3.Distance(new Vector3(0, 0, 0), cameraRig.transform.position) > 20)
                    {
                        tempParent.GetComponent<MC>().GetComponent<AudioSource>().enabled = false;
                        farfar.GetComponent<HC>().IsActive = true;
                    }
                    */

                    tempParent.GetComponent<MC>().IsActive = true;
                    CreateNeurons.listN[n].GetComponent<Excitatory>().latestActivationTime = (Mathf.RoundToInt(currentTime) + 1);
                    StartCoroutine(CreateNeurons.listN[n].GetComponent<PlayAnimation>().ActivateExc(speed));
                }
            }
            
            /*
            var selectedValues = DataReader.phase.Where(x => (x.Key < Mathf.RoundToInt(currentTime)+1) && (x.Key > Mathf.RoundToInt(currentTime) - myDeltaTime)).Select(x => x.Key);
            foreach(var timeStamp in selectedValues) { 
                //deactivationQueue.Enqueue(timeStamp);
                tempListExc = DataReader.phase[timeStamp];
                foreach (var index in tempListExc) {
                    int n = DataReader.e_src[index];
                    //if (Mathf.RoundToInt(currentTime) - CreateNeurons.listN[n].GetComponent<Excitatory>().PreviousTime > 15 && !CreateNeurons.listN[n].GetComponent<Excitatory>().IsActive)
                    //if (!CreateNeurons.listN[n].GetComponent<Excitatory>().IsActive && timeStamp >CreateNeurons.listN[n].GetComponent<Excitatory>().latestActivationTime)
                    //{
                        CreateNeurons.listN[n].GetComponent<Excitatory>().IsActive = true;
                        CreateNeurons.listN[n].GetComponent<Excitatory>().latestActivationTime = timeStamp;
                        StartCoroutine(CreateNeurons.listN[n].GetComponent<PlayAnimation>().ActivateExc(speed*0.01f));
                    //}
                }

            }
            */
        }
    }

    /*
        this method is supposed to check current time and activate the basket cells that correspond to it
    */
    public void ActivationBasket(float currentTime) {
        //Inhibitory
        if (DataReader.basketphase.ContainsKey(Mathf.RoundToInt(currentTime)+1) && prevTime != Mathf.RoundToInt(currentTime)+1) {
            QueueBasket.Enqueue((Mathf.RoundToInt(currentTime) + 1)); //enqueues successive basket cell phase value
            tempListInh = DataReader.basketphase[Mathf.RoundToInt(currentTime)+1]; //values corresponding to a specific phase

            //for every value, selects the related basket cell and, if the cell is inactive and its latest activation time is lesser than the current one - 15, updates those values and plays activation animation
            foreach (var index in tempListInh) {
                int n = DataReader.i_src[index];
                //if (Mathf.RoundToInt(currentTime) - CreateNeurons.listN[n].GetComponent<Inhibitory>().PreviousTime > 15 && !CreateNeurons.listN[n].GetComponent<Inhibitory>().IsActive)
                if (!CreateNeurons.listN[n].GetComponent<Inhibitory>().IsActive && Mathf.RoundToInt(currentTime)-15> CreateNeurons.listN[n].GetComponent<Inhibitory>().latestActivationTime) {
                    CreateNeurons.listN[n].GetComponent<Inhibitory>().IsActive = true;
                    CreateNeurons.listN[n].GetComponent<Inhibitory>().latestActivationTime = Mathf.RoundToInt(currentTime);
                    StartCoroutine(CreateNeurons.listN[n].GetComponent<PlayAnimation>().ActivateInh(speed));
                    //CreateNeurons.listN[n].GetComponent<Inhibitory>().PreviousTime = currentTime;
                }
            }
        }
        prevTime = Mathf.RoundToInt(currentTime); //updated the current time value
    }

    /*
        this method is supposed to progressively deactivate neurons while playing an animation
    */
    public void DeactivationBasket(float currentTime) {
        /* 
        var selectedValues = DataReader.basketDeactivation.Where(x => (x.Key < Mathf.RoundToInt(currentTime)) && (x.Key > Mathf.RoundToInt(currentTime-50))).Select(x => x.Value);

        foreach (var list in selectedValues) {
            foreach (var index in list) {
                int n = DataReader.i_src[index];
                if (CreateNeurons.listN[n].GetComponent<Inhibitory>().IsActive) {
                    StartCoroutine(CreateNeurons.listN[n].GetComponent<PlayAnimation>().DeactivateInh(speed * 0.01f));
                    CreateNeurons.listN[n].GetComponent<Inhibitory>().IsActive = false;
                }
            }
        }
        */
        
        if (QueueBasket.Count != 0) {
            //foreach(var timeIndex in deactivationQueue) { 
            var timeIndex = QueueBasket.Peek();

            if (timeIndex < (Mathf.RoundToInt(currentTime) - 15)) {
                QueueBasket.Dequeue();
                tempListInh = DataReader.basketphase[timeIndex];
                foreach (var index in tempListInh) {
                    int n = DataReader.i_src[index];
                    //if (Mathf.RoundToInt(currentTime) - CreateNeurons.listN[n].GetComponent<Excitatory>().PreviousTime > 15 && !CreateNeurons.listN[n].GetComponent<Excitatory>().IsActive)
                    if (CreateNeurons.listN[n].GetComponent<Inhibitory>().IsActive) {
                        StartCoroutine(CreateNeurons.listN[n].GetComponent<PlayAnimation>().DeactivateInh(speed));
                        CreateNeurons.listN[n].GetComponent<Inhibitory>().IsActive = false;
                    }
                }
            }
        }
    }


    /*
        this method pauses the activation animation and updates the slider's value to the current timestamp
    */
    public void Dropdown_ChangeIndex(int index) {
        PlayStop();
        float phaseStart = DataReader.myStart[index];
        timeSlider.value = phaseStart;
    }

    /*
        this method identifies the current and the next phase indexes, changes the label accordingly and checks the animation upper bound
    */
    public void PhaseLabel(float currentTime) {
        nextPhaseIndex = DataReader.myStart.FindIndex(b => (b >= currentTime)); //finds index of the very next phase
        //print("next time index" + nextPhaseIndex);

        //if the current time is lesser than the last phase timestamp, the current phase index is set to the one preceding the next one
        if (currentTime < DataReader.myStart[DataReader.myStart.Count() - 1]) {
            currentPhaseIndex = DataReader.myStart.FindIndex(b => (b < DataReader.myStart[nextPhaseIndex] && b >= DataReader.myStart[nextPhaseIndex - 1]));
            //print("current time index " + currentPhaseIndex);
        } else {
            currentPhaseIndex = (DataReader.myStart.Count() - 1);
        }
        
        phase.text = "Phase: " + DataReader.myNames[currentPhaseIndex];
        phaseVR.text = phase.text;
        
        //If the slider value reaches the max value, then the animation will stop
        if (currentTime >= timeSlider.maxValue && isPlaying) {
            PlayStop();
        }
    }

    /*
        these two methods are supposed to be resetting all the activation queues and stop the animation
    */
    public void bruv() {
        Invoke("Callmemaybe", 0.1f); //waits 100ms before invoking Callmemaybe()
    }
    public void Callmemaybe() {
        deactivationQueue.Clear();
        QueueBasket.Clear();
        PlayStop();
        CreateNeurons.UnactivateAll();
    }

    public void PlayStop() {
		isPlaying=false;
		myText.text = "Status: Paused";
        myTextVR.text = "►";
        print("visualization is paused");
    }

    /*
    this 2 methods check whether the activation animation is playing or not
    */
    public static void PlayNow() {
		if(isPlaying) {
			isPlaying=false;
			//myText.text = "►";
			print("visualization is paused");
            //Invoke("bruv", 0.15f);
        } else {
			isPlaying = true;
			//myText.text="■";
			print("visualization is playing");
		}
	}
    public void PlayNow2() {
        if (isPlaying) {
            isPlaying = false;
            myText.text = "Status: Paused";
            myTextVR.text = "►";
            print("visualization is paused");
            //Invoke("bruv", 0.15f);
        } else {
            isPlaying = true;
            myText.text="Status: Running";
            myTextVR.text = "■";
            print("visualization is playing");
        }
    }

    //change speed of animation
    public void SpeedFunction(float sliderSpeed){
        if (sliderSpeed <= maxSpeed && sliderSpeed >= minSpeed) {
            speed = (Mathf.Pow(10, sliderSpeed));
        } 
	}
}