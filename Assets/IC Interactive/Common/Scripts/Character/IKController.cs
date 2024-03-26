using UnityEngine;
using System.Collections;

public class IKController : MonoBehaviour
{
    [HideInInspector]
    public Character character;
    private Animator animator;
    private Transform lookAtTarget;
    private float lookAtWeight;
    private float lookAtWeightGoal;
    public void SetLookAtWeight(float goal)
    {
        lookAtWeightGoal = goal;
    }
    private float rightHandWeight;
    private float rightHandWeightGoal;
    private float leftHandWeight;
    private float leftHandWeightGoal;
    public float handsWeightTime = 0.1f;

    protected Vector3 footPosL;
    protected Vector3 footPosR;
    private AvatarIKGoal leftFoot;
    private AvatarIKGoal rightFoot;
    [HideInInspector]
    public float leftFootWeight;
    [HideInInspector]
    public float rightFootWeight;
    // Use this for initialization
    void Awake() {
        animator = GetComponent<Animator>();
        lookAtWeightGoal = 1f;

        GameObject lookAtTargetGameObject = new GameObject("Look At Target");
        lookAtTarget = lookAtTargetGameObject.transform;
    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
        float increment = Time.deltaTime * 1f / handsWeightTime;
        rightHandWeight = Mathf.Lerp(rightHandWeight, rightHandWeightGoal, increment);
        leftHandWeight = Mathf.Lerp(leftHandWeight, leftHandWeightGoal, increment);
        lookAtWeight = Mathf.Lerp(lookAtWeight, lookAtWeightGoal, increment);

        lookAtTarget.position = Vector3.Lerp (lookAtTarget.position, character.cameraController.cameraTarget.position, increment);

    }

    public void EngageArmPose()
    {
        if (character.weaponsController.GetWeaponIndex() == 0)
        {
            leftHandWeightGoal = 1f;
            Transform tempLeftHandArmed = character.weaponsController.GetCurrentFirearmLeftHandArmed();
            character.leftHandIKTarget.parent = tempLeftHandArmed;
            character.leftHandIKTarget.position = tempLeftHandArmed.position;
            character.leftHandIKTarget.rotation = tempLeftHandArmed.rotation;
        }
    }

    public void DisengageArmPose()
    {
        leftHandWeightGoal = 0f;
        leftHandWeight = 0f;
    }

    public void EngageAim()
    {
        rightHandWeightGoal = 1f;
        character.rightHandIKTarget.parent = character.weaponsController.GetRightHandAim();
        character.rightHandIKTarget.position = character.weaponsController.GetRightHandAim().position;
        character.rightHandIKTarget.rotation = character.weaponsController.GetRightHandAim().rotation;

        leftHandWeightGoal = 1f;
        character.leftHandIKTarget.parent = character.weaponsController.GetLeftHandAim();
        character.leftHandIKTarget.position = character.weaponsController.GetLeftHandAim().position;
        character.leftHandIKTarget.rotation = character.weaponsController.GetLeftHandAim().rotation;

        SetLookAtWeight(0f);
    }

    public void DisengageAim()
    {
        EngageArmPose();
        rightHandWeightGoal = 0f;
        SetLookAtWeight(1f);
        if (character.weaponsController.GetWeaponIndex() == 1) leftHandWeightGoal = 0f;
    }

    void OnAnimatorIK()
    {
        character.animator.SetLookAtWeight(lookAtWeight);
        character.animator.SetLookAtPosition(lookAtTarget.position);

        leftFootWeight = character.animator.GetFloat("leftFootWeight");
        rightFootWeight = character.animator.GetFloat("rightFootWeight");
        FootPlacement();

        //Recoil
        if (animator)
        {
            Transform rightHandTarget = character.rightHandIKTarget;
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);

            Transform leftHandTarget = character.leftHandIKTarget;
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
        }
    }

    void FootPlacement()
    {
        leftFoot = AvatarIKGoal.LeftFoot;
        rightFoot = AvatarIKGoal.RightFoot;
        PlaceFoot(footPosL, leftFoot, leftFootWeight);
        PlaceFoot(footPosR, rightFoot, rightFootWeight);
    }

    void PlaceFoot(Vector3 footPos, AvatarIKGoal goal, float weight)
    {
        character.animator.SetIKPositionWeight(goal, weight);
        character.animator.SetIKRotationWeight(goal, weight);
        RaycastHit hit;
        footPos = character.animator.GetIKPosition(goal);
        if (Physics.Raycast(footPos + Vector3.up, Vector3.down, out hit, 2.0f, character.footPlacementLayer))
        {
            character.animator.SetIKPosition(goal, hit.point + character.footOffset);
            Vector3 footLForward = character.animator.GetIKRotation(goal) * Vector3.forward;
            character.animator.SetIKRotation(goal, Quaternion.LookRotation(Vector3.ProjectOnPlane(footLForward, hit.normal), hit.normal));
            footPos = hit.point;
        }
    }
}
