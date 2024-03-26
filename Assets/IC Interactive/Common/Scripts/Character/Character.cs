using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour {
	public Animator animator;
    private AnimatorEvents animatorEvents;
    [HideInInspector]
    public AnimatorMovement animatorMovement;
    [HideInInspector]
    public InputManager input;
    [HideInInspector]
    public CharacterInteraction interaction;
    [HideInInspector]
    public Skill weaponsController;
    [HideInInspector]
    public SkillFirearmsVariables skillFirearmsVariables;
    [HideInInspector]
    public CharacterRagdollController ragdoll;
    [HideInInspector]
    public Inventory inventory;
    [HideInInspector]
    public IKController ik;
    private float vertical;
	private float horizontal;
	[HideInInspector] public bool isMove;
	public float moveDampTime = 0.1f;
	public CGUI gui;
	public CameraController cameraController;
	[HideInInspector] public Transform moveTarget;
	[HideInInspector] public bool canControl = true;
	[HideInInspector] public bool followPhysics = true;
	[HideInInspector] public Vector3 horizontalForward;
	public CameraOrbitCenterInfo cameraCenter;
	private Transform cameraTarget;
	private float aimAngle;
	[HideInInspector] public CarControllerAlt carController;

	//FootIK
	public LayerMask footPlacementLayer;
	public Vector3 footOffset;

	//Movement
	public Transform targetTransform;
	public float rotateSmoothnessAim = 0.1f;

	[HideInInspector]
    public Transform originalParent;

	public List<GameObject> ItemsInHand;

    public Transform leftHandIKTarget;
    public Transform rightHandIKTarget;

    void Awake () {
		animatorMovement = animator.GetComponent<AnimatorMovement>();
        ik = animator.GetComponent<IKController>();
        animatorEvents = animator.GetComponent<AnimatorEvents>();
        animatorEvents.character = this;
        animatorMovement.physicsRigidbody = GetComponent<Rigidbody>();
		animatorMovement.physicsTransform = transform;
		
		interaction = GetComponent<CharacterInteraction>();
		ragdoll = GetComponent<CharacterRagdollController>();
		input = GetComponent<InputManager>();
		inventory = GetComponent<Inventory>();
        weaponsController = GetComponent<Skill>();
        skillFirearmsVariables = GetComponent<SkillFirearmsVariables>();
        weaponsController.skillFirearmsVariables = skillFirearmsVariables;

        animatorMovement.character = this;
		interaction.character = this;
		input.character = this;
		weaponsController.character = this;
		ragdoll.character = this;
		gui.character = this;
		inventory.character = this;
        ik.character = this;


		GameObject cameraTargetGameObject = new GameObject("Camera Target");
		cameraTarget = cameraTargetGameObject.transform;

		cameraController.cameraTarget = cameraTarget;
		
		GameObject moveTargetGameObject = new GameObject("Move Target");
		moveTarget = moveTargetGameObject.transform;
		originalParent = transform.parent;
	}

	void Update () {
		if (canControl) GetInputUser ();
		else if (followPhysics) GetInputNoControl ();
		SetAnimatorVariables ();
		animatorMovement.SetPositionRotation ();
		cameraController.isZoom = weaponsController.GetIsAim();
		
		Vector3 down = - transform.up;
		Vector3 camForward = cameraController.transform.forward;
		aimAngle = Vector3.Angle (camForward, down);
	}

	void GetInputUser () {
		weaponsController.SetIsAim (input.isAim);
		vertical = input.vertical;
		horizontal = input.horizontal;
		isMove = input.isMove;
		if (!weaponsController.GetIsReloadWeapon()) weaponsController.SetIsShootHold(input.isShootHold);

        if (input.isAim) horizontalForward = cameraController.cameraTarget.position - transform.position;
        else horizontalForward = cameraController.transform.forward;
    }

	void GetInputNoControl ()
    {
        weaponsController.SetIsAim(input.isAim);
        horizontal = 0f;
        weaponsController.SetIsShootHold(false);
		vertical = 1.522809f;
		isMove = true;

		horizontalForward = moveTarget.position - transform.position;
	}

	public void StopCharacter () {
        weaponsController.SetIsAim(false);
        vertical = 0f;
		horizontal = 0f;
		isMove = false;
		horizontalForward = Vector3.zero;
	}

	void SetAnimatorVariables () {
		animator.SetBool (Animator.StringToHash ("IsAim"), weaponsController.GetIsAim());
		animator.SetFloat (Animator.StringToHash ("Vertical"), vertical, moveDampTime, Time.deltaTime);
		animator.SetFloat (Animator.StringToHash ("Horizontal"), horizontal, moveDampTime, Time.deltaTime);
		animator.SetBool (Animator.StringToHash ("IsMove"), isMove);
		if (carController) animator.SetFloat (Animator.StringToHash ("Steering"), carController.steering, moveDampTime, Time.deltaTime);
		animator.SetFloat (Animator.StringToHash ("AimAngle"), aimAngle, moveDampTime, Time.deltaTime);
		animator.SetBool (Animator.StringToHash ("IsArmed"), weaponsController.GetIsArmed());
		animator.SetBool (Animator.StringToHash ("IsReloadWeapon"), weaponsController.GetIsReloadWeapon());
	}
	
	public void StartGoToInteraction (InteractiveObject interactiveObject) {
		if (weaponsController.CheckIfCanStartGoToInteraction()) {
            weaponsController.StartGoToInteraction();
			interaction.StartGoToInteraction (interactiveObject);
		}
	}

	public void InterruptInteraction () {
		interaction.InterruptInteraction ();
    }

    public void Shoot ()
    {
		weaponsController.SetShootTrigger(true);
	}
    public void NextFireMode()
    {
        weaponsController.NextFireMode();
    }

   

    public void UseItem () {
		if ((inventory.selectedItem != null) && (!weaponsController.GetIsArmed()) && (interaction.GetState () == CharacterInteraction.State.Move)) {
			animator.SetTrigger (Animator.StringToHash ("UseItem"));
			animator.SetInteger (Animator.StringToHash ("ItemIndex"), inventory.selectedItemVisualIndex);
			ik.SetLookAtWeight(0f);
		}
    }

    public void SwitchControl(bool switcher)
    {
        cameraController.isControllable = switcher;
        input.isCharacterControllable = switcher;
        input.vertical = 0f;
        input.horizontal = 0f;
        input.isMove = false;
        if (!input.debugAimOn) input.isAim = false;
        input.isShootHold = false;
    }
}
