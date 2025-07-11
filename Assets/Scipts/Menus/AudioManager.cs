using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{


    [Header("Music Sources")]
    public AudioSource gameMusic;
    public AudioSource pauseMusic;

    [Header("SFX Source")]
    public AudioSource sfxSource; 

    [Header("Mixer")]
    public AudioMixer audioMixer;

    public float fadeDuration = 0.5f;

    public float masterVolume = 1f; 
    //private float musicVolume => Mathf.Clamp(masterVolume - 0.4f, 0f, 1f);
    

    [Header("Clips")]
    public AudioClip selectAudio;
    public AudioClip unselectAudio;
    public AudioClip meleeAudio;
    public AudioClip rangeAudio;
    public AudioClip unitDeathAudio;
    public AudioClip rotationAudio;



    // -------AÇÒ LO QUE FA ES FERO EN SINGLETON
    private static AudioManager instance;

    private void Awake()
    {
        // Si YA existe otro AudioManager y no soy yo → me destruyo
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Soy el primero → me quedo y persisto entre escenas
        instance = this;
        DontDestroyOnLoad(gameObject);

    }


    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("GeneralVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }

    private void Start()
    {
       // gameMusic.volume = musicVolume;
        pauseMusic.volume = 0f;
        // gameMusic.Play();
        // pauseMusic.Play();
    }

    public void BeginMusic()
    {
        gameMusic.Play();
        pauseMusic.Play();

    }

    public void StopMusic()
    {
        StopAllCoroutines();
        gameMusic.Stop();
        pauseMusic.Stop();
        // Reset volumes to initial state
        gameMusic.volume = 1f;
        pauseMusic.volume = 0f;
    }
    
    public void FadeToPauseMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeVolumes(gameMusic, pauseMusic));
    }

    public void FadeToGameMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeVolumes(pauseMusic, gameMusic));
    }

    private IEnumerator FadeVolumes(AudioSource from, AudioSource to)
    {
        float timer = 0f;

        if (!to.isPlaying) to.Play();

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / fadeDuration;

            from.volume = 1f - t;
            to.volume = t;

            yield return null;
        }

        from.volume = 0f;
        to.volume = 1f;
    }



    public void PlayOneShot(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    // /* Helpers para tu UI / gameplay */
    // public void PlaySelect()      => PlayOneShot(selectAudio);
    // public void PlayUnselect()    => PlayOneShot(unselectAudio);
    // public void PlayMelee()       => PlayOneShot(meleeAudio);
    // public void PlayRange()       => PlayOneShot(rangeAudio);
    // public void PlayUnitDeath()   => PlayOneShot(unitDeathAudio);
    // public void PlayRotation()    => PlayOneShot(rotationAudio);

}
