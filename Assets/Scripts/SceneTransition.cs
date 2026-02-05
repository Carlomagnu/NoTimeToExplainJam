using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField]
    Material mat;
    [SerializeField]
    bool transitionOnLoad = true;

    const float TRANSITION_TIME = 6f;
    /* 
        Not really semaphore but the transition animation needs to fully complete for all elements that should be "consumed" so if
        the lock is ever above 0 then the next level should not be loaded
    */
    private int transitionSemaphore = 0;
    private int animationSemaphore = 0;

    void Awake()
    {
        if (transitionOnLoad)
        {
            applyTransitionColor(true);
        }
    }

    void Start()
    {
        if (transitionOnLoad)
        {
            StartCoroutine(startAllTransitions(true));
        }
    }

    public void changeScene(string sceneName)
    {
        applyTransitionColor(false);
        StartCoroutine(startAllTransitions(false));
        StartCoroutine(loadScene(sceneName));
    }

    private IEnumerator loadScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while(transitionSemaphore > 0 || animationSemaphore > 0)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
    }

    private void applyTransitionColor(bool reverse = false)
    {
        GameObject[] gameObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach(GameObject gameObject in gameObjects)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if(renderer != null)
            {
                Material[] materials = renderer.materials;
                Material[] newMaterials = new Material[materials.Length+1];
                for(int i = 0; i < materials.Length; i++)
                {
                    newMaterials[i] = materials[i];
                }
                newMaterials[^1] = mat;

                // Why do i have to include this
                if (reverse)
                {
                    newMaterials[^1].SetFloat("_Op", 1f);
                }
                else
                {
                    newMaterials[^1].SetFloat("_Op", 0f);
                }


                renderer.materials = newMaterials;

            }
        }
    }

    // Reverse means starting with color fully applied then disappearing over time
    private IEnumerator startAllTransitions(bool reverse)
    {
        // Critical section
        transitionSemaphore++;
        GameObject[] gameObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach(GameObject gameObject in gameObjects)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if(renderer != null)
            {
                StartCoroutine(transitionEffect(gameObject, reverse));

                yield return new WaitForSeconds(0.3f);
            }
        }
        transitionSemaphore--;

    }

    private IEnumerator transitionEffect(GameObject gameObject, bool reverse)
    {
        //Critical sectcion
        animationSemaphore++;
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();

        if(renderer != null)
        {
            float timeElapsed = 0f;
            float val;
            Material material = renderer.materials[^1];

            while(timeElapsed <= TRANSITION_TIME)
            {
                float progress = timeElapsed / TRANSITION_TIME;

                if (reverse)
                {
                    val = Mathf.Lerp(1f, 0f, progress);
                }
                else
                {

                    val = Mathf.Lerp(0f, 1f, progress);
                }

                material.SetFloat("_Op", val);
                timeElapsed += Time.deltaTime;

                yield return null;
            }
        }
        animationSemaphore--;
    }
}
