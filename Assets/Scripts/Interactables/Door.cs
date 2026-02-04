using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{

    [Header("Door Parts")]
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;
    [SerializeField] private Collider interactionZone;

    [Header("Door Movement")]
    [SerializeField] private float slideDistance = 5f;
    [SerializeField] private float slideSpeed = 2f;

    [Header("Auto Close")]
    [SerializeField] private float autoCloseDelay = 2f;

    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;
    private Vector3 leftOpenPos;
    private Vector3 rightOpenPos;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact(PlayerInteract player)
    {
        Debug.Log(this.name + " is now opening: " + player.name);
        StartCoroutine(DoorSequence());
    }

    void Awake()
    {
        leftClosedPos = leftDoor.localPosition;
        rightClosedPos = rightDoor.localPosition;

        leftOpenPos = leftClosedPos + Vector3.left * slideDistance;
        rightOpenPos = rightClosedPos + Vector3.right * slideDistance;
    }

    private IEnumerator DoorSequence()
    {
        interactionZone.enabled = false;

        // Open doors
        yield return MoveDoors(leftClosedPos, rightClosedPos, leftOpenPos, rightOpenPos);

        // Wait and close doors

        yield return new WaitForSeconds(autoCloseDelay);
        yield return MoveDoors(leftOpenPos,rightOpenPos,leftClosedPos,rightClosedPos);

        interactionZone.enabled = true;
    }



    private IEnumerator MoveDoors(Vector3 leftStart, Vector3 rightStart, Vector3 leftTarget, Vector3 rightTarget)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * slideSpeed;

            leftDoor.localPosition =
                Vector3.Lerp(leftStart, leftTarget, t);

            rightDoor.localPosition =
                Vector3.Lerp(rightStart, rightTarget, t);

            yield return null;
        }

        leftDoor.localPosition = leftTarget;
        rightDoor.localPosition = rightTarget;
    }
}
