using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElevatorInteract : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] Transform playerSnapPoint;
    [SerializeField] ElevatorController elevator;
    GameObject player;


    public void Interact(PlayerInteract player)
    {
        if (elevator.isDescending) return;

        EnterLift(player);
    }

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && Input.GetKeyDown(KeyCode.K))
        {
            EnterLiftJK(player);
        }
    }


    void EnterLiftJK(GameObject player)
    {
        Transform playerT = player.transform;

        // Snap position
        playerT.position = playerSnapPoint.position;

        Debug.Log("Player entered lift");
        StartCoroutine(StartLiftNextFrame());
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
        yield return 5f;
        elevator.StartMoveUp();
    }
}
