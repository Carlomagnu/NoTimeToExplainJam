using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialAudioInteract : MonoBehaviour, IInteractable
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip firstInteractSound;
    [SerializeField] private AudioClip subsequentInteractSound;

    [Header("State")]
    [SerializeField] private bool hasPlayedFirstSound = false;
    private bool isPlayingAudio = false;

    private void Awake()
    {
        // Cache the AudioSource component if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void Interact(PlayerInteract player)
    {
        // Don't allow interaction while audio is playing
        if (isPlayingAudio)
        {
            Debug.Log($"{gameObject.name}: Audio is still playing, ignoring interaction.");
            return;
        }

        // Check if this is the first interaction
        if (!hasPlayedFirstSound)
        {
            // Play the first sound
            if (audioSource != null && firstInteractSound != null)
            {
                StartCoroutine(PlayAudioClip(firstInteractSound));
                hasPlayedFirstSound = true;
                Debug.Log($"{gameObject.name}: Playing first interaction sound.");
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: AudioSource or firstInteractSound not assigned!");
            }
        }
        else
        {
            // Play the subsequent sound
            if (audioSource != null && subsequentInteractSound != null)
            {
                StartCoroutine(PlayAudioClip(subsequentInteractSound));
                Debug.Log($"{gameObject.name}: Playing subsequent interaction sound.");
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: AudioSource or subsequentInteractSound not assigned!");
            }
        }
    }

    private IEnumerator PlayAudioClip(AudioClip clip)
    {
        isPlayingAudio = true;
        audioSource.PlayOneShot(clip);
        
        // Wait for the clip to finish playing
        yield return new WaitForSeconds(clip.length);
        
        isPlayingAudio = false;
        Debug.Log($"{gameObject.name}: Audio finished playing.");
    }

    // Optional: Reset the interaction state (useful for testing or game mechanics)
    public void ResetInteraction()
    {
        hasPlayedFirstSound = false;
        isPlayingAudio = false;
        Debug.Log($"{gameObject.name}: Interaction state reset.");
    }
}