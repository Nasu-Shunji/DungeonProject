using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]

    //Enemyの最大HP
    [SerializeField] private int maxHealth = 60;

    //Enemyの現在HP
    private int currentHealth;

    //死亡処理を何度も実行しないためのフラグ
    private bool isDead;

    [Header("Drop")]

    //Enemyが死亡したときに生成するアイテム
    [SerializeField] private GameObject dropPrefab;

    //Enemyの位置からどれだけずらして生成するか
    [SerializeField] private Vector3 dropOffset =
        new Vector3(0f, 0.5f, 0f);

    //Enemyがダメージを受けたことを外部へ通知するイベント
    public event Action Damaged;

    //Enemyの現在HPと最大HPが変化したことをUIへ通知するイベント
    public event Action<int, int> HealthChanged;

    //外部から現在HPを確認するためのプロパティ
    public int CurrentHealth => currentHealth;

    //外部から最大HPを確認するためのプロパティ
    public int MaxHealth => maxHealth;

    //Enemyが死亡したことを外部へ通知するイベント
    public event Action Died;

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

        //現在HPからダメージを引き、0未満にならないようにする
        currentHealth = Mathf.Max(
            currentHealth - damage,
            0
        );

        //HPが変化したことをHPバーへ通知
        HealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );

        //Enemyがダメージを受けたことを演出用スクリプトへ通知
        Damaged?.Invoke();

        Debug.Log(
            $"Enemy took {damage} damage. HP: {currentHealth}"
        );

        //HPが0になったら死亡処理
        if (currentHealth == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //死亡処理を一度だけ実行する
        isDead = true;

        Debug.Log("Enemy died.");

        //Enemyが死亡したことを外部へ通知
        Died?.Invoke();

        //ドロップするPrefabが設定されている場合
    if (dropPrefab != null)
    {
        //Enemyの位置に回復アイテムを生成、Prefabをゲーム中に生成する処理
        //何を生成するか, どこへ生成するか, 回転していない状態で生成
        Instantiate(
            dropPrefab,
            transform.position + dropOffset,
            Quaternion.identity
        );
    }

        //EnemyのGameObjectを削除
        Destroy(gameObject);
    }
}