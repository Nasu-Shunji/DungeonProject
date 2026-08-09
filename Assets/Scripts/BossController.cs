using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Phase 2")]
    //HPが何割以下になったら第2形態になるか
    [SerializeField, Range(0f, 1f)]
    private float phase2HealthRate = 0.5f;

    //第2形態の攻撃間隔
    [SerializeField] private float phase2AttackInterval = 0.8f;

    //第2形態の弾速
    [SerializeField] private float phase2ProjectileSpeed = 12f;

    private EnemyHealth enemyHealth;
    private EnemyAttack enemyAttack;

    //一度だけ第2形態へ移行するための判定
    private bool isPhase2;

    [Header("Phase 2 Visual")]
    //第2形態になったときに表示するエフェクト
    [SerializeField] private GameObject phase2Effect;

    private void Awake()
    {
        enemyHealth =
            GetComponent<EnemyHealth>();

        enemyAttack =
            GetComponent<EnemyAttack>();

        //ゲーム開始時は第2形態エフェクトを非表示
        if (phase2Effect != null)
        {
            phase2Effect.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (enemyHealth != null)
        {
            //BossのHPが変化したら確認する
            enemyHealth.HealthChanged +=
                HandleHealthChanged;
        }
    }

    private void OnDisable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.HealthChanged -=
                HandleHealthChanged;
        }
    }

    private void HandleHealthChanged(
        int currentHealth,
        int maxHealth
    )
    {
        //すでに第2形態なら何もしない
        if (isPhase2)
        {
            return;
        }

        //現在HPが最大HPの何割かを計算
        float healthRate =
            (float)currentHealth / maxHealth;

        //まだ第2形態になるHPではない
        if (healthRate > phase2HealthRate)
        {
            return;
        }

        EnterPhase2();
    }

    private void EnterPhase2()
    {
        isPhase2 = true;

        //攻撃間隔を短くする
        enemyAttack.SetAttackInterval(
            phase2AttackInterval
        );

        //弾を速くする
        enemyAttack.SetProjectileSpeed(
            phase2ProjectileSpeed
        );

        //第2形態になったことを見た目でも分かるようにする
        if (phase2Effect != null)
        {
            phase2Effect.SetActive(true);
        }

        Debug.Log(
            "Boss entered Phase 2.",
            this
        );
    }
}