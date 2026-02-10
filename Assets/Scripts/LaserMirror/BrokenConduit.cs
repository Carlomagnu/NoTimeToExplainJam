using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

using UnityEngine;

public class BrokenConduit : MonoBehaviour, ILaserReceiver
{
    public WaterVolume water;
    public float holdSeconds = 0.2f;

    public ElectrifiedVFX vfx;

    float _timer;
    bool _wasEnergized;

    public void SetLaserActive(bool active)
    {
        if (active) _timer = holdSeconds;
    }

    void Update()
    {
        if (_timer > 0f) _timer -= Time.deltaTime;
        bool energized = _timer > 0f;

        if (water != null)
            water.SetElectrified(energized);

        if (vfx != null && energized != _wasEnergized)
            vfx.SetElectrified(energized);

        _wasEnergized = energized;
    }
}