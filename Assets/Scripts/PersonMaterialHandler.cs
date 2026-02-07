using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonMaterialHandler : MonoBehaviour
{
    [SerializeField]
    Texture[] textures;
    [SerializeField]
    Material mat;

    void Start()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Person");
        foreach(GameObject gameObject in gameObjects)
        {
            if (UnityEngine.Random.Range(0f, 1f) > 0.5)
            {
                continue;
            }


            Renderer renderer = gameObject.GetComponent<Renderer>();
            if(renderer != null)
            {
                Material[] arr = {mat};
                renderer.materials = arr;
                renderer.materials[0].SetTexture("_Person", generateRandomTexture());
            }
        }
    }

    Texture generateRandomTexture()
    {
        int randomIndex = UnityEngine.Random.Range(0, textures.Length);
        return textures[randomIndex];
    }
}
