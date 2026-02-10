using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElevatorInteract : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] Transform playerSnapPoint;
    [SerializeField] ElevatorController elevator;


    public void Interact(PlayerInteract player)
    {
        if (elevator.isDescending) return;

        EnterLift(player);
    }

    void EnterLift(PlayerInteract player)
    {
        Transform playerT = player.transform;

        // Snap position
        playerT.position = playerSnapPoint.position;

        Debug.Log("Player entered lift");
        StartCoroutine(StartLiftNextFrame());
    }

    IEnumerator StartLiftNextFrame()
    {
        yield return 1f;
        elevator.StartMoveUp();
    }
}
