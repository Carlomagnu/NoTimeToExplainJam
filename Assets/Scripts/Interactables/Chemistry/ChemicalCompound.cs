using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChemicalCompound : MonoBehaviour, IInteractable
{
    public enum State
    {
        Solid,
        Liquid
    }

    public enum pH
    {
        Acidic,
        Neutral,
        Basic
    }

    [Header("Compound Properties")]
    [SerializeField] private string compoundName;
    [SerializeField] private State state;
    [SerializeField] private pH pHLevel;
    [SerializeField] private bool containsCopper;

    [Header("Visual")]
    [SerializeField] private Color compoundColor;
    [SerializeField] private Material compoundMaterial;

    public string CompoundName => compoundName;
    public State CurrentState => state;
    public pH CurrentPH => pHLevel;
    public bool ContainsCopper => containsCopper;

    private void Start()
    {
        ApplyVisuals();
    }

    private void ApplyVisuals()
    {
        if (compoundMaterial != null)
        {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = compoundMaterial;
            }
        }
    }

    public void TransformCompound(string newCompoundName, State newState, pH newPH, bool newContainsCopper, Material newMaterial)
    {
        compoundName = newCompoundName;
        state = newState;
        pHLevel = newPH;
        containsCopper = newContainsCopper;
        compoundMaterial = newMaterial;

        ApplyVisuals();
        Debug.Log($"Compound transformed to: {compoundName}");
    }

    public void Interact(PlayerInteract player)
    {
        // This allows the compound to be picked up
        player.PickUp(gameObject);
    }
}