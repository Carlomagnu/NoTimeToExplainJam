using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericInteract : MonoBehaviour, IInteractable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent != null)
        {
            Debug.Log(transform.localPosition);
        }
    }

    public void Interact(PlayerInteract player)
    {
        Debug.Log(this.name + " just go picked up by: " + player.name);
        player.PickUp(gameObject);
    }

    // MAKES IT NOT DRIFT AWAY
    void LateUpdate()
    {
        if (transform.parent != null)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
}
