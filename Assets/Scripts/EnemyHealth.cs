using System.Collections;
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

    [Header("Death Effect")]

    //Enemyが消えるまでの時間
    [SerializeField] private float deathDuration = 0.4f;

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
        //死亡処理を何度も実行しないようにする
        if (isDead)
        {
            return;
        }

        //Enemyを死亡状態にする
        isDead = true;

        //Enemyが死亡したことをRoomEnemyManagerなどへ通知
        Died?.Invoke();

        //Enemyの移動処理を停止
        EnemyMover enemyMover =
            GetComponent<EnemyMover>();

        if (enemyMover != null)
        {
            enemyMover.enabled = false;
        }

        //Enemyの攻撃処理を停止
        EnemyAttack enemyAttack =
            GetComponent<EnemyAttack>();

        if (enemyAttack != null)
        {
            enemyAttack.enabled = false;
        }

        //死亡演出中にPlayerの攻撃が再び当たらないよう、EnemyのColliderを無効にする
        Collider enemyCollider =
            GetComponent<Collider>();

        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        Debug.Log("Enemy died.");

        //Enemyが小さくなって消える死亡演出を開始
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        //死亡開始時の大きさを保存
        Vector3 startScale =
            transform.localScale;

        //演出開始からの経過時間
        float elapsedTime = 0f;

        //deathDuration秒かけてEnemyを小さくする
        while (elapsedTime < deathDuration)
        {
            //毎フレーム経過時間を加算
            elapsedTime += Time.deltaTime;

            //経過時間を0～1の割合へ変換
            float t = Mathf.Clamp01(
                elapsedTime / deathDuration
            );

            //元の大きさから0へ少しずつ変化させる
            //Vector3.Lerp()は大きさを滑らかに変える
            transform.localScale =
                Vector3.Lerp(
                    startScale,
                    Vector3.zero,
                    t
                );

            //次のフレームまで待つ
            //これによって縮小処理をフレームごとに少しずつ進める
            yield return null;
        }

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

        //死亡演出が終了したEnemyを削除
        Destroy(gameObject);
    }
}