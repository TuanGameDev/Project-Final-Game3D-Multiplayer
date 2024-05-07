using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource[] soundEffects;
    public static AudioManager _audioManager;
    private void Awake()
    {
        if (_audioManager != null && _audioManager != this)
            gameObject.SetActive(false);
        else
        {
            _audioManager = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void PlaySFX(int sxfNumber)
    {
        soundEffects[sxfNumber].Play();
    }
    public void StopSFX(int sxfNumber)
    {
        soundEffects[sxfNumber].Stop();
    }
}
