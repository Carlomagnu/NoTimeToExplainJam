using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] Transform topPoint;
    [SerializeField] Transform bottomPoint;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 2f;

    [Header("Music")]
    [SerializeField] AudioSource speaker;
    [SerializeField] AudioClip elevatorMusic;

    private bool isDescending;

    void Update()
    {
        if (isDescending)
        {
            MoveDown();
        }
    }

    private void Awake()
    {
        transform.position = topPoint.position;
    }

    public void CallElevator()
    {
        isDescending = true;
        if (speaker && elevatorMusic)
            speaker.PlayOneShot(elevatorMusic);
    }

    void MoveDown()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            bottomPoint.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position,
                             bottomPoint.position) < 0.01f)
        {
            isDescending = false;
            OnArrived();
        }
    }

    void OnArrived()
    {
        Debug.Log("Elevator arrived at player floor");
        // Next phase later: enable interaction
    }
}
