using System;
using UnityEngine;

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

    //Playerがダメージを受けたことを外部へ通知するイベント(引数を渡さないためActionの<>は不要)
    public event Action Damaged;

    //Playerが死亡したことを外部へ通知するイベント
    public event Action Died;

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

        //Playerがダメージを受けたことを演出用スクリプトへ通知
        Damaged?.Invoke();

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
        //死亡処理を何度も実行しないようにする
        if (isDead)
        {
            return;
        }

        //Playerを死亡状態にする
        isDead = true;

        //Playerの移動処理を取得
        PlayerMovement playerMovement =
            GetComponent<PlayerMovement>();

        //PlayerMovementが存在する場合は操作を停止
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        //Playerが死亡したことをゲームオーバーUIへ通知
        Died?.Invoke();

        Debug.Log("Player died.");
    }
}