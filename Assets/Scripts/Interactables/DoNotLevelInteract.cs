using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotInteract : MonoBehaviour, IInteractable
{
    public void Interact(PlayerInteract playerInteract)
    {
        DoNotDoStateManager stateManager = GameObject.FindAnyObjectByType<DoNotDoStateManager>();
        if(stateManager != null)
        {
            stateManager.nextState();
        }
    }
}
