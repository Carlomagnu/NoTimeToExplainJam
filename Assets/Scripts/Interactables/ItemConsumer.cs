using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemConsumer : MonoBehaviour, IInteractable
{
    [Header("Consumer Settings")]
    [SerializeField] private string consumerName = "Item Consumer";
    [SerializeField] private string successMessage = "Item consumed!";

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip consumeSound;

    private void Awake()
    {
        // Cache AudioSource if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void Interact(PlayerInteract player)
    {
        GameObject heldItem = player.GetHeldItem();
        
        if (heldItem == null)
        {
            Debug.Log($"{consumerName}: You need to be holding an item!");
            return;
        }

        // Play consume sound
        if (audioSource != null && consumeSound != null)
        {
            audioSource.PlayOneShot(consumeSound);
        }

        // Unparent the item
        heldItem.transform.SetParent(null);

        // Re-enable physics and colliders
        Rigidbody rb = heldItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        Collider col = heldItem.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        // Destroy the item
        Debug.Log(successMessage);
        Destroy(heldItem);
    }
}