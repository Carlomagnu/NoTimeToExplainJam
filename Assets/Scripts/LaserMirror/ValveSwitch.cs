using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;


public class ValveSwitch : MonoBehaviour, IInteractable
{
    // --- INTERACTION SFX ---
[SerializeField] private AudioSource _sfx;
[SerializeField] private AudioClip interactSound;
    public WaterVolume water;

    public void Interact(PlayerInteract interactor)
    {

        play_interact_sfx();
        if (water == null) return;
        water.SetWaterOn(!water.waterOn);
    }

    private void play_interact_sfx()
{
    if (interactSound == null) return;

    if (_sfx == null)
    {
        _sfx = GetComponent<AudioSource>();
        if (_sfx == null)
            _sfx = gameObject.AddComponent<AudioSource>();

        _sfx.playOnAwake = false;
        _sfx.loop = false;
        _sfx.mute = false;
        _sfx.volume = 1f;

        // 3D audio tuning (THIS is the usual fix)
        _sfx.spatialBlend = 1f;                 // 3D
        _sfx.rolloffMode = AudioRolloffMode.Linear;
        _sfx.minDistance = 2f;                  // full volume within 2m
        _sfx.maxDistance = 35f;                 // fades out by 35m
        _sfx.dopplerLevel = 0f;
    }

    _sfx.PlayOneShot(interactSound, 1f);
}
}

