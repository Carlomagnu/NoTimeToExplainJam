using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp : MonoBehaviour, IInteractable
{
    [SerializeField]
    SceneTransition sceneTransition;
    [SerializeField]
    string nextScene;

    public void Interact(PlayerInteract interactor)
    {
        sceneTransition.changeScene(nextScene);
    }
}
