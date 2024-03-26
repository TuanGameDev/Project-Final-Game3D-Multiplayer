using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	[HideInInspector] public Character character;

	//Keys
	[HideInInspector] public KeyCode forward;
	[HideInInspector] public KeyCode backward;
	[HideInInspector] public KeyCode right;
	[HideInInspector] public KeyCode left;
	[HideInInspector] public KeyCode sprint;
	[HideInInspector] public KeyCode aim;
	[HideInInspector] public KeyCode startInteraction;
	[HideInInspector] public KeyCode stopInteraction;
	[HideInInspector] public KeyCode shoot;
	[HideInInspector] public KeyCode disarm;
	[HideInInspector] public KeyCode weaponSlot1;
	[HideInInspector] public KeyCode weaponSlot2;
	[HideInInspector] public KeyCode weaponSlot3;
	[HideInInspector] public KeyCode nextFireMode;
	[HideInInspector] public KeyCode dropItemInHands;
	[HideInInspector] public KeyCode reloadWeapon;
	[HideInInspector] public KeyCode showMenu;
	[HideInInspector] public KeyCode hideMenu;
	[HideInInspector] public KeyCode useItem;
	[HideInInspector] public KeyCode showInventory;

	//Vars
	[HideInInspector] public float vertical;
	[HideInInspector] public float horizontal;
	[HideInInspector] public bool isAim;
	[HideInInspector] public bool isMove;
	[HideInInspector] public bool isShootHold;

	public bool isAI;
	public bool debugAimOn;
	[HideInInspector] public bool isCharacterControllable = false;

	//private vars
	private float speedMultiplyer;

	void Awake () {
		forward = KeyCode.W;
		backward = KeyCode.S;
		right = KeyCode.D;
		left = KeyCode.A;
		sprint = KeyCode.Space;
		aim = KeyCode.Mouse1;
		startInteraction = KeyCode.E;
		stopInteraction = KeyCode.Q;
		shoot = KeyCode.Mouse0;
		disarm = KeyCode.F;
		weaponSlot1 = KeyCode.Alpha1;
		weaponSlot2 = KeyCode.Alpha2;
		weaponSlot3 = KeyCode.Alpha3;
		nextFireMode = KeyCode.V;
		dropItemInHands = KeyCode.G;
		reloadWeapon = KeyCode.R;
		showMenu = KeyCode.C;
		hideMenu = KeyCode.X;
		useItem = KeyCode.BackQuote;
		showInventory = KeyCode.Tab;
	}

	void Update () {
		if (!isAI) {
			GetInputGUI ();
			if (isCharacterControllable) GetInputCharacter ();
		}
	}

	void GetInputCharacter () {
		if (Input.GetKey (forward) || Input.GetKey (backward)) {
			if (Input.GetKey (forward) && Input.GetKey (backward)) vertical = 0f;
			else if (Input.GetKey (forward) && !Input.GetKey (backward)) vertical = 1f;
			else if (!Input.GetKey (forward) && Input.GetKey (backward)) vertical = -1f;
		}
		else vertical = 0f;
		
		if (Input.GetKey (right) || Input.GetKey (left)) {
			if (Input.GetKey (right) && Input.GetKey (left)) horizontal = 0f;
			else if (Input.GetKey (right) && !Input.GetKey (left)) horizontal = 1f;
			else if (!Input.GetKey (right) && Input.GetKey (left)) horizontal = -1f;
		}
		else horizontal = 0f;
		
		isMove = ((vertical * vertical) + (horizontal * horizontal) > 0.1f);
		
		if (Input.GetKey (sprint) && !isAim) speedMultiplyer = 5.204679f;
		else speedMultiplyer = 1.522809f;
		vertical *= speedMultiplyer;
		horizontal *= speedMultiplyer;

		if (debugAimOn) {
			if (Input.GetKeyDown (aim)) isAim = !isAim; //Debug aim
		}
		else {
			if (Input.GetKey (aim)) isAim = true;
			else isAim = false;
		}
		
		if (Input.GetKey (shoot)) isShootHold = true;
		else isShootHold = false;
		
		if (Input.GetKeyDown (startInteraction)) {
			if (character.interaction.closestInteractiveObject) {
				character.StartGoToInteraction (character.interaction.closestInteractiveObject);
			}
		}
		if (Input.GetKeyDown (stopInteraction)) {
			character.InterruptInteraction ();
		}
		if (Input.GetKeyDown (shoot)) character.Shoot ();
		if (Input.GetKeyDown (disarm)) character.weaponsController.Disarm ();
		if (Input.GetKeyDown (weaponSlot1)) character.weaponsController.WeaponSlot (0);
		if (Input.GetKeyDown (weaponSlot2)) character.weaponsController.WeaponSlot (1);
		if (Input.GetKeyDown (weaponSlot3)) character.weaponsController.WeaponSlot (2);
		if (Input.GetKeyDown (nextFireMode)) character.NextFireMode ();
		if (Input.GetKeyDown (dropItemInHands)) character.weaponsController.DropItemInHands ();
		if (Input.GetKeyDown (reloadWeapon)) character.weaponsController.ReloadWeapon ();
		if (Input.GetKeyDown (useItem)) character.UseItem ();
	}

	void GetInputGUI () {
		if (Input.GetKeyDown (showMenu)) character.gui.ShowIneractionMenu (true);
		if (Input.GetKeyDown (showInventory)) character.gui.ShowInventory (true);
		if (Input.GetKeyDown (hideMenu)) {
			character.gui.ShowIneractionMenu (false);
			character.gui.ShowInventory (false);
		}
	}
}
