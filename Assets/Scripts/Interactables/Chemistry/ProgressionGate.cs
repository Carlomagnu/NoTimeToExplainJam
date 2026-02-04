using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionGate : MonoBehaviour, IInteractable
{
    [Header("Lock Settings")]
    [SerializeField] private string lockName = "Progression Lock";

    [Header("Requirements for Submission")]
    [SerializeField] private bool requireCopper = true;
    [SerializeField] private ChemicalCompound.State requiredState = ChemicalCompound.State.Solid;
    [SerializeField] private ChemicalCompound.pH requiredPH = ChemicalCompound.pH.Neutral;

    [Header("Gate Reference")]
    [SerializeField] private GameObject gateToUnlock; // Reference to the actual gate prefab

    [Header("Lock State")]
    [SerializeField] private bool isUnlocked = false;

    public bool IsUnlocked => isUnlocked;

    public void Interact(PlayerInteract player)
    {
        if (isUnlocked)
        {
            Debug.Log($"{lockName}: Already unlocked!");
            return;
        }

        GameObject heldItem = player.GetHeldItem();
        if (heldItem == null)
        {
            Debug.Log($"{lockName}: You need to submit an item!");
            return;
        }

        ChemicalCompound compound = heldItem.GetComponent<ChemicalCompound>();
        if (compound == null)
        {
            Debug.Log($"{lockName}: This is not a valid chemical compound!");
            return;
        }

        // Check requirements
        if (!EvaluateCompound(compound))
        {
            return; // Feedback is given in EvaluateCompound
        }

        // Success! Unlock the lock
        Debug.Log($"{lockName}: Correct compound! Lock disengaged...");
        RemoveItemFromPlayer(heldItem);
        UnlockGate();
    }

    private bool EvaluateCompound(ChemicalCompound compound)
    {
        List<string> issues = new List<string>();

        if (requireCopper && !compound.ContainsCopper)
            issues.Add("does not contain copper");

        if (compound.CurrentState != requiredState)
            issues.Add($"is {compound.CurrentState.ToString().ToLower()} but must be {requiredState.ToString().ToLower()}");

        if (compound.CurrentPH != requiredPH)
            issues.Add($"is {compound.CurrentPH.ToString().ToLower()} but must be {requiredPH.ToString().ToLower()}");

        if (issues.Count > 0)
        {
            string feedback = $"{lockName}: Sample rejected. The compound ";
            feedback += string.Join(", ", issues) + ".";
            Debug.Log(feedback);
            return false;
        }

        return true;
    }

    private void UnlockGate()
    {
        isUnlocked = true;

        if (gateToUnlock != null)
        {
            // Call Activate on the gate (works with Door or any object with Activate method)
            gateToUnlock.SendMessage("Activate", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void RemoveItemFromPlayer(GameObject item)
    {
        // Unparent the item
        item.transform.SetParent(null);

        // Re-enable physics and colliders
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        Collider col = item.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        // Destroy the item after submission (it's consumed)
        Destroy(item);
    }
}