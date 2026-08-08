using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    //Enemyがどの攻撃方法を使うか
    private enum AttackType
    {
        Melee,
        Ranged
    }

    [Header("Target")]
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Attack")]
    //近接攻撃か遠距離攻撃か
    [SerializeField] private AttackType attackType =
        AttackType.Melee;

    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackInterval = 1f;

    [Header("Ranged Attack")]
    //発射する弾のPrefab
    [SerializeField] private GameObject projectilePrefab;

    //弾を生成する位置
    [SerializeField] private Transform firePoint;

    //Playerの足元ではなく、少し上を狙うための高さ
    [SerializeField] private float targetHeight = 1f;

    private float attackTimer;

    public float AttackDistance => attackDistance;

    private void Update()
    {
        //攻撃後の待ち時間を毎フレーム減らす
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    public void TryAttack()
    {
        if (playerHealth == null)
        {
            return;
        }

        //まだ攻撃待ち時間中なら攻撃しない
        if (attackTimer > 0f)
        {
            return;
        }

        //遠距離Enemyなら弾を発射
        if (attackType == AttackType.Ranged)
        {
            ShootProjectile();
        }
        else
        {
            //通常Enemyなら今まで通り直接ダメージ
            playerHealth.TakeDamage(attackDamage);
        }

        //次に攻撃できるまでの時間を設定
        attackTimer = attackInterval;
    }

    private void ShootProjectile()
    {
        if (projectilePrefab == null
            || firePoint == null)
        {
            Debug.LogWarning(
                "Projectile Prefab or Fire Point is not assigned.",
                this
            );

            return;
        }

        //Playerの少し上を狙う
        Vector3 targetPosition =
            playerHealth.transform.position
            + Vector3.up * targetHeight;

        //FirePointからPlayerへ向かう方向を求める
        Vector3 directionToPlayer =
            targetPosition
            - firePoint.position;

        //方向がほぼ0の場合はLookRotationを実行しない
        if (directionToPlayer.sqrMagnitude < 0.001f)
        {
            return;
        }

        //Player方向を向く回転を作る
        Quaternion projectileRotation =
            Quaternion.LookRotation(
                directionToPlayer
            );

        //FirePointの位置から弾を生成
        GameObject projectileObject =
            Instantiate(
                projectilePrefab,
                firePoint.position,
                projectileRotation
            );

        //生成した弾からEnemyProjectileを取得
        EnemyProjectile projectile =
            projectileObject.GetComponent<EnemyProjectile>();

        //RangedEnemy自身のAttack Damageを弾へ渡す
        if (projectile != null)
        {
            projectile.SetDamage(
                attackDamage
            );
        }
    }
}