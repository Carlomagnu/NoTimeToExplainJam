using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string firstSceneName = "Level1";
    
    [Header("Optional Transition")]
    [SerializeField] private bool useTransition = false;
    [SerializeField] private SceneTransition sceneTransition;
    [SerializeField] private float delayBeforeLoad = 0f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip playButtonSound;
    [SerializeField] private bool playAudioOnClick = true;

    private void Awake()
    {
        // Cache the AudioSource component if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Call this method from the Play button's OnClick event
    public void PlayGame()
    {
        // Play sound effect
        if (playAudioOnClick && audioSource != null && playButtonSound != null)
        {
            audioSource.PlayOneShot(playButtonSound);
        }

        if (useTransition && sceneTransition != null)
        {
            // Use the scene transition if available
            StartCoroutine(PlayWithTransition());
        }
        else
        {
            // Direct scene load
            if (delayBeforeLoad > 0)
            {
                StartCoroutine(LoadSceneWithDelay());
            }
            else
            {
                LoadScene();
            }
        }
    }

    private void LoadScene()
    {
        Debug.Log($"Loading scene: {firstSceneName}");
        SceneManager.LoadScene(firstSceneName);
    }

    private IEnumerator LoadSceneWithDelay()
    {
        Debug.Log($"Waiting {delayBeforeLoad} seconds before loading scene...");
        yield return new WaitForSeconds(delayBeforeLoad);
        LoadScene();
    }

    private IEnumerator PlayWithTransition()
    {
        if (delayBeforeLoad > 0)
        {
            yield return new WaitForSeconds(delayBeforeLoad);
        }
        
        Debug.Log($"Starting transition to: {firstSceneName}");
        sceneTransition.changeScene(firstSceneName);
    }

    // Optional: Quit the game (useful for a Quit button)
    public void QuitGame()
    {
        // Play sound effect
        if (playAudioOnClick && audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }

        Debug.Log("Quitting game...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // Optional: Load a specific scene by name
    public void LoadSceneByName(string sceneName)
    {
        // Play sound effect
        if (playAudioOnClick && audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }

        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}