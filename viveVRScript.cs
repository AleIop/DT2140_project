using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class viveVRScript : MonoBehaviour {

    
    public GameObject scriptContainer;
    public GameObject camera;
    public GameObject cameraRig;
    public GameObject rightCanvas;
    public GameObject rightCanvasTrigger;
    [SteamVR_DefaultAction("PlayPause")]
    public SteamVR_Action_Boolean playPause;
    public SteamVR_Action_Vector2 touchpadPositionLeft;
    public SteamVR_Action_Boolean triggerPress;
    public SteamVR_Action_Vector2 touchpadPositionRight;



    bool rightTriggerBool;
    public GameObject rController;

    void Start() {
        rightCanvas.SetActive(true);
        rightCanvasTrigger.SetActive(false);
    }

    // Update is called once per frame
    void Update () {

        //pauses or resumes animation whenever the play/pause button on the left controller touchpad is pressed
        Vector2 touchpadCordLeft = touchpadPositionLeft.GetAxis(SteamVR_Input_Sources.Any);
        if (SteamVR_Input._default.inActions.PlayPause.GetStateUp(SteamVR_Input_Sources.Any) && Mathf.Abs(touchpadCordLeft.x)<0.4f && Mathf.Abs(touchpadCordLeft.y)<0.4f) {
            //UIActions.PlayNow();
            scriptContainer.GetComponent<UIActions>().PlayNow2();
        }

        /*
        if (SteamVR_Input._default.inActions.PlayPause.GetStateUp(SteamVR_Input_Sources.Any) && Mathf.Abs(touchpadCordLeft.y) >0.6f)
        {
            UIActions.speed
        }
        */
        
        /*
        if(Mathf.Abs(touchpadCordLeft.y) > 0.7f)
        {
            if (scriptContainer.GetComponent<UIActions>().speedSlider.value <= scriptContainer.GetComponent<UIActions>().speedSlider.maxValue && scriptContainer.GetComponent<UIActions>().speedSlider.value >= scriptContainer.GetComponent<UIActions>().speedSlider.minValue)
            {
                scriptContainer.GetComponent<UIActions>().speed = scriptContainer.GetComponent<UIActions>().speedSlider.value + touchpadCordLeft.y;
               
            }
        }
        */

        Vector2 touchCordRight = touchpadPositionRight.GetAxis(SteamVR_Input_Sources.Any);

        //zoom in and slightly translates the environment
        if (Mathf.Abs(touchCordRight.x) < 0.7 && !rightTriggerBool) {
            scriptContainer.GetComponent<CreateNeurons>().dummyGameObject.transform.localScale += new Vector3(touchCordRight.y * 0.01f, touchCordRight.y * 0.01f, touchCordRight.y * 0.01f);
        }
        if (Mathf.Abs(touchCordRight.y) < 0.7 && !rightTriggerBool) { scriptContainer.GetComponent<CreateNeurons>().dummyGameObject.transform.localPosition += new Vector3(0, touchCordRight.x * 0.01f, 0);
        }
        
        //maps the right touchpad coordinates onto the environment coordinates by moving it around, while locking the vertical movement
        rightTriggerBool = triggerPress.GetState(SteamVR_Input_Sources.Any);
        if (rightTriggerBool) {
            rightCanvas.SetActive(false);
            rightCanvasTrigger.SetActive(true);
            //scriptContainer.GetComponent<CreateNeurons>().dummyGameObject.transform.localPosition += new Vector3(touchpadPosition.y * 0.01f, 0, -touchpadPosition.x * 0.01f);
            Vector3 yolo = -camera.transform.forward * -touchCordRight.y * 0.01f;
            yolo += camera.transform.right * touchCordRight.x * 0.01f;
            yolo.y *= 0;
            scriptContainer.GetComponent<CreateNeurons>().dummyGameObject.transform.localPosition += yolo;
        }
        if (!rightTriggerBool) {
            rightCanvas.SetActive(true);
            rightCanvasTrigger.SetActive(false);
        }
    }
}
