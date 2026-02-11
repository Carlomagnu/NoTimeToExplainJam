using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bossNameText;
    [SerializeField] private Image healthFill;
    [SerializeField] private RectTransform negativeLine;
    [SerializeField] private float negativeLineHeightStep = 20f;

    private float negativeWidth = 0f;
    private float screenWidth;
    private float currentNegativeY = 0f;

    void Start()
    {
        screenWidth = Screen.width;
        currentNegativeY = negativeLine.anchoredPosition.y;
    }


    private float currentFill = 1f;
    private float targetFill = 1f;
    private float speed = 3f; // how fast the bar shrinks

    void Update()
    {
        // Smoothly interpolate toward target
        healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, targetFill, Time.deltaTime * speed);
    }

    public void SetBossName(string name)
    {
        bossNameText.text = name;
    }

    public void UpdateHealth(int current, int max)
    {
        targetFill = Mathf.Clamp01((float)current / max);
    }

    public void UpdateNegativeHealth(int negativeAmount)
    {
        // Convert negative health to positive distance
        float distance = Mathf.Abs(negativeAmount) * 5f; // 5 px per damage, tweak as needed

        // Extend the line
        negativeWidth = distance;

        // If it exceeds screen width, wrap around
        while (negativeWidth > screenWidth)
        {
            negativeWidth -= screenWidth;

            // Move the line up for the next wrap
            currentNegativeY += negativeLineHeightStep;

            // Reset Y if it goes too high
            if (currentNegativeY > 300f)
                currentNegativeY = 0f;
        }

        // Apply width and position
        negativeLine.sizeDelta = new Vector2(negativeWidth, negativeLine.sizeDelta.y);
        negativeLine.anchoredPosition = new Vector2(0, currentNegativeY);
    }

}
