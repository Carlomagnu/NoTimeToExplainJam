using UnityEngine;
using System.Collections;

public class FogControl : MonoBehaviour
{
    [SerializeField] private SceneTransition transition;
    [Header("Timing")]
    [SerializeField] private float delayBeforeIncrease = 2f; // n seconds
    [SerializeField] private float increaseDuration = 3f;
    [SerializeField] private float delayBeforeDecrease = 2f; // k seconds
    [SerializeField] private float decreaseDuration = 3f;
    [Header("Fog Settings")]
    [SerializeField] private float targetFogDensity = 0.04f;
    [SerializeField] private float enemyMoveFogDensity = 0.04f;
    [Header("Scene Transition")]
    [SerializeField] private float timeBeforeSceneChange = 60f; // p seconds
    [SerializeField] private string nextSceneName = "NextScene"; // Name of the scene to load
    [Header("Object Deletion")]
    [SerializeField] private bool enableObjectDeletion = true; // Toggle object deletion

    public Transform Enemy;
    public bool isFogActive => RenderSettings.fogDensity > enemyMoveFogDensity;
    private Coroutine fogRoutine;
    private Coroutine sceneChangeRoutine;

    private void OnEnable()
    {
        fogRoutine = StartCoroutine(FogLoop());
        sceneChangeRoutine = StartCoroutine(SceneChangeTimer());
    }

    private void OnDisable()
    {
        if (fogRoutine != null)
        {
            StopCoroutine(fogRoutine);
            fogRoutine = null;
        }
        if (sceneChangeRoutine != null)
        {
            StopCoroutine(sceneChangeRoutine);
            sceneChangeRoutine = null;
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

            // Delete nearest stackable object when fog starts to decrease
            if (enableObjectDeletion)
            {
                DeleteNearestStackableObject();
            }

            // Decrease fog
            yield return AnimateFog(targetFogDensity, 0f, decreaseDuration);
        }
    }

    private void DeleteNearestStackableObject()
    {
        if (Enemy == null)
        {
            Debug.LogWarning("[FogControl] Enemy reference is null. Cannot find nearest stackable object.");
            return;
        }

        // Find all GenericInteract objects in the scene
        GenericInteract[] allInteractables = FindObjectsOfType<GenericInteract>();

        GenericInteract nearestStackable = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GenericInteract interactable in allInteractables)
        {
            // Check if it's stackable (using reflection to access private field)
            System.Reflection.FieldInfo isStackableField = typeof(GenericInteract).GetField("isStackable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (isStackableField != null)
            {
                bool isStackable = (bool)isStackableField.GetValue(interactable);

                if (isStackable)
                {
                    float distance = Vector3.Distance(Enemy.position, interactable.transform.position);

                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestStackable = interactable;
                    }
                }
            }
        }

        if (nearestStackable != null)
        {
            Debug.Log($"[FogControl] Deleting nearest stackable object: {nearestStackable.name} at distance {nearestDistance:F2}");
            Destroy(nearestStackable.gameObject);
        }
        else
        {
            Debug.Log("[FogControl] No stackable objects found to delete.");
        }
    }

    private IEnumerator SceneChangeTimer()
    {
        Debug.Log($"[FogControl] Scene change timer started. Will transition to '{nextSceneName}' in {timeBeforeSceneChange} seconds.");
        yield return new WaitForSeconds(timeBeforeSceneChange);
        Debug.Log($"[FogControl] Time elapsed. Changing scene to '{nextSceneName}'.");
        if (transition != null)
        {
            transition.changeScene(nextSceneName);
        }
        else
        {
            Debug.LogError("[FogControl] SceneTransition reference is null! Cannot change scene.");
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