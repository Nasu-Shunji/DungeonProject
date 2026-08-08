using System.Collections;
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

    //攻撃判定の対象となるEnemy Layer
    [SerializeField] private LayerMask enemyLayer;

    [Header("Attack Visual")]

    //剣を振るときの回転中心
    [SerializeField] private Transform weaponPivot;

    //1回の攻撃で剣を振る角度
    [SerializeField] private float swingAngle = 120f;

    //剣を振って元の位置へ戻るまでの時間
    [SerializeField] private float attackDuration = 0.25f;

    //次に攻撃できるまでの残り時間
    private float attackTimer;

    //現在攻撃中かどうか
    private bool isAttacking;

    private void Update()
    {
        //次に攻撃できるまでの残り時間を、毎フレーム経過時間分だけ減らす
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

        //攻撃の待ち時間が残っている、または現在攻撃中なら新しい攻撃を開始しない
        if (attackTimer > 0f || isAttacking)
        {
            return;
        }

        //攻撃演出と攻撃判定を開始
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        //攻撃中の状態にする
        isAttacking = true;

        //次に攻撃できるまでの待ち時間を設定
        attackTimer = attackInterval;

        //WeaponPivotが設定されていない場合は、演出なしで攻撃判定だけ行う
        if (weaponPivot == null)
        {
            PerformHit();

            isAttacking = false;
            yield break;
        }

        //攻撃開始前の回転を保存
        Quaternion startRotation =
            weaponPivot.localRotation;

        //剣を振り切ったときの回転を作る
        //最初の角度から、さらにY方向へ120度回した角度
        Quaternion endRotation =
            startRotation
            * Quaternion.Euler(
                0f,
                swingAngle,
                0f
            );

        //攻撃時間の半分を計算
        //剣を振ることは往復運動に当たるため、攻撃時間の半分ずつで合計になる
        float halfDuration =
            attackDuration * 0.5f;

        //剣を振り始めてから何秒経ったかを保存する変数
        float elapsedTime = 0f;

        //開始位置から振り切る位置まで、少しずつ剣を回転させる
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;

            //0～1の割合へ変換（Clamp01は0～1に収める）
            //Slerpでは0スタート、1ゴール（0.5は途中半分まで）のための計算
            float t = Mathf.Clamp01(
                elapsedTime / halfDuration
            );

            //開始位置から終了位置へ滑らかに回転、Quaternion.Slerpは回転を滑らかに変える
            //回転A(start)から回転B(end)まで、指定された割合(t)だけ進んだ回転を取得する
            weaponPivot.localRotation =
                Quaternion.Slerp(
                    startRotation,
                    endRotation,
                    t
                );

            //次のフレームまで待つ
            //これによって回転処理をフレームごとに少しずつ進める
            yield return null;
        }

        //剣を振り切ったタイミングで攻撃判定
        PerformHit();

        elapsedTime = 0f;

        //振り切った位置から、元の位置へ少しずつ戻す
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsedTime / halfDuration
            );

            weaponPivot.localRotation =
                Quaternion.Slerp(
                    endRotation,
                    startRotation,
                    t
                );

            //次のフレームまで待つ
            yield return null;
        }

        //誤差が残らないよう最後に元の回転へ戻す
        weaponPivot.localRotation =
            startRotation;

        //攻撃終了
        isAttacking = false;
    }

    private void PerformHit()
    {
        //AttackPointを中心とした球形範囲内から、Enemy LayerのColliderをすべて取得
        Collider[] hitEnemies =
            Physics.OverlapSphere(
                attackPoint.position,
                attackRange,
                enemyLayer,
                QueryTriggerInteraction.Ignore
            );

        //攻撃範囲内にいるEnemyを順番に確認
        foreach (Collider hitEnemy in hitEnemies)
        {
            //当たったCollider、または親オブジェクトからEnemyHealthを探す
            EnemyHealth enemyHealth =
                hitEnemy.GetComponentInParent<EnemyHealth>();

            //EnemyHealthがあればダメージを与える
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(
                    attackDamage
                );
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        //AttackPointが設定されていなければ表示しない
        if (attackPoint == null)
        {
            return;
        }

        //攻撃範囲を分かりやすく赤色で表示
        Gizmos.color = Color.red;

        //Scene画面に攻撃範囲をワイヤー球として表示
        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRange
        );
    }
}