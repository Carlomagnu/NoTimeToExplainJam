using UnityEngine;
using System.Collections;

public class FogControl : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float delayBeforeIncrease = 2f; // n seconds
    [SerializeField] private float increaseDuration = 3f;
    [SerializeField] private float delayBeforeDecrease = 2f; // k seconds
    [SerializeField] private float decreaseDuration = 3f;

    [Header("Fog Settings")]
    [SerializeField] private float targetFogDensity = 0.04f;

    private Coroutine fogRoutine;

    private void OnEnable()
    {
        fogRoutine = StartCoroutine(FogLoop());
    }

    private void OnDisable()
    {
        if (fogRoutine != null)
        {
            StopCoroutine(fogRoutine);
            fogRoutine = null;
        }
    }

    private IEnumerator FogLoop()
    {
        while (true)
        {
            // Ensure fog starts at zero
            RenderSettings.fogDensity = 0f;

            // Wait before increasing
            yield return new WaitForSeconds(delayBeforeIncrease);

            // Increase fog
            yield return AnimateFog(0f, targetFogDensity, increaseDuration);

            // Wait before decreasing
            yield return new WaitForSeconds(delayBeforeDecrease);

            // Decrease fog
            yield return AnimateFog(targetFogDensity, 0f, decreaseDuration);
        }
    }

    private IEnumerator AnimateFog(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Quadratic easing (ease in/out symmetric)
            float easedT = t * t;

            RenderSettings.fogDensity = Mathf.Lerp(from, to, easedT);
            yield return null;
        }

        RenderSettings.fogDensity = to;
    }
}
