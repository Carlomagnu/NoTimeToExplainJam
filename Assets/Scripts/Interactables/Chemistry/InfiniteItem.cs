using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteItem : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemName;
    [SerializeField] private GameObject itemPrefabToInstantiate; // The prefab to spawn instead of cloning

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip interactSound;
    [SerializeField] private bool playOnInteract = true;

    public void Interact(PlayerInteract player)
    {
        // Play audio if enabled
        if (playOnInteract && audioSource != null && interactSound != null)
        {
            audioSource.PlayOneShot(interactSound);
        }

        // Instantiate the prefab
        GameObject itemCopy;
        if (itemPrefabToInstantiate != null)
        {
            itemCopy = Instantiate(itemPrefabToInstantiate, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }
        else
        {
            // Fallback: clone this item if no prefab is assigned
            itemCopy = Instantiate(gameObject);
        }

        // Remove the InfiniteItem component from the copy (so it's just a regular item)
        InfiniteItem infiniteComponent = itemCopy.GetComponent<InfiniteItem>();
        if (infiniteComponent != null)
        {
            Destroy(infiniteComponent);
        }

        // Add GenericInteract to the copy so it can be picked up normally
        GenericInteract genericInteract = itemCopy.GetComponent<GenericInteract>();
        if (genericInteract == null)
        {
            itemCopy.AddComponent<GenericInteract>();
        }

        // Add rigidbody if it doesn't exist, and enable physics with gravity
        Rigidbody rb = itemCopy.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = itemCopy.AddComponent<Rigidbody>();
        }
        rb.isKinematic = false;
        rb.useGravity = true;

        // Enable collider
        Collider col = itemCopy.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
        else
        {
            // Add a collider if none exists
            itemCopy.AddComponent<BoxCollider>();
        }

        // Pick up the copy
        player.PickUp(itemCopy);
        Debug.Log($"Picked up {itemName}. Original remains on the table.");
    }
}