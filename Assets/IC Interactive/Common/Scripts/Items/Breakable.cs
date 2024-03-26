using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour {
    public float restoreTime;
    private float timer;
    private bool restored = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (timer > 0) timer -= Time.deltaTime;
        else if (!restored) Restore();
    }

    public void Break()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshCollider>().enabled = false;
        GetComponent<AudioSource>().Play();
        timer = restoreTime;
        restored = false;
    }
    public void Restore()
    {
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<MeshCollider>().enabled = true;
        restored = true;
    }
}
