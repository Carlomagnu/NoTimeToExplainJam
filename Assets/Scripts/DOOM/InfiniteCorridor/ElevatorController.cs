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
    [SerializeField] AudioClip gateOpen;
    [SerializeField] AudioSource playerBackground;

    //Getting in
    [SerializeField] GameObject restrict;

    [Header("Lift Bars")]
    [SerializeField] Transform liftBars;
    [SerializeField] float barsOpenHeight = 2f;
    [SerializeField] float barsSpeed = 2f;
    private Vector3 barsClosedPos;
    private Vector3 barsOpenPos;
    private bool openingBars = false;
    [SerializeField] BossMusic bossMusic;
    [SerializeField] MeshCollider arenaCollider;
    [SerializeField] SphereCollider Dome;

    public bool isDescending;
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
        if (openingBars)
            OpenBars();
    }

    private void Awake()
    {
        transform.position = topPoint.position;
        restrict.SetActive(false);

        // Store bar positions
        barsClosedPos = liftBars.localPosition;
        barsOpenPos = barsClosedPos + Vector3.up * barsOpenHeight;
        
    }

    private void Start()
    {
        arenaCollider.enabled = false;
        Dome.enabled = false;
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
            2f * Time.deltaTime
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
        openingBars = true;
        isAscending = false;
        speaker.Stop();
        speaker.PlayOneShot(gateOpen);
        playerBackground.Stop();
        PlayDoom();
        arenaCollider.enabled = true;
        Dome.enabled = true;
        BossHealthUIController.Instance.ShowUI();
    }

    void OpenBars()
    {
        liftBars.localPosition = Vector3.MoveTowards(
            liftBars.localPosition,
            barsOpenPos,
            barsSpeed * Time.deltaTime
        );

        if (Vector3.Distance(liftBars.localPosition, barsOpenPos) < 0.01f)
        {
            openingBars = false;
            Debug.Log("Bars opened");
        }
    }

    private void PlayDoom()
    {
        bossMusic.PlayDoomMusic();
    }
}
