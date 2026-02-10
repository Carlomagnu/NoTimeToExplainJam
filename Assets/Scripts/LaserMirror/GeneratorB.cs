using UnityEngine;

using UnityEngine;

public class GeneratorB : MonoBehaviour, ILaserReceiver
{
    public GeneratorA generatorA;

    [Header("State")]
    public bool lockedOn = false;

    [Header("VFX")]
    public ElectrifiedVFX vfx;

    [Tooltip("If true: effect stays on after lock. If false: only shows while being hit by laser.")]
    public bool keepEffectOnWhenLocked = true;

    [Tooltip("How long the 'being hit' effect lingers after the laser stops hitting (prevents flicker).")]
    public float hitHoldSeconds = 0.1f;

    float _hitTimer;
    bool _wasElectrified;

    public void SetLaserActive(bool active)
    {
        if (active)
            _hitTimer = hitHoldSeconds;

        // Lock condition: must be hit AND GenA must be active
        if (active && !lockedOn && generatorA != null && generatorA.IsActive)
        {
            lockedOn = true;
            Debug.Log("Generator B LOCKED ON");
        }
    }

    void Update()
    {
        if (_hitTimer > 0f)
            _hitTimer -= Time.deltaTime;

        bool beingHit = _hitTimer > 0f;

        // Decide whether VFX should be on:
        bool electrified = lockedOn
            ? keepEffectOnWhenLocked
            : beingHit;

        if (vfx != null && electrified != _wasElectrified)
            vfx.SetElectrified(electrified);

        _wasElectrified = electrified;
    }
}