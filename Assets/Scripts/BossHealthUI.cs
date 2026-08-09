using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [Header("Boss")]
    //HPを表示するBoss
    [SerializeField] private EnemyHealth bossHealth;

    [Header("UI")]
    //Boss HP全体のPanel
    [SerializeField] private GameObject bossHealthPanel;

    //BossのHPを表示するSlider
    [SerializeField] private Slider healthSlider;

    //Boss名を表示するText
    [SerializeField] private TMP_Text bossNameText;

    private void Awake()
    {
        //ゲーム開始時はBoss HPを非表示
        if (bossHealthPanel != null)
        {
            bossHealthPanel.SetActive(false);
        }

        if (bossNameText != null)
        {
            bossNameText.text = "BOSS";
        }
    }

    private void OnEnable()
    {
        if (bossHealth == null)
        {
            return;
        }

        //BossのHPが変わったらHPバーを更新
        bossHealth.HealthChanged += UpdateHealthBar;

        //Bossがダメージを受けたらHPバーを表示
        bossHealth.Damaged += ShowBossHealth;

        //Bossが死亡したらHPバーを非表示
        bossHealth.Died += HideBossHealth;
    }

    private void Start()
    {
        if (bossHealth == null)
        {
            return;
        }

        //Bossの最大HPと現在HPをSliderへ反映
        UpdateHealthBar(
            bossHealth.CurrentHealth,
            bossHealth.MaxHealth
        );
    }

    private void OnDisable()
    {
        if (bossHealth == null)
        {
            return;
        }

        bossHealth.HealthChanged -= UpdateHealthBar;
        bossHealth.Damaged -= ShowBossHealth;
        bossHealth.Died -= HideBossHealth;
    }

    private void UpdateHealthBar(
        int currentHealth,
        int maxHealth
    )
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    private void ShowBossHealth()
    {
        if (bossHealthPanel != null)
        {
            bossHealthPanel.SetActive(true);
        }
    }

    private void HideBossHealth()
    {
        if (bossHealthPanel != null)
        {
            bossHealthPanel.SetActive(false);
        }
    }
}