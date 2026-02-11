using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericInteract : MonoBehaviour, IInteractable
{
    [Header("Stacking Settings")]
    [SerializeField] private bool isStackable = false;
    [SerializeField] private float stackHeight = 1f; // Height of one block
    [SerializeField] private LayerMask stackableLayer; // Layer for stackable objects
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private bool showDebugGizmos = true;
    
    private GenericInteract topBlock = null; // Block on top of this one
    private GenericInteract bottomBlock = null; // Block below this one
    
    void Start()
    {
        // Auto-calculate stack height from collider if not set
        if (stackHeight == 0)
        {
            Collider col = GetComponent<Collider>();
            if (col)
            {
                stackHeight = col.bounds.size.y;
                DebugLog($"Auto-calculated stackHeight: {stackHeight}");
            }
            else
            {
                DebugLog("WARNING: No collider found and stackHeight is 0!", true);
            }
        }
        
        DebugLog($"Initialized - isStackable: {isStackable}, stackHeight: {stackHeight}, Layer: {LayerMask.LayerToName(gameObject.layer)}");
    }

    public void Interact(PlayerInteract player)
    {
        // Don't pick up if there's a block on top
        if (topBlock != null)
        {
            DebugLog($"Can't pick up {name} - {topBlock.name} is on top!", true);
            return;
        }
        
        DebugLog($"{name} picked up by {player.name}");
        
        // Break connection with block below
        if (bottomBlock != null)
        {
            DebugLog($"Breaking connection with {bottomBlock.name} below");
            bottomBlock.topBlock = null;
            bottomBlock = null;
        }
        else
        {
            DebugLog("No block below to disconnect from");
        }
        
        player.PickUp(gameObject);
    }

    void LateUpdate()
    {
        if (transform.parent != null)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
    
    // Called when this block is placed
    public void OnPlaced()
    {
        DebugLog("OnPlaced() called");
        
        if (!isStackable)
        {
            DebugLog("Not stackable - skipping stack detection");
            return;
        }
        
        // Check below for stackable objects
        RaycastHit hit;
        float rayDistance = stackHeight * 2f;
        
        DebugLog($"Raycasting down from {transform.position} for distance {rayDistance}");
        DebugLog($"StackableLayer value: {stackableLayer.value}");
        
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance, stackableLayer))
        {
            DebugLog($"Hit {hit.collider.name} at distance {hit.distance}");
            
            GenericInteract blockBelow = hit.collider.GetComponent<GenericInteract>();
            
            if (blockBelow != null)
            {
                DebugLog($"Found GenericInteract on {blockBelow.name}, isStackable: {blockBelow.isStackable}");
                
                if (blockBelow.isStackable)
                {
                    DebugLog($"Snapping to {blockBelow.name}");
                    SnapToBlock(blockBelow);
                }
                else
                {
                    DebugLog($"{blockBelow.name} is not stackable - skipping snap");
                }
            }
            else
            {
                DebugLog($"No GenericInteract component on {hit.collider.name}");
            }
        }
        else
        {
            DebugLog("No stackable object detected below");
        }
    }
    
    private void SnapToBlock(GenericInteract blockBelow)
    {
        Vector3 oldPosition = transform.position;
        
        // Position this block on top
        Vector3 targetPos = blockBelow.transform.position + Vector3.up * stackHeight;
        transform.position = targetPos;
        transform.rotation = blockBelow.transform.rotation;
        
        // Create stack relationship
        this.bottomBlock = blockBelow;
        blockBelow.topBlock = this;
        
        DebugLog($"STACKED: {name} on top of {blockBelow.name}");
        DebugLog($"Position change: {oldPosition} → {targetPos} (moved {Vector3.Distance(oldPosition, targetPos):F2} units)");
    }
    
    // Helper to find the top of a stack
    public GenericInteract GetTopOfStack()
    {
        GenericInteract current = this;
        int stackCount = 0;
        
        while (current.topBlock != null)
        {
            current = current.topBlock;
            stackCount++;
            
            if (stackCount > 100) // Prevent infinite loops
            {
                DebugLog("ERROR: Infinite loop detected in GetTopOfStack!", true);
                break;
            }
        }
        
        DebugLog($"Top of stack from {name} is {current.name} ({stackCount} blocks above)");
        return current;
    }
    
    // Helper to check if this block is at bottom of stack
    public bool IsBottomOfStack()
    {
        bool isBottom = bottomBlock == null;
        DebugLog($"{name} IsBottomOfStack: {isBottom}");
        return isBottom;
    }
    
    // Debug logging helper
    private void DebugLog(string message, bool isWarning = false)
    {
        if (!enableDebugLogs) return;
        
        string fullMessage = $"[GenericInteract - {name}] {message}";
        
        if (isWarning)
            Debug.LogWarning(fullMessage, this);
        else
            Debug.Log(fullMessage, this);
    }
    
    // Visual debugging
    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        
        // Draw stack height
        Gizmos.color = Color.cyan;
        Vector3 topPoint = transform.position + Vector3.up * stackHeight;
        Gizmos.DrawLine(transform.position, topPoint);
        Gizmos.DrawWireSphere(topPoint, 0.1f);
        
        // Draw connections
        if (topBlock != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, topBlock.transform.position);
        }
        
        if (bottomBlock != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, bottomBlock.transform.position);
        }
        
        // Draw raycast when checking below (only in Play mode)
        if (Application.isPlaying && isStackable)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector3.down * stackHeight * 2f);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;
        
        // Draw stack detection range when selected
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, stackHeight * 2f, 1f));
    }
}