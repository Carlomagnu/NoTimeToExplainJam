using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionLockedDoor : MonoBehaviour, IInteractable
{
    [Header("Door Reference")]
    [SerializeField] private Door doorComponent;

    [Header("Lock Settings")]
    [SerializeField] private bool isLockedByGate = true; // Changed to true - starts locked!
    [SerializeField] private string doorLockErrorMessage = "The door is locked. You need to submit the correct chemical compound to unlock it.";
    
    [Header("Auto Open Settings")]
    [SerializeField] private bool autoOpenWhenUnlocked = true; // New setting

    [Header("Scene Transition Settings")]
    [SerializeField] private bool useSceneTransition = true;
    [SerializeField] private float transitionDelay = 2f; // Delay in seconds before transitioning
    [SerializeField] private SceneTransition sceneTransition;
    [SerializeField] private string nextSceneName = ""; // Name of the scene to load

    private Coroutine transitionCoroutine;

    private void Awake()
    {
        // Cache the Door component if not assigned
        if (doorComponent == null)
        {
            doorComponent = GetComponent<Door>();
        }

        // Cache the SceneTransition component if not assigned
        if (sceneTransition == null)
        {
            sceneTransition = GetComponentInChildren<SceneTransition>();
        }
    }

    private void Start()
    {
        Debug.Log($"{gameObject.name}: isLockedByGate = {isLockedByGate}");
    }

    public void Interact(PlayerInteract player)
    {
        // Check if the door is locked by the progression gate
        if (isLockedByGate)
        {
            Debug.Log($"{gameObject.name}: {doorLockErrorMessage}");
            return;
        }

        // Door is unlocked, pass the interaction to the Door script
        if (doorComponent != null)
        {
            doorComponent.Interact(player);
            
            // Start the scene transition after delay if enabled
            if (useSceneTransition && sceneTransition != null && transitionCoroutine == null)
            {
                transitionCoroutine = StartCoroutine(TransitionAfterDelay());
            }
        }
    }

    public void Unlock()
    {
        isLockedByGate = false;
        Debug.Log($"{gameObject.name}: Door unlocked by progression gate!");
    
        // Auto-open the door when unlocked
        if (autoOpenWhenUnlocked && doorComponent != null)
        {
            // Find the player in the scene to pass to Door.Interact
            PlayerInteract player = FindObjectOfType<PlayerInteract>();
        
            if (player != null)
            {
                doorComponent.Interact(player);
                
                // Start the scene transition after delay if enabled
                if (useSceneTransition && sceneTransition != null && transitionCoroutine == null)
                {
                    transitionCoroutine = StartCoroutine(TransitionAfterDelay());
                }
            }
            else
            {
                Debug.LogWarning("Could not find PlayerInteract to auto-open door!");
            }
        }
    }   

    public void Activate()
    {
        // Allows ProgressionGate to activate the door unlock via SendMessage
        Unlock();
    }

    private IEnumerator TransitionAfterDelay()
    {
        Debug.Log($"{gameObject.name}: Scene transition will occur in {transitionDelay} seconds...");
        
        yield return new WaitForSeconds(transitionDelay);
        
        if (sceneTransition != null)
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                Debug.Log($"{gameObject.name}: Triggering scene transition to {nextSceneName}!");
                sceneTransition.changeScene(nextSceneName);
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: Next scene name is not set!");
            }
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: SceneTransition component not found!");
        }
        
        transitionCoroutine = null;
    }
}