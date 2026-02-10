using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChemicalTransformer : MonoBehaviour, IInteractable
{
    public enum ReagentType
    {
        None,
        Copper,
        Base,
        Acid
    }

    [SerializeField] private string deviceName = "Chemical Transformer";
    
    
    [Header("Transformation Target")]
    [SerializeField] private string targetCompoundName;
    [SerializeField] private bool applyTargetState = true;
    [SerializeField] private ChemicalCompound.State targetState;
    [SerializeField] private int pHChange; // Add/subtract this value from current pH
    [SerializeField] private bool applyTargetContainsCopper = true;
    [SerializeField] private bool targetContainsCopper;
    [SerializeField] private bool applyColorChange = true;
    [SerializeField] private Color colorChange = Color.white; // RGB offset to add to the base color
    [SerializeField] private string bottlefillMaterialName = "GPVFX_Bottle_D_Fill";
    
    [Header("Conditional Reactions")]
    [SerializeField] private bool useConditionalReactions = false;
    [SerializeField] private string conditionalCompoundName;
    [SerializeField] private bool applyConditionalState = true;
    [SerializeField] private ChemicalCompound.State conditionalTargetState;
    [SerializeField] private int conditionalPHChange = 0;
    [SerializeField] private bool applyConditionalContainsCopper = false;
    [SerializeField] private bool conditionalContainsCopper;
    [SerializeField] private bool applyConditionalColorChange = true;
    [SerializeField] private Color conditionalColorChange = Color.white;
    [SerializeField] private bool requireCopperForConditional = false;
    [SerializeField] private ChemicalCompound.State requiredStateForConditional = ChemicalCompound.State.Liquid;
    [SerializeField] private bool requireSpecificPHForConditional = false;
    [SerializeField] private int requiredPHForConditional = 7;

    [Header("Error Display")]
    [SerializeField] private TextMeshProUGUI errorMessageDisplay;
    [SerializeField] private float errorDisplayDuration = 3f;

    [Header("UI Feedback")]
    [SerializeField] private ItemDisplayUI itemDisplayUI;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip conditionalReactionSound;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private bool playSuccessSound = true;
    [SerializeField] private bool playConditionalSound = true;
    [SerializeField] private bool playErrorSound = true;

    private Coroutine errorFadeCoroutine;

    public void Interact(PlayerInteract player)
    {
        GameObject heldItem = player.GetHeldItem();

        if (heldItem == null)
        {
            ShowError("You need to be holding a chemical compound!");
            return;
        }

        ChemicalCompound heldCompound = heldItem.GetComponent<ChemicalCompound>();
        if (heldCompound == null)
        {
            ShowError("The item you're holding is not a chemical compound!");
            return;
        }

        // Calculate new pH with limits (0-14)
        int newPH = Mathf.Clamp(heldCompound.CurrentPH + pHChange, 0, 14);

        // Check if conditional reaction should occur
        string effectiveCompoundName = targetCompoundName;
        ChemicalCompound.State effectiveState = heldCompound.CurrentState; // Preserve current state by default
        Color effectiveColorChange = colorChange;
        bool effectiveContainsCopper = heldCompound.ContainsCopper; // Preserve current state by default
        bool shouldApplyColorChange = applyColorChange;
        bool isConditionalReaction = false;
        
        if (useConditionalReactions && CheckConditionalReactionConditions(heldCompound))
        {
            Debug.Log($"{deviceName}: Conditional reaction triggered!");
            effectiveCompoundName = conditionalCompoundName;
            effectiveColorChange = conditionalColorChange;
            shouldApplyColorChange = applyConditionalColorChange;
            newPH = Mathf.Clamp(heldCompound.CurrentPH + conditionalPHChange, 0, 14);
            isConditionalReaction = true;
            
            if (applyConditionalState)
            {
                effectiveState = conditionalTargetState;
            }
            
            if (applyConditionalContainsCopper)
            {
                effectiveContainsCopper = conditionalContainsCopper;
            }
        }
        else
        {
            if (applyTargetState)
            {
                effectiveState = targetState;
            }
            
            if (applyTargetContainsCopper)
            {
                effectiveContainsCopper = targetContainsCopper;
            }
        }

        // Transform the held compound to match the target properties
        heldCompound.TransformCompound(
            effectiveCompoundName,
            effectiveState,
            newPH,
            effectiveContainsCopper,
            null
        );

        // Update the color only if enabled
        if (shouldApplyColorChange)
        {
            UpdateBottleColor(heldItem, effectiveColorChange);
        }

        // Show the updated item info in the UI
        if (itemDisplayUI != null)
        {
            itemDisplayUI.DisplayCompound(heldCompound);
        }

        // Play appropriate success audio
        if (audioSource != null)
        {
            if (isConditionalReaction && playConditionalSound && conditionalReactionSound != null)
            {
                audioSource.PlayOneShot(conditionalReactionSound);
            }
            else if (playSuccessSound && successSound != null)
            {
                audioSource.PlayOneShot(successSound);
            }
        }

        Debug.Log($"{deviceName}: Successfully transformed compound to {effectiveCompoundName} (pH: {newPH})!");
    }

    private bool CheckConditionalReactionConditions(ChemicalCompound compound)
    {
        // Check if the required conditions for the conditional reaction are met
        if (requireCopperForConditional && !compound.ContainsCopper)
        {
            return false;
        }

        if (compound.CurrentState != requiredStateForConditional)
        {
            return false;
        }

        if (requireSpecificPHForConditional && compound.CurrentPH != requiredPHForConditional)
        {
            return false;
        }

        return true;
    }

    private void ShowError(string errorMessage)
    {
        // Play error sound
        if (audioSource != null && playErrorSound && errorSound != null)
        {
            audioSource.PlayOneShot(errorSound);
        }

        if (errorMessageDisplay == null)
        {
            Debug.Log($"{deviceName}: {errorMessage}");
            return;
        }

        errorMessageDisplay.text = errorMessage;
        errorMessageDisplay.alpha = 1f;

        if (errorFadeCoroutine != null)
        {
            StopCoroutine(errorFadeCoroutine);
        }
        errorFadeCoroutine = StartCoroutine(FadeOutError());
    }

    private IEnumerator FadeOutError()
    {
        yield return new WaitForSeconds(errorDisplayDuration);

        if (errorMessageDisplay != null)
        {
            float fadeDuration = 0.5f;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                errorMessageDisplay.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }

            errorMessageDisplay.alpha = 0f;
            errorMessageDisplay.text = "";
        }
    }

    private void UpdateBottleColor(GameObject heldItem, Color colorToApply)
    {
        // Search for the bottle fill renderer by its game object name
        MeshRenderer[] renderers = heldItem.GetComponentsInChildren<MeshRenderer>();
        
        foreach (MeshRenderer renderer in renderers)
        {
            // Check if this renderer's game object matches the bottle fill object name
            if (renderer.gameObject.name.Contains(bottlefillMaterialName))
            {
                Material[] materials = renderer.materials;
                
                for (int i = 0; i < materials.Length; i++)
                {
                    Material mat = materials[i];
                    
                    // Create a material instance so changes don't affect other instances
                    Material matInstance = new Material(mat);
                    
                    // Get the current color to preserve alpha
                    Color currentColor = matInstance.GetColor("_BaseColor");
                    
                    // Set the color directly (instead of adding to it)
                    Color newColor = colorToApply;
                    newColor.a = currentColor.a; // Preserve alpha
                    
                    matInstance.SetColor("_BaseColor", newColor);
                    materials[i] = matInstance;
                    
                    Debug.Log($"{deviceName}: Changed bottle color to {newColor}");
                }
                
                // Apply modified materials back to the renderer
                renderer.materials = materials;
                return; // Found and updated the bottle fill, no need to continue
            }
        }
        
        Debug.LogWarning($"{deviceName}: Could not find renderer named '{bottlefillMaterialName}' on {heldItem.name}");
    }
}