using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class ChargeableCube : MonoBehaviour, IInteractable
{
    public float chargeDuration = 7f;
    private float _chargeTimer;

    public bool IsCharged => _chargeTimer > 0f;

    void Update()
    {
        if (_chargeTimer > 0f)
            _chargeTimer -= Time.deltaTime;
    }

    public void Interact(PlayerInteract player)
    {
        Debug.Log(this.name + " just got picked up by: " + player.name);
        player.PickUp(gameObject);
    }

    void OnTriggerStay(Collider other)
    {
        var water = other.GetComponentInParent<WaterVolume>();
        if (water != null && water.waterOn && water.electrified)
        {
            _chargeTimer = chargeDuration;
        }
    }

    public bool ConsumeCharge()
    {
        if (!IsCharged) return false;
        _chargeTimer = 0f;
        return true;
    }
}

