using UnityEngine;
using System.Collections;

public enum DoorState { Open, Animating, Closed };

public class SlidingDoorDemo : MonoBehaviour 
{
	// Public Members
	public	float			SlidingDistance	=	4.0f;
	public  float			Duration		=	1.5f;
	public  AnimationCurve	JumpCurve		=	new AnimationCurve();

	// Private Members
	private	Transform		_transform		=	null;
	private Vector3			_openPos		=	Vector3.zero;
	private Vector3			_closedPos		=	Vector3.zero;
	private DoorState		_doorState		=	DoorState.Closed;	


	// Use this for initialization
	void Start () 
	{
		_transform 	= transform;
		_closedPos	= _transform.position;
		_openPos	= _closedPos + ( _transform.right * SlidingDistance );

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown( KeyCode.Space ) && _doorState!=DoorState.Animating)
		{
			StartCoroutine( AnimateDoor( (_doorState==DoorState.Open)?DoorState.Closed:DoorState.Open ) );
		}
	}

	IEnumerator AnimateDoor ( DoorState newState )
	{
		_doorState = DoorState.Animating;
		float time = 0.0f;
		Vector3 startPos = (newState==DoorState.Open)?_closedPos:_openPos;
		Vector3 endPos	 = (newState==DoorState.Open)?_openPos:_closedPos;

		while ( time <= Duration)
		{
			float t = time/Duration;
			_transform.position = Vector3.Lerp(startPos, endPos, JumpCurve.Evaluate(t)) ;
			time+=Time.deltaTime;
			yield return null;
		}

		_transform.position = endPos;
		_doorState = newState;
	}
}
