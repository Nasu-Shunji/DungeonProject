using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Player")]

    //PlayerのHPを減らすための参照
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Attack")]

    //この距離以内なら攻撃状態へ切り替える
    [SerializeField] private float attackDistance = 1.5f;

    //1回の攻撃で与えるダメージ
    [SerializeField] private int attackDamage = 20;

    //次の攻撃までの待ち時間
    [SerializeField] private float attackInterval = 1f;

    //次に攻撃できるまでの残り時間
    private float attackTimer;

    //EnemyMoverが攻撃距離を確認するためのプロパティ
    public float AttackDistance => attackDistance;

    private void Update()
    {
        //攻撃後の待ち時間が残っていれば、毎フレーム経過時間分だけ減らす(1フレームが約0.016秒なら1 - 0.016・・と続く)
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    public void TryAttack()
    {
        //PlayerHealthが設定されていなければ処理しない
        if (playerHealth == null)
        {
            return;
        }

        //次の攻撃までの待ち時間が残っていれば処理しない
        if (attackTimer > 0f)
        {
            return;
        }

        //Playerへダメージを与える
        playerHealth.TakeDamage(attackDamage);

        //次に攻撃できるまでの待ち時間を設定
        attackTimer = attackInterval;
    }
}