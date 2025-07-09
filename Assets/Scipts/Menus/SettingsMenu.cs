using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;

    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;



    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        float value;

        if (audioManager.audioMixer.GetFloat("GeneralVolume", out value))
            masterSlider.value = Mathf.Pow(10f, value / 20f);

        if (audioManager.audioMixer.GetFloat("MusicVolume", out value))
            musicSlider.value = Mathf.Pow(10f, value / 20f);

        if (audioManager.audioMixer.GetFloat("SFXVolume", out value))
            sfxSlider.value = Mathf.Pow(10f, value / 20f);

    }
    public void OnMasterVolumeChanged(float value)
    {
        audioManager.SetMasterVolume(value);
    }

    public void OnMusicVolumeChanged(float value)
    {
        audioManager.SetMusicVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        audioManager.SetSFXVolume(value);
    }
    
}
