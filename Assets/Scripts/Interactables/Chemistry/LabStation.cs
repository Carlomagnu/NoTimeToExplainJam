using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabStation : MonoBehaviour, IInteractable
{
    public enum TransformationType
    {
        Dissolution,      // Copper salt + water -> copper solution
        Precipitation,    // Copper solution + base -> copper hydroxide
        Neutralization    // Copper hydroxide + acid -> neutral compound
    }

    [Header("Station Setup")]
    [SerializeField] private string stationName;
    [SerializeField] private TransformationType transformType;

    [Header("Required Input")]
    [SerializeField] private string requiredCompoundName;
    [SerializeField] private ChemicalCompound.State requiredState;
    [SerializeField] private ChemicalCompound.pH requiredPH;
    [SerializeField] private bool requiresCopper;

    [Header("Output")]
    [SerializeField] private string outputCompoundName;
    [SerializeField] private ChemicalCompound.State outputState;
    [SerializeField] private ChemicalCompound.pH outputPH;
    [SerializeField] private bool outputContainsCopper;
    [SerializeField] private Material outputMaterial;

    [Header("Feedback")]
    [SerializeField] private string feedbackMessage;

    private PlayerInteract lastInteractor;

    public void Interact(PlayerInteract player)
    {
        lastInteractor = player;

        // Check if player has an item
        if (player.GetHeldItem() == null)
        {
            Debug.Log($"{stationName}: You need to be holding an item!");
            return;
        }

        ChemicalCompound compound = player.GetHeldItem().GetComponent<ChemicalCompound>();
        if (compound == null)
        {
            Debug.Log($"{stationName}: The item you're holding is not a chemical compound!");
            return;
        }

        // Check if item matches requirements
        if (!MatchesRequirements(compound))
        {
            Debug.Log($"{stationName}: This compound cannot be processed here. {feedbackMessage}");
            return;
        }

        // Transform the compound
        compound.TransformCompound(outputCompoundName, outputState, outputPH, outputContainsCopper, outputMaterial);
        Debug.Log($"{stationName}: Compound successfully transformed to {outputCompoundName}!");
    }

    private bool MatchesRequirements(ChemicalCompound compound)
    {
        if (compound.CompoundName != requiredCompoundName)
            return false;

        if (compound.CurrentState != requiredState)
            return false;

        if (compound.CurrentPH != requiredPH)
            return false;

        if (requiresCopper && !compound.ContainsCopper)
            return false;

        return true;
    }
}