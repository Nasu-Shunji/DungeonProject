using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack")]

    //攻撃範囲の中心となる位置
    [SerializeField] private Transform attackPoint;

    //攻撃が届く範囲
    [SerializeField] private float attackRange = 1f;

    //1回の攻撃で与えるダメージ
    [SerializeField] private int attackDamage = 20;

    //次の攻撃までの待ち時間
    [SerializeField] private float attackInterval = 0.5f;

    //攻撃判定の対象となるLayer
    [SerializeField] private LayerMask enemyLayer;

    //次に攻撃できるまでの残り時間
    private float attackTimer;

    private void Update()
    {
        //攻撃後の待ち時間が残っている場合、毎フレーム経過時間分だけ減らす
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        //キーボードが取得できなければ処理しない
        if (Keyboard.current == null)
        {
            return;
        }

        //Fキーが押された瞬間でなければ処理しない
        if (!Keyboard.current.fKey.wasPressedThisFrame)
        {
            return;
        }

        //まだ攻撃の待ち時間が残っていれば攻撃しない
        if (attackTimer > 0f)
        {
            return;
        }

        Attack();
    }

    private void Attack()
    {
        //AttackPointを中心とした球形範囲内から、Enemy LayerのColliderをすべて取得
        Collider[] hitEnemies = Physics.OverlapSphere(
            attackPoint.position,
            attackRange,
            enemyLayer,
            QueryTriggerInteraction.Ignore
        );

        //攻撃範囲内にいたEnemyを順番に確認
        foreach (Collider hitEnemy in hitEnemies)
        {
            //当たったCollider、または親オブジェクトからEnemyHealthを探す
            EnemyHealth enemyHealth =
                hitEnemy.GetComponentInParent<EnemyHealth>();

            //EnemyHealthがあればダメージを与える
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }

        //次に攻撃できるまでの待ち時間を設定
        attackTimer = attackInterval;
    }

    private void OnDrawGizmosSelected()
    {
        //AttackPointが設定されていなければ表示しない
        if (attackPoint == null)
        {
            return;
        }

        //Scene画面で攻撃範囲をワイヤー球として表示
        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRange
        );
    }
}