using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDisplayUI : MonoBehaviour
{
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private TextMeshProUGUI displayText; // UI text to show item info
    [SerializeField] private float displayDuration = 3f; // How long to show the info

    private GameObject lastHeldItem;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        if (displayText != null)
        {
            displayText.text = "";
        }
    }

    private void Update()
    {
        if (playerInteract == null)
            return;

        GameObject currentItem = playerInteract.GetHeldItem();

        // Check if the held item changed
        if (currentItem != lastHeldItem)
        {
            lastHeldItem = currentItem;

            if (currentItem != null)
            {
                ShowItemInfo(currentItem);
            }
            else
            {
                HideItemInfo();
            }
        }
    }

    private void ShowItemInfo(GameObject item)
    {
        ChemicalCompound compound = item.GetComponent<ChemicalCompound>();

        if (compound == null)
        {
            Debug.Log("Item does not have ChemicalCompound component");
            return;
        }

        DisplayCompound(compound);
    }

    private void HideItemInfo()
    {
        if (displayText != null)
        {
            displayText.text = "";
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
    }

    private IEnumerator FadeOutText()
    {
        yield return new WaitForSeconds(displayDuration);

        if (displayText != null)
        {
            float fadeDuration = 0.5f;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                displayText.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }

            displayText.alpha = 0f;
            displayText.text = "";
        }
    }

    public void DisplayCompound(ChemicalCompound compound)
    {
        if (compound == null)
        {
            Debug.Log("DisplayCompound: Compound is null");
            return;
        }

        Debug.Log($"DisplayCompound called for: {compound.CompoundName}");

        // Build the display text
        string info = $"";
        info += $"?? Level: {compound.CurrentPH}";

        Debug.Log($"Built info text: {info}");

        if (displayText != null)
        {
            displayText.text = info;
            displayText.alpha = 1f;
            Debug.Log("Text set successfully");
        }
        else
        {
            Debug.Log("ERROR: displayText is null!");
        }

        // Fade out after duration
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOutText());
    }
}