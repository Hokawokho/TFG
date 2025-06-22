using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{


    public AudioSource gameMusic;
    public AudioSource pauseMusic;
    public float fadeDuration = 0.5f;

    public AudioClip selectAudio;

    public AudioClip unselectAudio;
    public AudioClip meleeAudio;
    public AudioClip rangeAudio;
    public AudioClip unitDeathAudio;
    public AudioClip rotationAudio;

    private void Start()
    {
        gameMusic.volume = 1f;
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
            to.volume = Mathf.Lerp(startVolTo, 1f, t);
            yield return null;
        }

        // Asegurar valores finales
        from.volume = 0f;
        to.volume = 1f;
    }

    public void PlayOneShot(AudioClip clip)
    {
        if (clip == null) return;
        gameMusic.PlayOneShot(clip); // Usa el canal de gameMusic sin cortar la música
    }

}
