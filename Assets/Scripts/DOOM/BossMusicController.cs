using UnityEngine;
using System.Collections;

public class BossMusicController : MonoBehaviour
{
    public static BossMusicController Instance;

    [Header("Audio Sources")]
    public AudioSource ambientSource;
    public AudioSource combatSource;
    public AudioSource finalPhaseSource;

    [Header("Fade Settings")]
    public float fadeSpeed = 1.5f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Start with ambient music
        ambientSource.Play();
    }

    public void StartCombatMusic()
    {
        StartCoroutine(FadeTo(combatSource, ambientSource));
    }

    public void StartFinalPhaseMusic()
    {
        StartCoroutine(FadeTo(finalPhaseSource, combatSource));
    }

    private IEnumerator FadeTo(AudioSource fadeIn, AudioSource fadeOut)
    {
        fadeIn.volume = 0f;
        fadeIn.Play();

        while (fadeIn.volume < 1f)
        {
            fadeIn.volume += Time.deltaTime * fadeSpeed;
            fadeOut.volume -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        fadeOut.Stop();
    }
}
