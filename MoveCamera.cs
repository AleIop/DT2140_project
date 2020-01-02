using UnityEngine;
 
public class MoveCamera : MonoBehaviour {

	Vector3 topviewPosition = new Vector3(0f,24.56f,0f);
	Vector3 initPosition;
 
	private float speed = 5.0f;
	public float minX = -360.0f;
	public float maxX = 360.0f;
		
	public float minY = -45.0f;
	public float maxY = 45.0f;
	
	public float sensX = 1.0f;
	public float sensY = 1.0f;
		
	float rotationY = 0.0f;
	float rotationX = 0.0f;

	void Awake()
	{
		initPosition = transform.position;
	}
	void Update () {
	
		if (Input.GetKey(KeyCode.D)){
			//transform.position += Vector3.right * speed * Time.deltaTime;
			transform.Translate(Vector3.right * Time.deltaTime * speed, Space.Self); //RIGHT

		}
		if (Input.GetKey(KeyCode.A)){
			//transform.position += Vector3.left * speed * Time.deltaTime;
			transform.Translate(Vector3.left * Time.deltaTime * speed, Space.Self); //LEFT
		}
		if (Input.GetKey(KeyCode.Q)){
			transform.position -= Vector3.up * speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.E)){
			transform.position += Vector3.up * speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.W)){
			//transform.position += Vector3.forward * speed * Time.deltaTime;
			transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self); //FORWARD
		}

		if (Input.GetKey(KeyCode.S)){
			//transform.position += Vector3.back * speed * Time.deltaTime;
			transform.Translate(Vector3.back * Time.deltaTime * speed, Space.Self); //BACK
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

		//hold this in to rotate camera
		if (Input.GetKey (KeyCode.C)) {
			rotationY += Input.GetAxis ("Mouse Y") * sensY * Time.deltaTime;
						rotationX += Input.GetAxis ("Mouse X") * sensX * Time.deltaTime;
			transform.localEulerAngles = new Vector3 (-rotationY,rotationX, 0);
		}

	}
}
 