using UnityEngine;
using System.Collections;

public class AIZombieState_Feeding1 : AIZombieState 
{
	[SerializeField]						float		_slerpSpeed					=	5.0f;
	[SerializeField]						Transform	_bloodParticlesMount		=	null;
	[SerializeField] [Range(0.01f, 1.0f)] 	float 		_bloodParticlesBurstTime	=	0.1f;
	[SerializeField] [Range(1, 100)]		int 		_bloodParticlesBurstAmount 	= 	10;

	private int 			_eatingStateHash 	= Animator.StringToHash("Feeding State");
	private int				_eatingLayerIndex	= -1;
	private float			_timer				= 0.0f;
	public override AIStateType GetStateType() { return AIStateType.Feeding; }
	public override void 		OnEnterState()
	{
		Debug.Log ("Entering feeding State");
		base.OnEnterState ( );
		if (_zombieStateMachine==null) return;
		if (_eatingLayerIndex==-1 )
			_eatingLayerIndex= _zombieStateMachine.animator.GetLayerIndex("Cinematic");
		_timer = 0.0f;
		_zombieStateMachine.feeding			= true;
		_zombieStateMachine.seeking 		= 0;
		_zombieStateMachine.speed 			= 0;
		_zombieStateMachine.attackType		= 0;
		_stateMachine.NavAgentControl(true,false);
	}

	public override void OnExitState()
	{
		if (_zombieStateMachine!=null)
			_zombieStateMachine.feeding = false;
	}
	public override AIStateType	OnUpdate( )	
	{ 
		_timer += Time.deltaTime;

		if (_zombieStateMachine.satisfaction > 0.9f) 
		{
			_zombieStateMachine.GetWaypointPosition (false);
			return AIStateType.Alerted;
		}
		if (_zombieStateMachine.VisualThreat.type!=AITargetType.None && _zombieStateMachine.VisualThreat.type!=AITargetType.Visual_Food)
		{
			_zombieStateMachine.SetTarget ( _zombieStateMachine.VisualThreat );
			return AIStateType.Alerted;
		}	
		if (_zombieStateMachine.AudioThreat.type==AITargetType.Audio )
		{
			_zombieStateMachine.SetTarget ( _zombieStateMachine.AudioThreat);
			return AIStateType.Alerted;
		}
		if (_zombieStateMachine.animator.GetCurrentAnimatorStateInfo(_eatingLayerIndex).shortNameHash == _eatingStateHash)
		{
			_zombieStateMachine.satisfaction = Mathf.Min (_zombieStateMachine.satisfaction + ((Time.deltaTime * _zombieStateMachine.replenishRate)/100.0f),1.0f);
			if (GameSceneManager.instance && GameSceneManager.instance.bloodParticles && _bloodParticlesMount) 
			{
				if (_timer > _bloodParticlesBurstTime) 
				{
					ParticleSystem system = GameSceneManager.instance.bloodParticles;
					system.transform.position = _bloodParticlesMount.transform.position;
					system.transform.rotation = _bloodParticlesMount.transform.rotation;
					system.simulationSpace = ParticleSystemSimulationSpace.World;
					system.Emit (_bloodParticlesBurstAmount);
					_timer = 0.0f;
				}

			}
		}
			
		if (!_zombieStateMachine.useRootRotation)
		{
			Vector3 targetPos = _zombieStateMachine.targetPosition;
			targetPos.y = _zombieStateMachine.transform.position.y;
			Quaternion  newRot = Quaternion.LookRotation (  targetPos - _zombieStateMachine.transform.position);
			_zombieStateMachine.transform.rotation = Quaternion.Slerp( _zombieStateMachine.transform.rotation, newRot, Time.deltaTime* _slerpSpeed);
		}
		return AIStateType.Feeding;
	}

}
