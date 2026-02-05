using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField]
    Material mat;

    const float TRANSITION_TIME = 6f;
    /* 
        Not really semaphore but the transition animation needs to fully complete for all elements that should be "consumed" so if
        the lock is ever above 0 then the next level should not be loaded
    */
    private int transitionSemaphore = 0;
    private int animationSemaphore = 0;

    public void changeScene(string sceneName)
    {
        StartCoroutine(applyTransitionColor());
        StartCoroutine(loadScene(sceneName));
    }

    private IEnumerator loadScene(string sceneName)
    {
        applyTransitionColor();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while(transitionSemaphore > 0 || animationSemaphore > 0)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
    }

    private IEnumerator applyTransitionColor()
    {
        // Critical section
        transitionSemaphore++;
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
                newMaterials[materials.Length] = mat;
                renderer.materials = newMaterials;

                StartCoroutine(transitionEffect(gameObject));

                yield return new WaitForSeconds(0.3f);
            }

        }
        transitionSemaphore--;
    }

    private IEnumerator transitionEffect(GameObject gameObject)
    {
        //Critical sectcion
        animationSemaphore++;
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();

        if(renderer != null)
        {
            float timeElapsed = 0f;
            Material material = renderer.materials[^1];

            while(timeElapsed <= TRANSITION_TIME)
            {
                float val = Mathf.Lerp(0f, 1f, timeElapsed / TRANSITION_TIME);
                Debug.Log(timeElapsed + gameObject.name);
                material.SetFloat("_Op", val);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
        animationSemaphore--;
    }
}
