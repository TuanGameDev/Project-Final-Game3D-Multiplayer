using UnityEngine;
using System.Collections;

public abstract class AIState : MonoBehaviour 
{
	public virtual void SetStateMachine( AIStateMachine stateMachine ) { _stateMachine = stateMachine; }

	// Default Handlers
	public virtual void			OnEnterState()			{}
	public virtual void 		OnExitState()			{}
	public virtual void 		OnAnimatorIKUpdated()	{}
	public virtual void 		OnTriggerEvent( AITriggerEventType eventType, Collider other ){}
	public virtual void 		OnDestinationReached ( bool isReached ) {}
	public abstract AIStateType GetStateType();
	public abstract AIStateType OnUpdate();

	protected AIStateMachine	_stateMachine;
	public virtual void 		OnAnimatorUpdated() 	
	{
		if (_stateMachine.useRootPosition)
			_stateMachine.navAgent.velocity = _stateMachine.animator.deltaPosition / Time.deltaTime;

		if (_stateMachine.useRootRotation)
			_stateMachine.transform.rotation = _stateMachine.animator.rootRotation;

	}
	public static void ConvertSphereColliderToWorldSpace( SphereCollider col, out Vector3 pos, out float radius )
	{
		pos 	= Vector3.zero;
		radius 	= 0.0f;
		if (col == null)
			return;
		pos    = col.transform.position;
		pos.x += col.center.x * col.transform.lossyScale.x;
		pos.y += col.center.y * col.transform.lossyScale.y;
		pos.z += col.center.z * col.transform.lossyScale.z;
		radius = Mathf.Max(	col.radius * col.transform.lossyScale.x,
							col.radius * col.transform.lossyScale.y); 

		radius = Mathf.Max( radius, col.radius * col.transform.lossyScale.z);
	}
	public static float FindSignedAngle( Vector3 fromVector, Vector3 toVector )
	{
		if (fromVector == toVector)
			return 0.0f;

		float angle = Vector3.Angle (fromVector, toVector);
		Vector3 cross = Vector3.Cross (fromVector, toVector);
		angle *= Mathf.Sign (cross.y);
		return angle;
	}
}
