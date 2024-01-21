using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestruct : MonoBehaviour 
{

	[SerializeField] private float _time	=	10.0f;

	// Use this for initialization
	void Awake () 
	{
		Invoke(	"DestroyNow", _time );
	}
	

	void DestroyNow () 
	{
		Destroy( gameObject );
	}
}
