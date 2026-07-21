using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMover : MonoBehaviour
{
    //敵の現在の行動状態
    private enum EnemyState
    {
    Patrol, //巡回
    Chase,  //Playerを追跡
    Attack  //その場でPlayerを攻撃
    }

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;

    [Header("Player")]
    [SerializeField] private Transform player;

    //Playerを発見できる最大距離
    [SerializeField] private float detectionDistance = 5f;

    //追跡を諦める距離
    [SerializeField] private float loseDistance = 8f;

    [Header("Sight")]

    //敵の視野角。90なら正面から左右45度ずつ見える
    [SerializeField] private float fieldOfViewAngle = 90f;

    //敵の目の高さ
    [SerializeField] private float eyeHeight = 1f;

    //Playerのどの高さを目標として見るか
    [SerializeField] private float playerTargetHeight = 1f;

    //Playerを見失ってから巡回に戻るまでの時間
    [SerializeField] private float loseSightDelay = 2f;

    //Raycastが判定するLayer
    [SerializeField] private LayerMask sightLayers;

    private NavMeshAgent agent;

    //現在向かっている巡回地点の番号
    private int currentPointIndex;

    //現在の敵の状態
    private EnemyState currentState;

    //Playerを見失ってからの経過時間
    private float loseSightTimer;

    //同じEnemyに付いている攻撃処理
    private EnemyAttack enemyAttack;

    private void Awake()
    {
        //このEnemyに付いているNavMeshAgentを取得
        agent = GetComponent<NavMeshAgent>();

        //このEnemyに付いているEnemyAttackを取得
        enemyAttack = GetComponent<EnemyAttack>();
    }

    private void Start()
    {
        //巡回地点が設定されていなければ処理を停止
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError(
                "Patrol Points are not assigned.",
                this
            );

            enabled = false;
            return;
        }

        //Playerが設定されていなければ処理を停止
        if (player == null)
        {
            Debug.LogError(
                "Player is not assigned.",
                this
            );

            enabled = false;
            return;
        }

        //ゲーム開始時は巡回状態
        currentState = EnemyState.Patrol;

        //最初の巡回地点へ移動を開始
        MoveToCurrentPoint();
    }

    private void Update()
    {
        //このEnemyとPlayerの距離を取得
        float distanceToPlayer = Vector3.Distance(
            transform.position,
            player.position
        );

        //現在が巡回状態の場合
        if (currentState == EnemyState.Patrol)
        {
            //Playerが発見距離内にいて、実際に見えているか確認
            bool canDetectPlayer =
                distanceToPlayer <= detectionDistance
                && CanSeePlayer();

            //Playerを発見したら追跡へ切り替える
            if (canDetectPlayer)
            {
                StartChase();
                return;
            }

            //Playerが見えていなければ巡回を続ける
            UpdatePatrol();
        }

        //現在が追跡状態の場合
        else if (currentState == EnemyState.Chase)
        {
            UpdateChase(distanceToPlayer);
        }

        //現在が攻撃状態の場合
        else if (currentState == EnemyState.Attack)
        {
            UpdateAttack(distanceToPlayer);
        }
    }

    private void UpdatePatrol()
    {
        //NavMeshがルートを計算中なら、まだ到着判定をしない
        if (agent.pathPending)
        {
            return;
        }

        //remainingDistance：目的地までの残り距離
        //stoppingDistance：目的地の何メートル手前で止まるか
        if (agent.remainingDistance
            <= agent.stoppingDistance + 0.05f)
        {
            //次の巡回地点の番号へ進める
            currentPointIndex++;

            //最後の巡回地点まで到達したら0番へ戻す
            if (currentPointIndex >= patrolPoints.Length)
            {
                currentPointIndex = 0;
            }

            //次の巡回地点へ移動を開始
            MoveToCurrentPoint();
        }
    }

    private void UpdateChase(float distanceToPlayer)
    {
        //追跡を継続できる距離内で、なおかつPlayerが見えているか確認
        bool canStillSeePlayer =
            distanceToPlayer <= loseDistance
            && CanSeePlayer();

        if (canStillSeePlayer)
        {
            //Playerが見えているため、見失った時間を0に戻す
            loseSightTimer = 0f;
        }
        else
        {
            //Playerが見えない時間を加算
            loseSightTimer += Time.deltaTime;
        }

        //Playerが遠くへ離れた、または一定時間見失ったら巡回へ戻る
        if (distanceToPlayer >= loseDistance
            || loseSightTimer >= loseSightDelay)
        {
            StartPatrol();
            return;
        }

        //Playerが攻撃距離内に入ったら攻撃状態へ切り替える
    if (distanceToPlayer <= enemyAttack.AttackDistance
        && CanSeePlayer())
        {
            StartAttack();
            return;
        }

        //Playerの現在位置を目的地として追跡
        agent.SetDestination(player.position);
    }

    private void UpdateAttack(float distanceToPlayer)
    {
        //Playerが攻撃距離の外へ出たら、
        //攻撃をやめて再び追跡する
        if (distanceToPlayer > enemyAttack.AttackDistance)
        {
            StartChase();
            return;
        }

        //EnemyからPlayerへ向かう方向を取得
        Vector3 directionToPlayer =
            player.position - transform.position;

        //上下方向の傾きをなくし、Enemyが横方向だけを向くようにする(Yの値が違えば上下のベクトルも加わるため)
        directionToPlayer.y = 0f;

        //Player方向が存在する場合
        if (directionToPlayer.sqrMagnitude > 0.001f)
        {
            //Playerの方向を向くための回転を作る
            Quaternion targetRotation =
                Quaternion.LookRotation(directionToPlayer);

            //Enemyを少しずつPlayer方向へ回転させる、在の回転から目標の回転へ、指定した角度分だけ近づける
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                agent.angularSpeed * Time.deltaTime
            );
        }

        //攻撃可能なタイミングならPlayerへダメージを与える
        enemyAttack.TryAttack();
    }

    private bool CanSeePlayer()
    {
        //Enemyの位置から目の高さまで上げる
        Vector3 eyePosition =
            transform.position + Vector3.up * eyeHeight;

        //Playerの位置から確認したい高さまで上げる
        Vector3 playerTargetPosition =
            player.position + Vector3.up * playerTargetHeight;

        //Enemyの目からPlayerへ向かう方向を作る、座標同士を引くと、片方からもう片方へ向かうベクトルを作れる
        Vector3 directionToPlayer =
            playerTargetPosition - eyePosition;

        //Enemyの正面とPlayer方向の角度を取得、Vector3.Angle()は、この2つの方向の間に何度の差があるか
        float angleToPlayer = Vector3.Angle(
            transform.forward,
            directionToPlayer
        );

        //視野角の半分より外側なら、Playerは視界に入っていない
        if (angleToPlayer > fieldOfViewAngle * 0.5f)
        {
            return false;
        }

        //Enemyの目からPlayerへRayを飛ばし、途中で何かに当たったか確認
        bool hitSomething = Physics.Raycast(
            eyePosition,
            directionToPlayer.normalized,
            out RaycastHit hit,
            //magnitudeはベクトルの長さ
            directionToPlayer.magnitude,
            sightLayers,
            QueryTriggerInteraction.Ignore
        );

        //何にも当たらなければPlayerは見えていない
        if (!hitSomething)
        {
            return false;
        }

        //最初に当たったものがPlayer、またはPlayerの子オブジェクトなら見えている
        //hit.transform→当たったオブジェクト
        return hit.transform == player
            || hit.transform.IsChildOf(player);
    }

    private void StartChase()
    {
        //敵の状態を追跡へ変更
        currentState = EnemyState.Chase;

        //NavMeshAgentの移動を再開
        agent.isStopped = false;

        //見失った時間をリセット
        loseSightTimer = 0f;

        //Playerの現在位置へ移動を開始
        agent.SetDestination(player.position);

        Debug.Log("Enemy started chasing.");
    }

     private void StartAttack()
    {
        //敵の状態を攻撃へ変更
        currentState = EnemyState.Attack;

        //攻撃中はNavMeshAgentの移動を停止
        agent.isStopped = true;

        Debug.Log("Enemy started attacking.");
    }

    private void StartPatrol()
    {
        //敵の状態を巡回へ変更
        currentState = EnemyState.Patrol;

        //NavMeshAgentの移動を再開
        agent.isStopped = false;

        //見失った時間をリセット
        loseSightTimer = 0f;

        //現在選択されている巡回地点へ移動
        MoveToCurrentPoint();

        Debug.Log("Enemy returned to patrol.");
    }

    private void MoveToCurrentPoint()
    {
        //現在の番号に対応する巡回地点を取得
        Transform point = patrolPoints[currentPointIndex];

        //巡回地点が設定されていなければ処理を中止
        if (point == null)
        {
            Debug.LogError(
                $"Patrol Point {currentPointIndex} is not assigned.",
                this
            );

            return;
        }

        //巡回地点の座標をNavMeshAgentの目的地に設定
        agent.SetDestination(point.position);
    }
}