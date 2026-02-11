using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorA : MonoBehaviour
{
    // --- LOOPING ELECTRIC AUDIO ---
[SerializeField] private AudioSource _loopSource;
[SerializeField] private AudioClip electricLoop;
    public float activeSeconds = 1000f;
    private float _timer;

    public bool IsActive => _timer > 0f;

    public void Activate()
    {
        _timer = activeSeconds;
        Debug.Log("Generator A ACTIVE");
    }

    void Update()
    {
        if (_timer > 0f)
            _timer -= Time.deltaTime;
        set_electric_loop(IsActive);
    
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

