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
    
    [Header("Stacking")]
    [SerializeField] private float dropDistance = 2f; // How far to drop in front
    [SerializeField] private LayerMask stackDetectionLayer; // Layer to detect stacks
    [SerializeField] private float stackSnapRange = 1.5f; // How close to snap to stack
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private bool showDebugGizmos = true;
    
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
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropItem();
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos || playerCamera == null) return;
        
        Gizmos.color = new Color(1f, 0f, 0f);
        Gizmos.DrawRay(playerCamera.position, playerCamera.forward * hitRange);
    }
    
    private void OnDrawGizmos()
    {
        if (!showDebugGizmos || playerCamera == null) return;
        
        // Draw drop position and snap range when holding item
        if (inHandItem != null)
        {
            Vector3 dropPos = playerCamera.position + playerCamera.forward * dropDistance;
            
            // Drop position marker
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(dropPos, 0.2f);
            
            // Snap detection range
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(dropPos, stackSnapRange);
        }
    }
    
    private void Interact()
    {
        if(hit.collider != null)
        {
            DebugLog($"Interacting with {hit.collider.name}");
            hit.collider.GetComponentInParent<IInteractable>()?.Interact(this);
        }
        else
        {
            DebugLog("No collider hit");
        }
    }
    
    public void PickUp(GameObject item)
    {
        DebugLog($"PickUp called for {item.name}");
        
        if (inHandItem)
        {
            DebugLog("Already holding item, dropping first");
            DropItem();
        }
        
        inHandItem = item;
        
        Collider[] interactColliders = item.GetComponentsInChildren<Collider>();
        int disabledCount = 0;
        foreach (Collider c in interactColliders)
        {
            if (c.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            {
                c.enabled = false;
                disabledCount++;
            }
        }
        DebugLog($"Disabled {disabledCount} Interactable colliders");
        
        item.transform.SetParent(pickUpParent, false);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            DebugLog("Rigidbody made kinematic");
        }
        
        Collider col = item.GetComponent<Collider>();
        if (col)
        {
            col.enabled = false;
            DebugLog("Main collider disabled");
        }
    }
        private void DropItem()
    {
        if (!inHandItem)
        {
            DebugLog("DropItem called but no item in hand");
            return;
        }
        
        DebugLog($"=== DROPPING {inHandItem.name} ===");
        
        GameObject item = inHandItem;
        GenericInteract stackable = item.GetComponent<GenericInteract>();
        
        DebugLog($"Item has GenericInteract: {stackable != null}");
        
        // Calculate drop position
        Vector3 dropPosition = playerCamera.position + playerCamera.forward * dropDistance;
        DebugLog($"Drop position: {dropPosition}");
        
        // Check for nearby stacks to snap to
        DebugLog($"Searching sphere radius {stackSnapRange} at {dropPosition}");
        Collider[] nearbyObjects = Physics.OverlapSphere(dropPosition, stackSnapRange);
        DebugLog($"Found {nearbyObjects.Length} nearby colliders");
        
        GenericInteract nearestStack = null;
        float nearestDistance = float.MaxValue;
        
        for (int i = 0; i < nearbyObjects.Length; i++)
        {
            Collider col = nearbyObjects[i];
            
            if (col.gameObject == item)
            {
                DebugLog($"  [{i}] {col.name} - SKIPPED (is self)");
                continue;
            }
            
            GenericInteract potential = col.GetComponent<GenericInteract>();
            if (potential != null)
            {
                float distance = Vector3.Distance(dropPosition, col.transform.position);
                DebugLog($"  [{i}] {col.name} - GenericInteract found, distance: {distance:F2}");
                
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestStack = potential;
                    DebugLog($"    ^ NEW NEAREST!");
                }
            }
            else
            {
                DebugLog($"  [{i}] {col.name} - No GenericInteract");
            }
        }
        
        if (nearestStack != null)
        {
            DebugLog($"Nearest stack: {nearestStack.name} at {nearestDistance:F2} units");
        }
        else
        {
            DebugLog("No stack found nearby");
        }
        
        Collider[] interactColliders = item.GetComponentsInChildren<Collider>();
        int enabledCount = 0;
        foreach (Collider c in interactColliders)
        {
            if (c.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            {
                c.enabled = true;
                enabledCount++;
            }
        }
        DebugLog($"Re-enabled {enabledCount} Interactable colliders");
        
        item.transform.SetParent(null);
        inHandItem = null;
        
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            DebugLog("Rigidbody made non-kinematic");
        }
        
        Collider mainCol = item.GetComponent<Collider>();
        if (mainCol)
        {
            mainCol.enabled = true;
            DebugLog("Main collider re-enabled");
        }

        // Position the item
        if (nearestStack != null && stackable != null)
        {
            DebugLog("STACK MODE ACTIVATED");
            
            GenericInteract topOfStack = nearestStack.GetTopOfStack();
            DebugLog($"Top of stack: {topOfStack.name}");
            
            float stackHeight = stackable.GetComponent<Collider>().bounds.size.y;
            Vector3 stackPosition = topOfStack.transform.position + Vector3.up * stackHeight;
            
            DebugLog($"Stack height: {stackHeight}, placing at: {stackPosition}");
            
            item.transform.position = stackPosition;
            item.transform.rotation = topOfStack.transform.rotation;
            
            // Keep kinematic briefly when stacking
            if (rb)
            {
                rb.isKinematic = true;
            }
            
            stackable.OnPlaced();
            
            // Re-enable physics after a brief delay
            if (rb)
            {
                StartCoroutine(EnablePhysicsDelayed(rb, 0.2f));
            }
        }
        else
        {
            DebugLog("NORMAL DROP MODE");
            
            item.transform.position = dropPosition;
            
            if (stackable != null)
            {
                DebugLog("Calling OnPlaced for ground check");
                stackable.OnPlaced();
            }
            
            // Only apply downward velocity when NOT stacking
            if (rb)
            {
                rb.velocity = Vector3.down * 0.5f;
                DebugLog("Applied downward velocity");
            }
        }

        DebugLog("=== DROP COMPLETE ===");
    }
    private IEnumerator EnablePhysicsDelayed(Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (rb != null)
        {
            rb.isKinematic = false;
            DebugLog("Physics re-enabled after stack placement");
        }
    } 
    public GameObject GetHeldItem()
    {
        return inHandItem;
    }
    
    private void DebugLog(string message, bool isWarning = false)
    {
        if (!enableDebugLogs) return;
        
        string fullMessage = $"[PlayerInteract] {message}";
        
        if (isWarning)
            Debug.LogWarning(fullMessage, this);
        else
            Debug.Log(fullMessage, this);
    }
}