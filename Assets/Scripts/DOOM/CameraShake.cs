using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 originalPos;

    void Awake()
    {
        Instance = this;
        originalPos = transform.localPosition;
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    System.Collections.IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 offset =
                Random.insideUnitSphere * magnitude;

            transform.localPosition =
                originalPos + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}