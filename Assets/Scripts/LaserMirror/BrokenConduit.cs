using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

using UnityEngine;

public class BrokenConduit : MonoBehaviour, ILaserReceiver
{

    // --- LOOPING ELECTRIC AUDIO ---
[SerializeField] private AudioSource _loopSource;
[SerializeField] private AudioClip electricLoop;
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

        set_electric_loop(energized);
        _wasEnergized = energized;

    }
    protected void set_electric_loop(bool on)
{
    if (electricLoop == null) return;

    if (_loopSource == null)
    {
        _loopSource = GetComponent<AudioSource>();
        if (_loopSource == null)
            _loopSource = gameObject.AddComponent<AudioSource>();

        _loopSource.playOnAwake = false;
        _loopSource.loop = true;
        _loopSource.spatialBlend = 1f; // 3D sound
        _loopSource.clip = electricLoop;
    }

    if (on)
    {
        if (!_loopSource.isPlaying)
            _loopSource.Play();
    }
    else
    {
        if (_loopSource.isPlaying)
            _loopSource.Stop();
    }
}
}