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

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip requirementMetSound;
    [SerializeField] private AudioClip requirementFailedSound;
    [SerializeField] private AudioClip successSound; // New success sound
    [SerializeField] private float soundDelay = 1f;

    [Header("Lock State")]
    private bool isUnlocked = false;

    public bool IsUnlocked => isUnlocked;

    private void Awake()
    {
        // Cache AudioSource if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

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

        // Start the evaluation coroutine
        StartCoroutine(EvaluateCompoundWithSound(compound, heldItem));
    }

    private IEnumerator EvaluateCompoundWithSound(ChemicalCompound compound, GameObject heldItem)
    {
        List<string> issues = new List<string>();
        bool allRequirementsMet = true;

        // Check copper requirement
        if (requireCopper)
        {
            if (compound.ContainsCopper)
            {
                Debug.Log($"{lockName}: ✓ Contains copper");
                PlaySound(requirementMetSound);
            }
            else
            {
                Debug.Log($"{lockName}: ✗ Does not contain copper");
                issues.Add("does not contain copper");
                allRequirementsMet = false;
                PlaySound(requirementFailedSound);
            }
            yield return new WaitForSeconds(soundDelay);
        }

        // Check state requirement
        if (compound.CurrentState == requiredState)
        {
            Debug.Log($"{lockName}: ✓ State is {requiredState.ToString().ToLower()}");
            PlaySound(requirementMetSound);
        }
        else
        {
            Debug.Log($"{lockName}: ✗ State is {compound.CurrentState.ToString().ToLower()} but must be {requiredState.ToString().ToLower()}");
            issues.Add($"is {compound.CurrentState.ToString().ToLower()} but must be {requiredState.ToString().ToLower()}");
            allRequirementsMet = false;
            PlaySound(requirementFailedSound);
        }
        yield return new WaitForSeconds(soundDelay);

        // Check pH requirement
        if (compound.CurrentPH == requiredPH)
        {
            Debug.Log($"{lockName}: ✓ pH is {requiredPH}");
            PlaySound(requirementMetSound);
        }
        else
        {
            Debug.Log($"{lockName}: ✗ pH is {compound.CurrentPH} but must be {requiredPH}");
            issues.Add($"has pH {compound.CurrentPH} but must be pH {requiredPH}");
            allRequirementsMet = false;
            PlaySound(requirementFailedSound);
        }
        yield return new WaitForSeconds(soundDelay);

        // Check color requirement
        if (requireSpecificColor)
        {
            Color compoundColor = GetCompoundColor(compound.gameObject);
            if (ColorsMatch(compoundColor, requiredColor))
            {
                Debug.Log($"{lockName}: ✓ Color matches");
                PlaySound(requirementMetSound);
            }
            else
            {
                Debug.Log($"{lockName}: ✗ Color is {compoundColor} but must be {requiredColor}");
                issues.Add($"has color {compoundColor} but must be {requiredColor}");
                allRequirementsMet = false;
                PlaySound(requirementFailedSound);
            }
            yield return new WaitForSeconds(soundDelay);
        }

        // Final result
        if (allRequirementsMet)
        {
            // Play success sound after all checks pass
            PlaySound(successSound);
            
            Debug.Log($"{lockName}: Correct compound! Lock disengaged...");
            RemoveItemFromPlayer(heldItem);
            UnlockGate();
        }
        else
        {
            string feedback = $"{lockName}: Sample rejected. The compound ";
            feedback += string.Join(", ", issues) + ".";
            Debug.Log(feedback);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
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