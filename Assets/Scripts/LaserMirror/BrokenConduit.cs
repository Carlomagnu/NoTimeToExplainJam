using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class BrokenConduit : MonoBehaviour, ILaserReceiver
{
    public WaterVolume water;
    public float holdSeconds = 0.2f;

    private float _timer;

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
    }
}
