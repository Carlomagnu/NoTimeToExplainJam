using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;


public class ChargeableCube : MonoBehaviour, IInteractable
{
    public float chargeDuration = 7f;
    public ElectrifiedVFX vfx;

    float _timer;
    bool _wasCharged;

    public bool IsCharged => _timer > 0f;

    void Update()
    {
        if (_timer > 0f)
            _timer -= Time.deltaTime;

        bool charged = IsCharged;
        if (vfx != null && charged != _wasCharged)
            vfx.SetElectrified(charged);

        _wasCharged = charged;
    }

    void OnTriggerStay(Collider other)
    {
        var water = other.GetComponentInParent<WaterVolume>();
        if (water != null && water.waterOn && water.electrified)
        {
            _timer = chargeDuration;
        }
    }

    public bool ConsumeCharge()
    {
        if (!IsCharged) return false;
        _timer = 0f;
        return true;
    }
    public void Interact(PlayerInteract player)
    {
        Debug.Log(this.name + " just got picked up by: " + player.name);
        player.PickUp(gameObject);
    }

    
}

