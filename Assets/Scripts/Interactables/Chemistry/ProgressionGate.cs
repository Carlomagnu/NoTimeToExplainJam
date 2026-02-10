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
    [SerializeField] private int requiredPH = 7;
    [SerializeField] private bool requireSpecificColor = true;
    [SerializeField] private Color requiredColor = new Color(95f/255f, 255f/255f, 151f/255f, 1f);
    [SerializeField] private string bottlefillMaterialName = "GPVFX_Bottle_D_Fill";

    [Header("Gate Reference")]
    [SerializeField] private GameObject gateToUnlock; // Reference to the actual gate prefab

    [Header("Lock State")]
    private bool isUnlocked = false;

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
            issues.Add($"has pH {compound.CurrentPH} but must be pH {requiredPH}");

        if (requireSpecificColor)
        {
            Color compoundColor = GetCompoundColor(compound.gameObject);
            if (!ColorsMatch(compoundColor, requiredColor))
                issues.Add($"has color {compoundColor} but must be {requiredColor}");
        }

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

    private Color GetCompoundColor(GameObject compound)
    {
        // Search for the bottle fill renderer to get its color
        MeshRenderer[] renderers = compound.GetComponentsInChildren<MeshRenderer>();
        
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.gameObject.name.Contains(bottlefillMaterialName))
            {
                Material[] materials = renderer.materials;
                
                foreach (Material mat in materials)
                {
                    // Get the BaseColor from the material
                    return mat.GetColor("_BaseColor");
                }
            }
        }
        
        // Return white if color not found
        return Color.white;
    }

    private bool ColorsMatch(Color color1, Color color2, float tolerance = 0.01f)
    {
        // Compare colors with a small tolerance for floating point precision
        return Mathf.Abs(color1.r - color2.r) < tolerance &&
               Mathf.Abs(color1.g - color2.g) < tolerance &&
               Mathf.Abs(color1.b - color2.b) < tolerance &&
               Mathf.Abs(color1.a - color2.a) < tolerance;
    }
}