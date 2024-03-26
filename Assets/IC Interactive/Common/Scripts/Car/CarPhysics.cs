using UnityEngine;
using System.Collections;

public class CarPhysics : MonoBehaviour {

	public Transform visual;
	private bool engineOn;
	[HideInInspector] public CameraController cameraController;

	//Floating start
	public LayerMask mask;
	private const float waterDensity = 1000;
	public float density = 500;
	private Vector3 localArchimedesForce;

	public float forceMultiplyer;
	private float rotateSmoothness = 1f;
	public float levitationHeight;
	private bool isMove;
	private Vector3 horizontalForward;
	//Floating finish

	public float fatalSpeed;
	public bool floatingCar;

	public Vector3 jumpForce;

	public CarControllerAlt carControllerAlt;

	public GameObject headlights;

	void Awake () {
		float volume = GetComponent<Rigidbody>().mass / density;
		float archimedesForceMagnitude = waterDensity * Mathf.Abs(Physics.gravity.y) * volume;
		localArchimedesForce = new Vector3(0, archimedesForceMagnitude, 0);
		engineOn = false;
		isMove = false;
		horizontalForward = new Vector3 ();
	}

	
	// Update is called once per frame
	void Update () {
		if (visual) {
			visual.position = transform.position;
			visual.rotation = transform.rotation;
		}

		if (engineOn) {
			if (Input.GetKeyDown (KeyCode.F)) {
				floatingCar = !floatingCar;
			}
			if (Input.GetKeyDown (KeyCode.Space)) {
				GetComponent<Rigidbody>().AddForce(jumpForce);
			}
		}

		//Floating
		if (engineOn && floatingCar) {
			RaycastHit hit;
			if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, mask)) {
				if (hit.distance < levitationHeight) {

					float k = ((levitationHeight - hit.distance)/(levitationHeight));
					if (k > 1) {
						k = 1f;
					}
					else if (k < 0) {
						k = 0f;
					}
					
					var localDampingForce = -GetComponent<Rigidbody>().velocity * GetComponent<Rigidbody>().mass;
					var force = localDampingForce + Mathf.Sqrt(k) * localArchimedesForce;
					GetComponent<Rigidbody>().AddForce(force);
				}
			}

			if (Input.GetKey (KeyCode.W) ||
			    Input.GetKey (KeyCode.A) ||
			    Input.GetKey (KeyCode.D) ||
			    Input.GetKey (KeyCode.S) ||
			    Input.GetKey (KeyCode.LeftShift) ||
			    Input.GetKey (KeyCode.LeftControl)) {
				isMove = true;
				if (Input.GetKey (KeyCode.W)) GetComponent<Rigidbody>().AddRelativeForce (Vector3.forward * forceMultiplyer);
				if (Input.GetKey (KeyCode.A)) GetComponent<Rigidbody>().AddRelativeForce (Vector3.left * forceMultiplyer);
				if (Input.GetKey (KeyCode.D)) GetComponent<Rigidbody>().AddRelativeForce (Vector3.right * forceMultiplyer);
				if (Input.GetKey (KeyCode.S)) GetComponent<Rigidbody>().AddRelativeForce (Vector3.back * forceMultiplyer);
				if (Input.GetKey (KeyCode.LeftShift)) levitationHeight += 0.1f;
				if (Input.GetKey (KeyCode.LeftControl)) levitationHeight -= 0.1f;
				if (levitationHeight < 0f) levitationHeight = 0f;
			}
			else isMove = false;

			if (isMove) {
				horizontalForward = cameraController.transform.forward;
				horizontalForward.y = 0f;
			}
			rotateSmoothness = (((Vector3.Dot (transform.forward.normalized, horizontalForward.normalized)) + 1.5f)/2.5f);
			transform.forward = Vector3.Lerp (transform.forward, horizontalForward, Time.deltaTime * 3f/rotateSmoothness);
		}
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.collider.gameObject.GetComponent<CharacterInteraction>()) {
			if (GetComponent<Rigidbody>().velocity.magnitude > fatalSpeed) {
				collision.collider.gameObject.GetComponent<CharacterInteraction>().character.ragdoll.killTrigger = true;
			}
		}
	}

	public void StartEngine () {
		engineOn = true;
		carControllerAlt.canControl = true;
		carControllerAlt.GetComponent<SoundController>().StartEngine ();
		headlights.SetActive (true);
		//Put you code here
	}

	public void OffEngine () {
		engineOn = false;
		carControllerAlt.canControl = false;
		carControllerAlt.GetComponent<SoundController>().StopEngine ();
		headlights.SetActive (false);
		//Put you code here
	}
}
