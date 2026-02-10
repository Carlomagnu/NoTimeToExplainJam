using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    [Header("Teleport Target")]
    [SerializeField] Transform thisDoor;
    [SerializeField] Transform destinationDoor;

    private bool canTeleport = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!canTeleport) return;
        if (!other.CompareTag("Player")) return;

        Vector3 offset =
            destinationDoor.position -
            thisDoor.position;

        other.transform.position += new Vector3(offset.x, offset.y, offset.z);
        PosterState[] posters =
        FindObjectsOfType<PosterState>();

        foreach (var poster in posters)
        {
            poster.RandomizeState();
        }
    }
}