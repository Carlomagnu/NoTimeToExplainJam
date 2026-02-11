using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bossNameText;
    [SerializeField] private Image healthFill;
    [SerializeField] private GameObject negativeBottomLine;
    [SerializeField] private RectTransform negativeBarsContainer;
    [SerializeField] private GameObject negativeBarPrefab;
    [SerializeField] private float barHeight = 10f;
    [SerializeField] private float barSpacing = 3f;

    private int barCount = 0;


    private float screenWidth;
    private float remainingDistance = 0f;

    private RectTransform currentBar;


    private float currentY = 0f;

    void Start()
    {
        negativeBottomLine.SetActive(false);
        screenWidth = Screen.width * 2;
        CreateNewBar();

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

    // Called when boss health goes below 0
    public void AddNegativeDamage(float pxAmount)
    {
        negativeBottomLine.SetActive(true);
        remainingDistance += pxAmount;

        // Spawn full bars if needed
        while (remainingDistance >= screenWidth)
        {
            FillCurrentBarCompletely();
            remainingDistance -= screenWidth;
            CreateNewBar();
        }

        // Update the partial bar
        UpdateCurrentBar(remainingDistance);
    }

    private void CreateNewBar()
    {
        GameObject obj = Instantiate(negativeBarPrefab, negativeBarsContainer);
        currentBar = obj.GetComponent<RectTransform>();

        // Start empty
        currentBar.sizeDelta = new Vector2(0, barHeight);

        // Position this bar manually
        float yOffset = barCount * (barHeight + barSpacing);
        currentBar.anchoredPosition = new Vector2(0, yOffset);

        barCount++;
    }


    private void FillCurrentBarCompletely()
    {
        currentBar.sizeDelta = new Vector2(screenWidth, currentBar.sizeDelta.y);
    }

    private void UpdateCurrentBar(float width)
    {
        currentBar.sizeDelta = new Vector2(width, currentBar.sizeDelta.y);
    }


}


