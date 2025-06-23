using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{


    public AudioSource gameMusic;
    public AudioSource pauseMusic;
    public float fadeDuration = 0.5f;

    public float masterVolume = 1f; 
    private float musicVolume => Mathf.Clamp(masterVolume - 0.4f, 0f, 1f);
    

    public AudioClip selectAudio;

    public AudioClip unselectAudio;
    public AudioClip meleeAudio;
    public AudioClip rangeAudio;
    public AudioClip unitDeathAudio;
    public AudioClip rotationAudio;

    private void Start()
    {
        gameMusic.volume = musicVolume;
        pauseMusic.volume = 0f;
        gameMusic.Play();
        pauseMusic.Play();
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
        float startVolFrom = from.volume;
        float startVolTo = to.volume;

        // Si la fuente destino no está reproduciendo, arrancamos
        if (!to.isPlaying) to.Play();

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime; // usar unscaled para que funcione en pausa
            float t = timer / fadeDuration;
            from.volume = Mathf.Lerp(startVolFrom, 0f, t);
            to.volume = Mathf.Lerp(startVolTo, musicVolume, t);
            yield return null;
        }

        // Asegurar valores finales
        from.volume = 0f;
        to.volume   = musicVolume;  
    }

    public void PlayOneShot(AudioClip clip)
    {
        if (clip == null) return;
        gameMusic.PlayOneShot(clip, masterVolume); // Usa el canal de gameMusic sin cortar la música
    }

}
