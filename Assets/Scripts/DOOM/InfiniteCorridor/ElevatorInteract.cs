using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ElevatorInteract : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] Transform playerSnapPoint;
    [SerializeField] ElevatorController elevator;

    private Transform playerT;


    public void Interact(PlayerInteract player)
    {
        if (elevator.isDescending) return;

        EnterLift(player);
    }

    void EnterLift(PlayerInteract player)
    {
        playerT = player.transform;

        // Snap position
        playerT.position = playerSnapPoint.position;

        Debug.Log("Player entered lift");
        StartCoroutine(StartLiftNextNextFrame());
    }

    IEnumerator StartLiftNextFrame()
    {
        yield return 1f;
        elevator.StartMoveUp();
    }

    IEnumerator StartLiftNextNextFrame()
    {
        yield return 5f;
        playerT.position = playerSnapPoint.position;
        elevator.StartMoveUp();
    }
}
