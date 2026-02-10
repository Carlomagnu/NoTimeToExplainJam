using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class WaterVolume : MonoBehaviour
{
    public bool waterOn = false;
    public bool electrified = false;

    [Header("Optional visuals")]
    public Renderer waterRenderer;

    void Awake()
    {
        if (waterRenderer == null) waterRenderer = GetComponent<Renderer>();
        ApplyVisuals();
    }

    public void SetWaterOn(bool on)
    {
        waterOn = on;
        if (!waterOn) electrified = false;
        ApplyVisuals();
    }

    public void SetElectrified(bool on)
    {
        electrified = on && waterOn;
        ApplyVisuals();
    }

    void ApplyVisuals()
    {
        if (waterRenderer != null)
            waterRenderer.enabled = waterOn;
    }
}

