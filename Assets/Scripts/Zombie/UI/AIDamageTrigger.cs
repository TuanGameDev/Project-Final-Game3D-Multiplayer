using UnityEngine;
using System.Collections;

public class AIDamageTrigger : MonoBehaviour 
{
	// Inspector Variables
	[SerializeField] string			_parameter = "";
	[SerializeField] int			_bloodParticlesBurstAmount	=	10;
	[SerializeField] float			_damageAmount				=	0.1f;

	// Private Variables
	AIStateMachine  	_stateMachine 		= null;
	Animator	   	 	_animator	 		= null;
	int			    	_parameterHash		= -1;

	// ------------------------------------------------------------
	// Name	:	Start
	// Desc	:	Called on object start-up to initialize the script.
	// ------------------------------------------------------------
	void Start()
	{
		// Cache state machine and animator references
		_stateMachine = transform.root.GetComponentInChildren<AIStateMachine> ();

		if (_stateMachine != null)
			_animator = _stateMachine.animator;

		// Generate parameter hash for more efficient parameter lookups from the animator
		_parameterHash = Animator.StringToHash (_parameter); 

	}
}
