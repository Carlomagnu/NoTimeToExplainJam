using UnityEngine;
using System.Collections;

public class BossMusic : MonoBehaviour
{
    public static BossMusic Instance;

    [SerializeField] AudioSource speaker;
    [SerializeField] AudioClip DoomMusic;
    [SerializeField] AudioClip EtherealMusic;

    void Awake()
    {
        Instance = this;
    }

    public void PlayDoomMusic()
    {
        if (!speaker.isPlaying)
            speaker.PlayOneShot(DoomMusic, 0.5f);
    }

    public void StopDoomAndDoEthereal()
    {
        StartCoroutine(FadeToEthereal());
    }

    private IEnumerator FadeToEthereal()
    {
        float duration = 1.5f;   // fade time
        float time = 0f;

        float startVolume = speaker.volume;

        // Fade out DOOM
        while (time < duration)
        {
            time += Time.deltaTime;
            speaker.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        speaker.Stop();
        speaker.clip = EtherealMusic;
        speaker.Play();

        // Fade in ethereal
        time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            speaker.volume = Mathf.Lerp(0f, 0.7f, time / duration);
            yield return null;
        }
    }
}

