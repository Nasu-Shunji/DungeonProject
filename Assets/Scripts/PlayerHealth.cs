using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]

    //Playerの最大HP
    [SerializeField] private int maxHealth = 100;

    //現在のHP
    private int currentHealth;

    //死亡処理を何度も実行しないためのフラグ
    private bool isDead;

    //現在HPと最大HPが変化したことをUIへ通知するイベント
    public event Action<int, int> HealthChanged;

    //外部から現在HPを確認するためのプロパティ
    public int CurrentHealth => currentHealth;

    //外部から最大HPを確認するためのプロパティ
    public int MaxHealth => maxHealth;

    private void Awake()
    {
        //ゲーム開始時は最大HPにする
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        //0以下のダメージ、または死亡済みなら処理しない
        if (damage <= 0 || isDead)
        {
            return;
        }

        //HPを減らし、0未満にならないよう制限する
        currentHealth = Mathf.Max(
            currentHealth - damage,
            0
        );

        //現在HPが変わったことをUIへ通知, Invoke()で登録されているメソッド(UpdateHealthText)が実行される
        HealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );

        Debug.Log(
            $"Player took {damage} damage. HP: {currentHealth}"
        );

        //HPが0になったら死亡処理
        if (currentHealth == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //死亡状態にする
        isDead = true;

        //Playerの移動を停止する
        PlayerMovement playerMovement =
            GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        Debug.Log("Player died.");

        //少し待ってからDungeonを再読み込み
        StartCoroutine(ReloadSceneAfterDelay());
    }

    private IEnumerator ReloadSceneAfterDelay()
    {
        //1秒待つ
        yield return new WaitForSeconds(1f);

        //現在開いているSceneを最初から読み込む
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }
}