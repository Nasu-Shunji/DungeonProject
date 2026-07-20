using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class PlayerHealthUI : MonoBehaviour
{
    //HP情報を持っているPlayerHealth
    [SerializeField] private PlayerHealth playerHealth;

    //このGameObjectに付いているTextMeshPro
    private TMP_Text healthText;

    private void Awake()
    {
        //同じGameObjectのTextMeshProを取得
        healthText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        if (playerHealth == null)
        {
            Debug.LogError(
                "PlayerHealth is not assigned.",
                this
            );

            enabled = false;
            return;
        }

        //HPが変化したとき、UpdateHealthTextを実行する、実行するメソッドとして登録
        playerHealth.HealthChanged += UpdateHealthText;

        //ゲーム開始時のHPも表示する
        UpdateHealthText(
            playerHealth.CurrentHealth,
            playerHealth.MaxHealth
        );
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            //このUIが無効になったらイベント登録を解除
            playerHealth.HealthChanged -= UpdateHealthText;
        }
    }

    private void UpdateHealthText(
        int currentHealth,
        int maxHealth
    )
    {
        //現在HPと最大HPを画面へ表示
        healthText.text =
            $"HP: {currentHealth} / {maxHealth}";
    }
}