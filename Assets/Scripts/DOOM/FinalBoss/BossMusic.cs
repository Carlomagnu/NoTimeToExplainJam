using UnityEngine;
using System.Collections;

public class BossMusic : MonoBehaviour
{
    public static BossMusic Instance;

    [Header("Audio Sources")]
    [SerializeField] AudioSource speaker;   // main music source
    [SerializeField] AudioSource speaker2;  // for speech or SFX

    [Header("Clips")]
    [SerializeField] AudioClip DoomMusic;
    [SerializeField] AudioClip EtherealMusic;
    [SerializeField] AudioClip Impact;
    [SerializeField] AudioClip Speech;

    void Awake()
    {
        Instance = this;
    }


    public void PlayDoomMusic()
    {
        if (!speaker.isPlaying)
        {
            speaker.clip = DoomMusic;
            speaker.volume = 0.5f;
            speaker.Play();
        }
    }

    public void PlayImpactAndSilence()
    {
        StartCoroutine(ImpactAndSilenceRoutine());
    }

    public void PlayImpact()
    {
        speaker.PlayOneShot(Impact, 2f);
    }

    private IEnumerator ImpactAndSilenceRoutine()
    {
        // Play impact immediately
        speaker2.PlayOneShot(Impact, 1.5f);

        // Fade out doom instantly or over a short time
        float fadeTime = 0.5f;
        float t = 0f;
        float startVol = speaker.volume;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            speaker.volume = Mathf.Lerp(startVol, 0f, t / fadeTime);
            yield return null;
        }

        speaker.Stop();
        speaker.volume = 0f;
    }

    public void FadeInEthereal()
    {
        StartCoroutine(FadeInEtherealRoutine());
    }

    private IEnumerator FadeInEtherealRoutine()
    {
        speaker.clip = EtherealMusic;
        speaker.volume = 0f;
        speaker.Play();

        float fadeTime = 2.0f;
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            speaker.volume = Mathf.Lerp(0f, 0.5f, t / fadeTime);
            yield return null;
        }
    }

    public void PlaySpeech()
    {
        speaker2.PlayOneShot(Speech, 1f);
    }

    public void FadeOutAllSound(float duration = 1.5f)
    {
        StartCoroutine(FadeOutAllRoutine(duration));
    }

    private IEnumerator FadeOutAllRoutine(float duration)
    {
        float t = 0f;

        float startVol1 = speaker.volume;
        float startVol2 = speaker2.volume;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            speaker.volume = Mathf.Lerp(startVol1, 0f, lerp);
            speaker2.volume = Mathf.Lerp(startVol2, 0f, lerp);

            yield return null;
        }

        speaker.volume = 0f;
        speaker2.volume = 0f;

        speaker.Stop();
        speaker2.Stop();
    }

}

