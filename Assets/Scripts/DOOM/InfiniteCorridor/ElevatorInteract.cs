using UnityEngine;

public class ElevatorInteract : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] Transform playerSnapPoint;
    [SerializeField] ElevatorController elevator;


    public void Interact(PlayerInteract player)
    {

        EnterLift(player);
    }

    void EnterLift(PlayerInteract player)
    {
        Transform playerT = player.transform;

        // Snap position
        playerT.position = playerSnapPoint.position;

        // Parent to lift so they move with it
        //playerT.SetParent(elevator.transform);

        // Disable movement
        //PlayerMovement movement =
        //    player.GetComponent<PlayerMovement>();

        //if (movement)
        //    movement.enabled = false;


        Debug.Log("Player entered lift");
        elevator.StartMoveUp();

        // Next step later:
        // elevator.StartAscent();
    }
}
