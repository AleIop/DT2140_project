using UnityEngine;


/*
	this class is for controlling the camera position and movements with the keyboard only
*/
public class MoveCamera : MonoBehaviour {

	Vector3 topviewPosition = new Vector3(0f,24.56f,0f); //camera position from above the scene
	Vector3 initPosition; //initial camera position
 
	private float speed = 5.0f; //movement speed when pressing the keys
	
	//these are probably meant to limit the movement range for some reason... mah
	public float minX = -360.0f;
	public float maxX = 360.0f;	
	public float minY = -45.0f;
	public float maxY = 45.0f;
	
	//sensitivity of X and Y axis when rotating the camera with the mouse
	public float sensX = 1.0f;
	public float sensY = 1.0f;
	
	//initial rotation values when controlling the camera with the mouse, which get incremented while holding the C key 
	float rotationY = 0.0f;
	float rotationX = 0.0f;

	void Awake() {
		initPosition = transform.position;
	}
	void Update () {
	
		if (Input.GetKey(KeyCode.D)){
			//transform.position += Vector3.right * speed * Time.deltaTime;
			transform.Translate(Vector3.right * Time.deltaTime * speed, Space.Self); //moves right when pressing D

		}

		if (Input.GetKey(KeyCode.A)){
			//transform.position += Vector3.left * speed * Time.deltaTime;
			transform.Translate(Vector3.left * Time.deltaTime * speed, Space.Self); //moves left when pressing A
		}

		if (Input.GetKey(KeyCode.Q)){
			transform.position -= Vector3.up * speed * Time.deltaTime; //moves down when pressing Q
		}

		if (Input.GetKey(KeyCode.E)){
			transform.position += Vector3.up * speed * Time.deltaTime; //moves up when pressing E
		}

		if (Input.GetKey(KeyCode.W)){
			//transform.position += Vector3.forward * speed * Time.deltaTime;
			transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self); //moves forward when pressing W
		}

		if (Input.GetKey(KeyCode.S)){
			//transform.position += Vector3.back * speed * Time.deltaTime;
			transform.Translate(Vector3.back * Time.deltaTime * speed, Space.Self); //moves backward when pressing S
		}
		
		//top down view
		if (Input.GetKey(KeyCode.T)){
			transform.position = topviewPosition;
			transform.rotation= Quaternion.Euler(new Vector3(90,0,0));
		}
		//ground view
		if (Input.GetKey(KeyCode.G)){
			transform.position = initPosition;
			transform.rotation= Quaternion.Euler(new Vector3(30,0,0));
		}

		//hold C to rotate camera with the mouse
		if (Input.GetKey(KeyCode.C)) {
			rotationY += Input.GetAxis("Mouse Y")*sensY*Time.deltaTime;
			rotationX += Input.GetAxis("Mouse X")*sensX*Time.deltaTime;
			transform.localEulerAngles = new Vector3(-rotationY,rotationX, 0);
		}

	}
}
 