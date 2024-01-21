using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerTest : MonoBehaviour 
{
	public AudioClip clip;

	// Use this for initialization
	void Start () 
	{
		if (AudioManager.instance)
			AudioManager.instance.SetTrackVolume("Zombies", 10, 5);	

			InvokeRepeating("PlayTest", 1, 1 );
	}
	
	void PlayTest()
	{
	 	AudioManager.instance.PlayOneShotSound( "Player", clip, transform.position, 0.5f, 0.0f, 128);	
	}
}
