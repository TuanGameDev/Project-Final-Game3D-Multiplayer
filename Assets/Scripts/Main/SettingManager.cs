using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private AudioMixer MixerSFX;
    [SerializeField] private AudioMixer MixerMusic;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        MixerSFX.SetFloat("sfx", volume);
    }
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        MixerMusic.SetFloat("music", volume);
    }
}
