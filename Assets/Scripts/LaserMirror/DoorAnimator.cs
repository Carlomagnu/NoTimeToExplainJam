using System.Collections;
using UnityEngine;

public class GenGateDoor : MonoBehaviour, IInteractable
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

    [Header("Generators Required")]
    [SerializeField] private GeneratorA generatorA;
    [SerializeField] private GeneratorB generatorB;

    [Header("Audio")]
    [SerializeField] private AudioSource doorAudio;
    [SerializeField] private AudioClip doorSfx;

    [SerializeField] private SceneTransition transition;

    private Vector3 _leftClosed;
    private Vector3 _rightClosed;
    private Vector3 _leftOpen;
    private Vector3 _rightOpen;

    private bool _busy;

    void Awake()
    {
        _leftClosed = leftDoor.localPosition;
        _rightClosed = rightDoor.localPosition;

        _leftOpen = _leftClosed + Vector3.left * slideDistance;
        _rightOpen = _rightClosed + Vector3.right * slideDistance;
    }

    public void Interact(PlayerInteract player)
    {
        if (_busy) return;

        if (!gens_ok(out string reason))
        {
            Debug.Log($"[GenGateDoor] Locked: {reason}");
            return;
        }

        StartCoroutine(seq_open_close());
        transition.changeScene("Ferami Test");
    }

    private bool gens_ok(out string reason)
    {
        if (generatorA == null) { reason = "Missing GeneratorA reference."; return false; }
        if (generatorB == null) { reason = "Missing GeneratorB reference."; return false; }

        if (!generatorA.IsActive) { reason = "GeneratorA not active."; return false; }
        if (!generatorB.lockedOn) { reason = "GeneratorB not lockedOn."; return false; }

        reason = null;
        return true;
    }

    private IEnumerator seq_open_close()
    {
        _busy = true;
        if (interactionZone != null) interactionZone.enabled = false;

        play_sfx();
        yield return lerp_doors(_leftClosed, _rightClosed, _leftOpen, _rightOpen);

        yield return new WaitForSeconds(autoCloseDelay);

        play_sfx();
        yield return lerp_doors(_leftOpen, _rightOpen, _leftClosed, _rightClosed);

        if (interactionZone != null) interactionZone.enabled = true;
        _busy = false;
    }

    private void play_sfx()
    {
        if (doorAudio != null && doorSfx != null)
            doorAudio.PlayOneShot(doorSfx);
    }

    private IEnumerator lerp_doors(Vector3 leftStart, Vector3 rightStart, Vector3 leftTarget, Vector3 rightTarget)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * slideSpeed;

            if (leftDoor != null)
                leftDoor.localPosition = Vector3.Lerp(leftStart, leftTarget, t);

            if (rightDoor != null)
                rightDoor.localPosition = Vector3.Lerp(rightStart, rightTarget, t);

            yield return null;
        }

        if (leftDoor != null) leftDoor.localPosition = leftTarget;
        if (rightDoor != null) rightDoor.localPosition = rightTarget;
    }
}