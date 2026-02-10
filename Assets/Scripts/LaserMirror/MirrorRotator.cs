using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorRotator : MonoBehaviour, IInteractable
{
    public float rotateDegrees = -15f;

    public void Interact(PlayerInteract interactor)
    {
        transform.Rotate(0f, rotateDegrees, 0f, Space.World);
    }
}
