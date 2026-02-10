using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheaterBTN : MonoBehaviour, IInteractable
{
    [SerializeField]
    TheaterStateMachine theaterStateMachine;

    public void Interact(PlayerInteract interactor)
    {
        theaterStateMachine.nextStateBtn();
    }
}
