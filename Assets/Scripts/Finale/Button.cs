using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour, IInteractable
{
    [SerializeField]
    FinalTheaterSequence theaterStateMachine;

    public void Interact(PlayerInteract interactor)
    {
        theaterStateMachine.StartFinalSequence();
    }
}
