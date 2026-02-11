using System.Collections;
using UnityEngine;

public class FinalTheaterSequence : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioSource narratorAudio;
    [SerializeField] AudioSource audienceAudio;
    [SerializeField] AudioClip narratorLine;      // Now THAT is a fantastic joke
    [SerializeField] AudioClip lightLaughter;
    [SerializeField] AudioClip heavyLaughter;
    [SerializeField] AudioClip impactSFX;

    [Header("Lights & Visuals")]
    [SerializeField] GameObject audienceLights;
    [SerializeField] GameObject screenFade;       // Black image to fade in

    [Header("Timings")]
    [SerializeField] float delayBeforeLaughter = 1.0f;
    [SerializeField] float lightLaughterDuration = 2.0f;
    [SerializeField] float heavyLaughterDuration = 2.0f;
    [SerializeField] float insaneLaughterDuration = 2.0f;
    [SerializeField] float fadeToBlackTime = 1.5f;

    bool sequenceStarted = false;

    // Call this from your button or boss script
    public void StartFinalSequence()
    {
        if (!sequenceStarted)
        {
            sequenceStarted = true;
            StartCoroutine(FinalSequenceRoutine());
        }
    }

    private IEnumerator FinalSequenceRoutine()
    {
        // 1. Narrator speaks
        narratorAudio.PlayOneShot(narratorLine);
        while (narratorAudio.isPlaying)
            yield return null;

        // 2. Small pause
        yield return new WaitForSeconds(delayBeforeLaughter);

        // 3. Lights on
        audienceLights.SetActive(true);

        // 4. Light laughter
        audienceAudio.PlayOneShot(lightLaughter);
        yield return new WaitForSeconds(lightLaughterDuration);

        // 5. Heavy laughter
        audienceAudio.PlayOneShot(heavyLaughter);
        yield return new WaitForSeconds(heavyLaughterDuration);

        // 6. Insane laughter (just replay heavy or use a third clip)
        audienceAudio.PlayOneShot(heavyLaughter);
        yield return new WaitForSeconds(insaneLaughterDuration);

        // 7. IMPACT
        audienceAudio.PlayOneShot(impactSFX, 2f); // louder
        yield return new WaitForSeconds(0.3f);

        // 8. Fade to black
        StartCoroutine(FadeToBlack());
    }

    private IEnumerator FadeToBlack()
    {
        CanvasGroup cg = screenFade.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = screenFade.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        screenFade.SetActive(true);

        float t = 0f;
        while (t < fadeToBlackTime)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, t / fadeToBlackTime);
            yield return null;
        }

        cg.alpha = 1f;
    }
}
