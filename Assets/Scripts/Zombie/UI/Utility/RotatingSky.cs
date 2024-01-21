using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingSky : MonoBehaviour 
{
	// Inspector Assigned
	[SerializeField]	protected Material 	_skyMaterial 	= null;
	[SerializeField]	protected float		_speed			= 1.0f;

	// Internals
	protected float 	_angle 			=0;
	protected float 	_originalAngle	=	0;

	void OnEnable()
	{
		if (_skyMaterial) _originalAngle = _angle =  _skyMaterial.GetFloat("_Rotation");
	}


	void OnDisable()
	{
		if (_skyMaterial) _skyMaterial.SetFloat("_Rotation", _originalAngle);
	}

	void Update()
	{
		if (_skyMaterial==null) return;

		_angle+=_speed * Time.deltaTime;
			if (_angle>360.0f) _angle-=360.0f;
		else
			if (_angle<0.0f) _angle+=360.0f;

		_skyMaterial.SetFloat ("_Rotation", _angle );
	}
}
