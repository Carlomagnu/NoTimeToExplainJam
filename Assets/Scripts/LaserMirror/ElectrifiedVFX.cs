using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class ElectrifiedVFX : MonoBehaviour
{
    [Header("Assign one or more particle systems")]
    public ParticleSystem[] systems;

    [Header("Optional")]
    public Light glowLight;
    public float glowIntensityOn = 2f;

    bool _isOn;

    void Awake()
    {
        if (systems == null || systems.Length == 0)
            systems = GetComponentsInChildren<ParticleSystem>(true);

        if (glowLight != null)
            glowLight.intensity = 0f;

        // Start off
        SetElectrified(false, force: true);
    }

    public void SetElectrified(bool on, bool force = false)
    {
        if (!force && _isOn == on) return;
        _isOn = on;

        if (systems != null)
        {
            foreach (var ps in systems)
            {
                if (ps == null) continue;

                if (on)
                {
                    if (!ps.isPlaying) ps.Play(true);
                }
                else
                {
                    if (ps.isPlaying) ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
        }

        if (glowLight != null)
            glowLight.intensity = on ? glowIntensityOn : 0f;
    }
}
