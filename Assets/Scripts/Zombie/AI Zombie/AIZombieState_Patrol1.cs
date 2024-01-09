using UnityEngine;
using System.Collections;
public class AIZombieState_Patrol1 : AIZombieState
{
	[SerializeField] float _turnOnSpotThreshold = 80.0f;
	[SerializeField] float _slerpSpeed = 5.0f;

	[SerializeField][Range(0.0f, 3.0f)] float _speed = 1.0f;
	public override AIStateType GetStateType()
	{
		return AIStateType.Patrol;
	}
	public override void OnEnterState()
	{
		Debug.Log("Entering Patrol State");
		base.OnEnterState();
		if (_zombieStateMachine == null)
			return;
		_zombieStateMachine.NavAgentControl(true, false);
		_zombieStateMachine.seeking = 0;
		_zombieStateMachine.feeding = false;
		_zombieStateMachine.attackType = 0;
		_zombieStateMachine.navAgent.SetDestination(_zombieStateMachine.GetWaypointPosition(false));
		_zombieStateMachine.navAgent.Resume();
	}
	public override AIStateType OnUpdate()
	{
		if (_zombieStateMachine.VisualThreat.type == AITargetType.Visual_Player)
		{
			_zombieStateMachine.SetTarget(_zombieStateMachine.VisualThreat);
			return AIStateType.Pursuit;
		}

		if (_zombieStateMachine.VisualThreat.type == AITargetType.Visual_Light)
		{
			_zombieStateMachine.SetTarget(_zombieStateMachine.VisualThreat);
			return AIStateType.Alerted;
		}
		if (_zombieStateMachine.AudioThreat.type == AITargetType.Audio)
		{
			_zombieStateMachine.SetTarget(_zombieStateMachine.AudioThreat);
			return AIStateType.Alerted;
		}
		if (_zombieStateMachine.VisualThreat.type == AITargetType.Visual_Food)
		{
			if ((1.0f - _zombieStateMachine.satisfaction) > (_zombieStateMachine.VisualThreat.distance / _zombieStateMachine.sensorRadius))
			{
				_stateMachine.SetTarget(_stateMachine.VisualThreat);
				return AIStateType.Pursuit;
			}
		}
		if (_zombieStateMachine.navAgent.pathPending)
		{
			_zombieStateMachine.speed = 0;
			return AIStateType.Patrol;
		}
		else
			_zombieStateMachine.speed = _speed;
		float angle = Vector3.Angle(_zombieStateMachine.transform.forward, (_zombieStateMachine.navAgent.steeringTarget - _zombieStateMachine.transform.position));
		if (angle > _turnOnSpotThreshold)
		{
			return AIStateType.Alerted;
		}
		if (!_zombieStateMachine.useRootRotation)
		{
			Quaternion newRot = Quaternion.LookRotation(_zombieStateMachine.navAgent.desiredVelocity);
			_zombieStateMachine.transform.rotation = Quaternion.Slerp(_zombieStateMachine.transform.rotation, newRot, Time.deltaTime * _slerpSpeed);
		}
		if (_zombieStateMachine.navAgent.isPathStale ||
			!_zombieStateMachine.navAgent.hasPath ||
			_zombieStateMachine.navAgent.pathStatus != UnityEngine.AI.NavMeshPathStatus.PathComplete)
		{
			_zombieStateMachine.navAgent.SetDestination(_zombieStateMachine.GetWaypointPosition(true));
		}
		return AIStateType.Patrol;
	}
	public override void OnDestinationReached(bool isReached)
	{
		if (_zombieStateMachine == null || !isReached)
			return;

		if (_zombieStateMachine.targetType == AITargetType.Waypoint)
			_zombieStateMachine.navAgent.SetDestination(_zombieStateMachine.GetWaypointPosition(true));
	}
}
