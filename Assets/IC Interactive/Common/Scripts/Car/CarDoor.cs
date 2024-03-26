using UnityEngine;
using System.Collections;

public class CarDoor : MonoBehaviour {
	public AudioClip doorOpenSound;
	public AudioClip doorCloseSound;
	private AudioSource source;
	
	void Awake () {
		source = GetComponent<AudioSource>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void PlayOpenDoorSound () {
		source.PlayOneShot (doorOpenSound);
	}
	void PlayCloseDoorSound () {
		source.PlayOneShot (doorCloseSound);
	}
}
