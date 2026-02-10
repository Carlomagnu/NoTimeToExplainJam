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

    private void Awake()
    {
        // Cache the Door component if not assigned
        if (doorComponent == null)
        {
            doorComponent = GetComponent<Door>();
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
}