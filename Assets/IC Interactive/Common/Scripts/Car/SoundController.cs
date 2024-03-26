using UnityEngine;
using System.Collections;

// Simple class to controll sounds of the car, based on engine throttle and RPM, and skid velocity.
[RequireComponent (typeof (Drivetrain))]
[RequireComponent (typeof (CarControllerAlt))]
public class SoundController : MonoBehaviour {

	public AudioClip engine;
	public AudioClip skid;
	public AudioSource engineStart;
	
	AudioSource engineSource;
	AudioSource skidSource;
	
	CarControllerAlt car;
	Drivetrain drivetrain;

	private bool engineOn;
	
	AudioSource CreateAudioSource (AudioClip clip) {
		GameObject go = new GameObject("audio");
		go.transform.parent = transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.AddComponent(typeof(AudioSource));
		go.GetComponent<AudioSource>().clip = clip;
		go.GetComponent<AudioSource>().loop = true;
		go.GetComponent<AudioSource>().volume = 0;
		go.GetComponent<AudioSource>().spatialBlend = 1f;
		go.GetComponent<AudioSource>().Play();
		return go.GetComponent<AudioSource>();
	}
	
	void Awake () {
		engineSource = CreateAudioSource(engine);
		skidSource = CreateAudioSource(skid);
		car = GetComponent (typeof (CarControllerAlt)) as CarControllerAlt;
		drivetrain = GetComponent (typeof (Drivetrain)) as Drivetrain;
	}
	
	void Update () {
		engineSource.pitch = 0.5f + 1.3f * drivetrain.rpm / drivetrain.maxRPM;
		if (engineOn) {
			engineSource.volume = 0.4f + 0.6f * drivetrain.throttle;
			skidSource.volume = Mathf.Clamp01( Mathf.Abs(car.slipVelo) * 0.2f - 0.5f );
		}

		if (Input.GetKeyDown (KeyCode.I)) {
			StartEngine ();
		}
		if (Input.GetKeyDown (KeyCode.O)) {
			StopEngine ();
		}
	}

	public void StartEngine () {
		engineStart.Play ();
		StopAllCoroutines ();
		StartCoroutine (EngineVolumeUp ((0.4f + 0.6f * drivetrain.throttle), 5f));
	}

	public void StopEngine () {
		StopAllCoroutines ();
		StartCoroutine (EngineVolumeDown (2f));
		engineOn = false;
	}

	IEnumerator EngineVolumeUp (float target, float time) {
		float i = 0.0f;
		float rate = 1.0f/time;
		while (i < 1.0f) {
			i += Time.deltaTime * rate;
			engineSource.volume = Mathf.Lerp (engineSource.volume, target, i);
			yield return null;
		}
		engineOn = true;
	}
	
	IEnumerator EngineVolumeDown (float time) {
		float i = 0.0f;
		float rate = 1.0f/time;
		while (i < 1.0f) {
			i += Time.deltaTime * rate;
			engineSource.volume = Mathf.Lerp (engineSource.volume, 0f, i);
			yield return null;
		}
	}
}
