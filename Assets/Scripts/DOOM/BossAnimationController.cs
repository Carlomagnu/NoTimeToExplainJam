using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    public Animator anim;
    public int maxHealth = 100;
    private int currentHealth;

    private bool phase1Played = false;
    private bool phase2Played = false;
    private bool phase3Played = false;
    private float negativeAmount;

    [Header("Phases and music")]
    [SerializeField] BossMusic speaker;

    [Header("HealthUI")]
    [SerializeField] BossHealthUI bossUI;
    [SerializeField] string bossName = "DAMNED DEREK";

    //Final
    [SerializeField] GameObject finalUI;
    [SerializeField] SceneTransition transition;

    void Start()
    {
        currentHealth = maxHealth;
        bossUI.SetBossName(bossName);
        bossUI.UpdateHealth(currentHealth, maxHealth);
        negativeAmount = 20f;
        finalUI.SetActive(false);
    }


    public void TakeDamage(int amount)
    {
        if (currentHealth > 0)
        {
            amount /= 2;
        }
        currentHealth -= amount;
        // UI
        bossUI.UpdateHealth(currentHealth, maxHealth);
        //Negative damadge
        if (currentHealth < 0)
        {
            negativeAmount *= 1.1f;
            float px = Mathf.Abs(negativeAmount) * 5f;
            bossUI.AddNegativeDamage(px);
        }

        // Music shi
        if (currentHealth <= 99 && !phase1Played)
        {
            anim.SetTrigger("Trigger1");
            phase1Played = true;
        }

        if (currentHealth <= 0 && !phase2Played)
        {
            anim.SetTrigger("Trigger2");
            phase2Played = true;
            speaker.PlayImpactAndSilence();
        }

        if (currentHealth <= 0 && currentHealth > -100)
        {
            speaker.PlayImpactAndSilence();
        }

        if (currentHealth <= -100 && !phase3Played)
        {
            speaker.PlaySpeech();
            speaker.FadeInEthereal();
            phase3Played = true;
        }

        if (currentHealth <= -400)
        {
            speaker.PlayImpact();
            speaker.FadeOutAllSound();
            finalUI.SetActive(true);
            StartCoroutine(finalTransition());

        }

    }

    IEnumerator finalTransition()
    {
        yield return new WaitForSeconds(1f);
        transition.changeScene("TheaterENDING");
    }
}
