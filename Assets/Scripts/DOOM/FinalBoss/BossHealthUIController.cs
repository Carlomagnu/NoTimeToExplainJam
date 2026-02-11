using UnityEngine;

public class BossHealthUIController : MonoBehaviour
{
    public static BossHealthUIController Instance;

    [SerializeField] GameObject uiRoot; // BossHealthUI object

    void Awake()
    {
        Instance = this;
        uiRoot.SetActive(false);
    }

    public void ShowUI()
    {
        uiRoot.SetActive(true);
    }

    public void HideUI()
    {
        uiRoot.SetActive(false);
    }
}

