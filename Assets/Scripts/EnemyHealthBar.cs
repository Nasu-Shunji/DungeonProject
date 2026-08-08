using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Enemy")]

    //HP情報を持っているEnemyHealth
    [SerializeField] private EnemyHealth enemyHealth;

    [Header("UI")]

    //EnemyのHPを表示するSlider
    [SerializeField] private Slider healthSlider;

    private void OnEnable()
    {
        //EnemyHealthが設定されていなければ処理しない
        if (enemyHealth == null)
        {
            return;
        }

        //EnemyのHPが変化したとき、UpdateHealthBarを実行するよう登録
        enemyHealth.HealthChanged += UpdateHealthBar;
    }

    private void Start()
    {
        //EnemyHealthのAwakeによるHP初期化が終わったあとで、ゲーム開始時のHPをHPバーへ表示
        //OnEnable()に入れると0からスタートしてしまうため
        UpdateHealthBar(
            enemyHealth.CurrentHealth,
            enemyHealth.MaxHealth
        );
    }

    private void OnDisable()
    {
        if (enemyHealth != null)
        {
            //HP変更イベントへの登録を解除
            enemyHealth.HealthChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar(
        int currentHealth,
        int maxHealth
    )
    {
        //Sliderの最大値をEnemyの最大HPにする
        healthSlider.maxValue = maxHealth;

        //Sliderの現在値をEnemyの現在HPにする
        healthSlider.value = currentHealth;
    }
}