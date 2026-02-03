using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interact Setup")]
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private Transform playerCamera;
    [SerializeField] 
    [Min(1)] private float hitRange = 3;

    private RaycastHit hit;

    [Header("Picking stuff up")]
    [SerializeField] private Transform pickUpParent;
    [SerializeField] private GameObject inHandItem;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hit.collider != null)
        {
            hit.collider.GetComponentInParent<Highlight>()?.ToggleHighlight(false);
        }
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, hitRange, interactLayer))
        {
            hit.collider.GetComponentInParent<Highlight>()?.ToggleHighlight(true);
        }

        // Press E to interact
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

        // Press Q to drop
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropItem();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f);
        Gizmos.DrawRay(playerCamera.position, playerCamera.forward* hitRange);
    }

    // Interact, code what the interactino does:
    // The class must inherit from IInteractable
    private void Interact()
    {
        if(hit.collider != null)
        {
            hit.collider.GetComponentInParent<IInteractable>().Interact(this);
        }
    }

    // If you want item to get picked up, define this in interactable
    // interactor.PickUp
    public void PickUp(GameObject item)
    {
        //If item already in hand, drop the item
        if (inHandItem)
        {
            DropItem();
        }


        inHandItem = item;
        item.transform.SetParent(pickUpParent, false);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        //Disabling physics and colliders
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Collider col = item.GetComponent<Collider>();
        if (col) col.enabled = false;
        return;
    }

    private void DropItem()
    {
        if (!inHandItem) return;

        GameObject item = inHandItem;

        item.transform.SetParent(null);
        inHandItem = null;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        Collider col = item.GetComponent<Collider>();
        if (col) col.enabled = true;

    }
}
