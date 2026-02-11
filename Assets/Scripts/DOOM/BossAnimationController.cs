using UnityEngine;

public class Boss : MonoBehaviour
{
    public Animator anim;
    public int maxHealth = 100;
    private int currentHealth;

    private bool phase1Played = false;
    private bool phase2Played = false;
    private bool phase3Played = false;

    [Header("Phases and music")]
    [SerializeField] BossMusic speaker;

    [Header("HealthUI")]
    [SerializeField] BossHealthUI bossUI;
    [SerializeField] string bossName = "DAMNED DEREK";

    void Start()
    {
        currentHealth = maxHealth;
        bossUI.SetBossName(bossName);
        bossUI.UpdateHealth(currentHealth, maxHealth);
    }


    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        // UI
        bossUI.UpdateHealth(currentHealth, maxHealth);
        if (currentHealth < 0)
        {
            bossUI.UpdateNegativeHealth(currentHealth);
        }

        // Music shi
        if (currentHealth <= 99 && !phase1Played)
        {
            anim.SetTrigger("Trigger1");
            phase1Played = true;
        }

        if (currentHealth <= 50 && !phase2Played)
        {
            anim.SetTrigger("Trigger2");
            phase2Played = true;
            speaker.StopDoomAndDoEthereal();
        }

        if (currentHealth <= 0 && !phase3Played)
        {
            anim.SetTrigger("Trigger3");
            phase3Played = true;
        }

    }
}
