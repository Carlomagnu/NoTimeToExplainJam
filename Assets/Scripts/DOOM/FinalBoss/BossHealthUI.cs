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

    private float screenWidth;
    private float remainingDistance = 0f;

    private RectTransform currentBar;


    private float currentY = 0f;

    void Start()
    {
        negativeBottomLine.SetActive(false);
        screenWidth = Screen.width;
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

        // Start empty (width = 0)
        currentBar.sizeDelta = new Vector2(0, currentBar.sizeDelta.y);

        // Ensure pivot is correct (right side)
        currentBar.pivot = new Vector2(1f, 0.5f);
        currentBar.anchorMin = new Vector2(1f, 0.5f);
        currentBar.anchorMax = new Vector2(1f, 0.5f);
        currentBar.anchoredPosition = Vector2.zero;
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


