using UnityEngine;

public class Dome : MonoBehaviour
{
    [SerializeField] float pullDistance = 0.3f; // how far inside the dome to place the player

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Find direction from player → dome center
            Vector3 directionToCenter = (transform.position - other.transform.position).normalized;

            // Teleport the player slightly inside the dome
            other.transform.position = new Vector3(transform.position.x + directionToCenter.x * pullDistance, other.transform.position.y+0.3f, transform.position.z + directionToCenter.z * pullDistance);
        }
    }
}

