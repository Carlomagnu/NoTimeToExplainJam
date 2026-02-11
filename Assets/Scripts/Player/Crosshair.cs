using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [Header("Crosshair Settings")]
    [SerializeField] private bool showCrosshair = true;
    [SerializeField] private Color crosshairColor = Color.white;
    [SerializeField] private float crosshairSize = 10f;
    [SerializeField] private CrosshairType crosshairType = CrosshairType.Dot;

    public enum CrosshairType
    {
        Dot,
        Cross,
        Circle
    }

    private GameObject crosshairObject;
    private Image crosshairImage;

    void Start()
    {
        CreateCrosshair();
    }

    void CreateCrosshair()
    {
        // Find or create a Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("CrosshairCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create the crosshair GameObject
        crosshairObject = new GameObject("Crosshair");
        crosshairObject.transform.SetParent(canvas.transform, false);

        // Add Image component
        crosshairImage = crosshairObject.AddComponent<Image>();
        
        // Create the appropriate texture based on type
        Texture2D texture = CreateCrosshairTexture();
        Sprite crosshairSprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );
        
        crosshairImage.sprite = crosshairSprite;
        crosshairImage.color = crosshairColor;

        // Set size
        RectTransform rectTransform = crosshairObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(crosshairSize, crosshairSize);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

        // Set visibility
        crosshairObject.SetActive(showCrosshair);
    }

    Texture2D CreateCrosshairTexture()
    {
        int size = 32;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        // Fill with transparent
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }

        int center = size / 2;

        switch (crosshairType)
        {
            case CrosshairType.Dot:
                // Create a filled circle (dot)
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                        if (distance <= size / 4)
                        {
                            pixels[y * size + x] = Color.white;
                        }
                    }
                }
                break;

            case CrosshairType.Cross:
                // Create a cross shape
                int thickness = 2;
                for (int i = 0; i < size; i++)
                {
                    // Horizontal line
                    for (int t = -thickness; t <= thickness; t++)
                    {
                        if (center + t >= 0 && center + t < size)
                        {
                            pixels[(center + t) * size + i] = Color.white;
                        }
                    }
                    // Vertical line
                    for (int t = -thickness; t <= thickness; t++)
                    {
                        if (center + t >= 0 && center + t < size)
                        {
                            pixels[i * size + (center + t)] = Color.white;
                        }
                    }
                }
                break;

            case CrosshairType.Circle:
                // Create a circle outline
                int thickness2 = 2;
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                        if (distance >= size / 4 - thickness2 && distance <= size / 4 + thickness2)
                        {
                            pixels[y * size + x] = Color.white;
                        }
                    }
                }
                break;
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    // Public methods to control crosshair
    public void ShowCrosshair()
    {
        showCrosshair = true;
        if (crosshairObject != null)
        {
            crosshairObject.SetActive(true);
        }
    }

    public void HideCrosshair()
    {
        showCrosshair = false;
        if (crosshairObject != null)
        {
            crosshairObject.SetActive(false);
        }
    }

    public void SetCrosshairColor(Color color)
    {
        crosshairColor = color;
        if (crosshairImage != null)
        {
            crosshairImage.color = color;
        }
    }

    public void SetCrosshairSize(float size)
    {
        crosshairSize = size;
        if (crosshairObject != null)
        {
            RectTransform rectTransform = crosshairObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(size, size);
        }
    }
}