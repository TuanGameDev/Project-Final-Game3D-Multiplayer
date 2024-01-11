using UnityEngine;
using System.Collections;

public class AIDamageTrigger : MonoBehaviour 
{
	[SerializeField] string			_parameter = "";
	[SerializeField] int			_bloodParticlesBurstAmount	=	10;
	[SerializeField] float			_damageAmount				=	0.1f;
	AIStateMachine  	_stateMachine 		= null;
	Animator	   	 	_animator	 		= null;
	int			    	_parameterHash		= -1;
	GameSceneManager	_gameSceneManager	=	null;
	void Start()
	{
		_stateMachine = transform.root.GetComponentInChildren<AIStateMachine> ();

		if (_stateMachine != null)
			_animator = _stateMachine.animator;
		_parameterHash = Animator.StringToHash (_parameter); 

		_gameSceneManager = GameSceneManager.instance;
	}
	void OnTriggerStay( Collider col )
	{
		if (!_animator)
			return;
		if (col.gameObject.CompareTag ("Player") && _animator.GetFloat(_parameterHash) >0.9f)
		{
			if (GameSceneManager.instance && GameSceneManager.instance.bloodParticles) 
			{
				ParticleSystem system = GameSceneManager.instance.bloodParticles;
				system.transform.position = transform.position;
				system.transform.rotation = Camera.main.transform.rotation;

				system.simulationSpace = ParticleSystemSimulationSpace.World;
				system.Emit (_bloodParticlesBurstAmount);
			}

			if (_gameSceneManager!=null)
			{
				PlayerInfo info = _gameSceneManager.GetPlayerInfo( col.GetInstanceID() );
				if (info!=null && info.characterManager!=null)
				{
					info.characterManager.TakeDamage( _damageAmount );
				}
			}
		}
	}
}
