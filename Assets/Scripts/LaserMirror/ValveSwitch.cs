using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class ValveSwitch : MonoBehaviour, IInteractable
{
    public WaterVolume water;

    public void Interact(PlayerInteract interactor)
    {
        if (water == null) return;
        water.SetWaterOn(!water.waterOn);
    }
}

