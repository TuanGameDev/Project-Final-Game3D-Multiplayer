using UnityEngine;
using System.Collections;

// 3rd person game-like camera controller
// keeps camera behind the player and aimed at aiming point
public class CameraController : MonoBehaviour {
	
	private Transform orbitCenter;
	public CameraOrbitCenterInfo currentOrbitCenter;
	[HideInInspector] public Transform cameraTarget;

	private Vector3 camOffset = new Vector3 (0.0f, 0.0f, -2.4f);
	public Vector3 closeOffset = new Vector3 (0.5f, 0.0f, 0.0f);
	public Vector3 zoomOffset = new Vector3 (0.5f, 0.3f, -2.4f);
	public Vector3 normalOffset = new Vector3 (0.0f, 0.0f, -2.4f);

	private Vector3 NEWcloseOffset;
	private Vector3 NEWzoomOffset;
	private Vector3 NEWnormalOffset;
	
	public float horizontalAimingSpeed = 270f;
	public float verticalAimingSpeed = 270f;
	public float maxVerticalAngle = 89f;
	public float minVerticalAngle = -89f;
	
	public float mouseSensitivity = 1f;
	private float sensitiveness = 1f;

	public LayerMask mask;
	
	public float normalFieldOfView = 60f;
	public float zoomFieldOfView = 20f;
	public float zoomSmooth = 8f;
	
	private float angleH = 0;
    private float angleV = 0;
	private Transform cam;
	private float maxCamDist = 1f;

	private Camera cameraComponent;

	[HideInInspector] public bool isControllable = true;

	[HideInInspector] public bool isZoom = false;
	[HideInInspector] public BodyPartAlive aimBodyPart;
	
	// Use this for initialization
	void Awake () {
		GameObject orbitCenterGameObject = new GameObject("Orbit Center");
		orbitCenter = orbitCenterGameObject.transform;
		ChangeOrbitCenter (currentOrbitCenter);

		transform.forward = orbitCenter.forward;
		cam = transform;
		
		maxCamDist = 3f;
		cameraComponent = GetComponent<Camera>();
	}

	// Update is called once per frame
	public void LateUpdate () {
		if (Time.deltaTime == 0f || Time.timeScale == 0f || orbitCenter == null) 
			return;

		if (isControllable) GetInput ();
		
		// Before changing camera, store the prev aiming distance.
		// If we're aiming at nothing (the sky), we'll keep this distance.
		float prevDist = (cameraTarget.position - cam.position).magnitude;
		
		// Set aim rotation
		Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);
		Quaternion camYRotation = Quaternion.Euler(0, angleH, 0);
		
		// Find far and close position for the camera
		Vector3 farCamPoint = orbitCenter.position + aimRotation * camOffset;
		Vector3 closeCamPoint = orbitCenter.position + camYRotation * closeOffset;
		float farDist = Vector3.Distance(farCamPoint, closeCamPoint);
		
		// Smoothly increase maxCamDist up to the distance of farDist
		maxCamDist = Mathf.Lerp(maxCamDist, farDist, 5f * Time.deltaTime);
		
		// Make sure camera doesn't intersect geometry
		// Move camera towards closeOffset if ray back towards camera position intersects something 
		RaycastHit hit;
		Vector3 closeToFarDir = (farCamPoint - closeCamPoint) / farDist;
		float padding = 0.3f;
		if (Physics.Raycast(closeCamPoint, closeToFarDir, out hit, maxCamDist + padding, mask)) {
			maxCamDist = hit.distance - padding;
		}

		cam.position = closeCamPoint + closeToFarDir * maxCamDist;
		cam.rotation = aimRotation;
		
		// Do a raycast from the camera to find the distance to the point we're aiming at.
		float aimTargetDist;
		if (Physics.Raycast(cam.position, cam.forward, out hit, Mathf.Infinity, mask)) {
			aimTargetDist = hit.distance + 0.05f;
			aimBodyPart = hit.collider.GetComponent<BodyPartAlive>();
		}
		else {
			// If we're aiming at nothing, keep prev dist but make it at least 5.
			aimTargetDist = Mathf.Max(5, prevDist);
		}
		
		// Set the aimTarget position according to the distance we found. Make the movement slightly smooth.
		cameraTarget.position = cam.position + cam.forward * aimTargetDist;
		
		sensitiveness = (cameraComponent.fieldOfView / 60f) * mouseSensitivity;

		float currentFieldOfView = isZoom ? zoomFieldOfView : normalFieldOfView;
		Vector3 currentCamOffset = isZoom ? zoomOffset : normalOffset;

		cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, currentFieldOfView, Time.deltaTime * zoomSmooth);
		camOffset = Vector3.Lerp(camOffset, currentCamOffset, Time.deltaTime * zoomSmooth);
	}

	void GetInput () {
		angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1f, 1f) * horizontalAimingSpeed * sensitiveness * Time.deltaTime;
		angleV += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1f, 1f) * verticalAimingSpeed * sensitiveness * Time.deltaTime;

		// limit vertical angle
		angleV = Mathf.Clamp(angleV, minVerticalAngle, maxVerticalAngle);
	}

	public void ChangeOrbitCenter (CameraOrbitCenterInfo newOrbitCenter) {
		orbitCenter.parent = newOrbitCenter.transform;

		NEWcloseOffset = newOrbitCenter.closeOffset;
		NEWzoomOffset = newOrbitCenter.zoomOffset;
		NEWnormalOffset = newOrbitCenter.normalOffset;
		
		StartCoroutine (LerpOrbitCenter (1.5f));
	}

    public void AddTilt(float horizontalTilt, float verticalTilt)
    {
        angleH += horizontalTilt;
        angleV += verticalTilt;
    }

	IEnumerator LerpOrbitCenter (float time) {
		float i = 0.0f;
		float rate = 1.0f/time;
		while (i < 1.0f) {
			i += Time.deltaTime * rate;
			orbitCenter.localPosition = Vector3.Lerp (orbitCenter.localPosition, Vector3.zero, i);
			closeOffset = Vector3.Lerp (closeOffset, NEWcloseOffset, i);
			zoomOffset = Vector3.Lerp (zoomOffset, NEWzoomOffset, i);
			normalOffset = Vector3.Lerp (normalOffset, NEWnormalOffset, i);
			yield return null;
		}
	}
}
