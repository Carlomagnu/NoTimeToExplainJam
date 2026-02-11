using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBackgroundMusic : MonoBehaviour
{
    [Header("Music Settings")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool loopMusic = true;
    [SerializeField] private float volume = 0.5f;

    [Header("Fade Settings")]
    [SerializeField] private bool fadeIn = true;
    [SerializeField] private float fadeInDuration = 2f;

    private void Awake()
    {
        // Cache or create AudioSource
        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Configure the AudioSource
        musicSource.clip = backgroundMusic;
        musicSource.loop = loopMusic;
        musicSource.playOnAwake = false;
        musicSource.volume = fadeIn ? 0f : volume;
    }

    private void Start()
    {
        if (playOnStart && backgroundMusic != null)
        {
            if (fadeIn)
            {
                StartCoroutine(FadeInMusic());
            }
            else
            {
                musicSource.Play();
            }
        }
    }

    private IEnumerator FadeInMusic()
    {
        musicSource.volume = 0f;
        musicSource.Play();

        float timeElapsed = 0f;

        while (timeElapsed < fadeInDuration)
        {
            timeElapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, volume, timeElapsed / fadeInDuration);
            yield return null;
        }

        musicSource.volume = volume;
    }

    // Public methods to control music
    public void PlayMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void PauseMusic()
    {
        if (musicSource != null)
        {
            musicSource.Pause();
        }
    }

    public void UnpauseMusic()
    {
        if (musicSource != null)
        {
            musicSource.UnPause();
        }
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    public void FadeOutAndStop(float duration)
    {
        StartCoroutine(FadeOut(duration));
    }

    private IEnumerator FadeOut(float duration)
    {
        float startVolume = musicSource.volume;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, timeElapsed / duration);
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Stop();
    }
}