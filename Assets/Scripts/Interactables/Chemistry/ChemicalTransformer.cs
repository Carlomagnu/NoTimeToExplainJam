using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChemicalTransformer : MonoBehaviour, IInteractable
{
    [SerializeField] private string deviceName = "Chemical Transformer";
    
    [Header("Transformation Target")]
    [SerializeField] private string targetCompoundName;
    [SerializeField] private ChemicalCompound.State targetState;
    [SerializeField] private int pHChange; // Add/subtract this value from current pH
    [SerializeField] private bool targetContainsCopper;
    [SerializeField] private Color colorChange = Color.white; // RGB offset to add to the base color
    [SerializeField] private string bottlefillMaterialName = "GPVFX_Bottle_D_Fill";

    [Header("Error Display")]
    [SerializeField] private TextMeshProUGUI errorMessageDisplay;
    [SerializeField] private float errorDisplayDuration = 3f;

    [Header("UI Feedback")]
    [SerializeField] private ItemDisplayUI itemDisplayUI;

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

        // Transform the held compound to match the target properties
        heldCompound.TransformCompound(
            targetCompoundName,
            targetState,
            newPH,
            targetContainsCopper,
            null
        );

        // Update the color of GPVFX_Bottle_Fill material
        MeshRenderer renderer = heldItem.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Material[] materials = renderer.materials;
            foreach (Material mat in materials)
            {
                if (mat.name.Contains(bottlefillMaterialName))
                {
                    // Get current color and add the color change
                    Color currentColor = mat.GetColor("_BaseColor");
                    Color newColor = currentColor + colorChange;
                    
                    // Clamp RGB values between 0-1
                    newColor.r = Mathf.Clamp01(newColor.r);
                    newColor.g = Mathf.Clamp01(newColor.g);
                    newColor.b = Mathf.Clamp01(newColor.b);
                    newColor.a = currentColor.a; // Preserve alpha
                    
                    mat.SetColor("_BaseColor", newColor);
                }
            }
        }

        // Show the updated item info in the UI
        if (itemDisplayUI != null)
        {
            itemDisplayUI.DisplayCompound(heldCompound);
        }

        Debug.Log($"{deviceName}: Successfully transformed compound to {targetCompoundName} (pH: {newPH})!");
    }

    private void ShowError(string errorMessage)
    {
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
}