using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] Transform topPoint;
    [SerializeField] Transform bottomPoint;
    [SerializeField] Transform absoluteTop;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 2f;

    [Header("Music")]
    [SerializeField] AudioSource speaker;
    [SerializeField] AudioClip elevatorMusic;

    //Getting in
    [SerializeField] GameObject restrict;

    private bool isDescending;
    private bool isAscending;

    void Update()
    {
        if (isDescending)
        {
            MoveDown();
        }
        if (isAscending)
        {
            MoveUp();
        }
    }

    private void Awake()
    {
        transform.position = topPoint.position;
        restrict.SetActive(false);
    }

    public void CallElevator()
    {
        isDescending = true;
        if (speaker && elevatorMusic)
            speaker.PlayOneShot(elevatorMusic);
        restrict.SetActive(true);
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
        restrict.SetActive(false);
        // Next phase later: enable interaction
    }

    public void MoveUp()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            absoluteTop.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position,
                             absoluteTop.position) < 0.01f)
        {
            OnArrivedAtTop();
        }
    }

    public void StartMoveUp()  
    {
        Debug.Log("Starting move up");

        isAscending = true;
    }

    void OnArrivedAtTop()
    {
        Debug.Log("Elevator reached upper level");

        // Open rails / doors
        // Trigger next scene / hallway
    }
}
