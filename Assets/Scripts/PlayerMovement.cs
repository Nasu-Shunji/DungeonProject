using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Dash")]
    //ダッシュ中の移動速度
    [SerializeField] private float dashSpeed = 10f;

    //1回のダッシュが続く時間
    [SerializeField] private float dashDuration = 0.2f;

    //次にダッシュできるまでの待ち時間
    [SerializeField] private float dashCooldown = 1f;

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float fallLimitY = -5f;

    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;
    private float verticalVelocity;

    //現在ダッシュ中か
    private bool isDashing;

    //他のScriptから現在ダッシュ中か確認できるようにする
    public bool IsDashing => isDashing;

    //現在のダッシュの残り時間
    private float dashTimer;

    //次にダッシュできるまでの残り時間
    private float dashCooldownTimer;

    //ダッシュ開始時の移動方向を保存する
    private Vector3 dashDirection;

    [Header("Dash Visual")]
    //ダッシュ中に表示する軌跡
    [SerializeField] private TrailRenderer dashTrail;

    [Header("Dash Audio")]
    //ダッシュしたときに再生するSE
    [SerializeField] private AudioClip dashSound;

    //ダッシュSEを再生するAudioSource
    private AudioSource audioSource;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        //Playerに付いているAudioSourceを取得
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (transform.position.y < fallLimitY)
        {
            Respawn();
            return;
        }

        //ダッシュのクールタイムを毎フレーム減らす
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 horizontalMovement =
            (
                cameraForward * vertical
                + cameraRight * horizontal
            ).normalized;

        //Shiftを押した瞬間、移動入力があり、クールタイムが終わっていればダッシュ開始
        if (Input.GetKeyDown(KeyCode.LeftShift)
            && horizontalMovement.sqrMagnitude > 0.01f
            && dashCooldownTimer <= 0f)
        {
            StartDash(horizontalMovement);
        }

        //ダッシュ中は残り時間を減らす
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;

            //ダッシュ時間が終了したら通常移動へ戻す
            if (dashTimer <= 0f)
            {
                isDashing = false;

                //ダッシュ終了後は新しい軌跡を作らない
                if (dashTrail != null)
                {
                    dashTrail.emitting = false;
                }
            }
        }

        //通常時は現在の入力方向、ダッシュ中は開始時に保存した方向を使用
        Vector3 movementDirection =
            isDashing
                ? dashDirection
                : horizontalMovement;

        //movementDirection.sqrMagnitudeはのベクトルがどれくらいの大きさを持っているか→移動方向がちゃんとあるか
        if (movementDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(movementDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        //ダッシュ中だけ移動速度をdashSpeedへ変更
        float currentSpeed =
            isDashing
                ? dashSpeed
                : moveSpeed;

        Vector3 movement =
            movementDirection * currentSpeed;

        movement.y = verticalVelocity;

        controller.Move(
            movement * Time.deltaTime
        );
    }

    private void StartDash(Vector3 direction)
    {
        //Shiftを押した瞬間の移動方向を保存
        dashDirection = direction;

        //ダッシュ状態にする
        isDashing = true;

        //ダッシュできる残り時間を設定
        dashTimer = dashDuration;

        //次にダッシュできるまでの待ち時間を設定
        dashCooldownTimer = dashCooldown;

        //前回のダッシュで残っている軌跡を消す
        if (dashTrail != null)
        {
            dashTrail.Clear();

            //ダッシュ開始と同時に軌跡を出す
            dashTrail.emitting = true;
        }

        //ダッシュSEが設定されていれば再生
        if (dashSound != null)
        {
            audioSource.PlayOneShot(
                dashSound,
                0.5f
            );
        }
    }

    private void Respawn()
    {
        if (respawnPoint == null)
        {
            Debug.LogError(
                "Respawn Pointが設定されていません。",
                this
            );

            return;
        }

        controller.enabled = false;

        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;

        verticalVelocity = 0f;

        controller.enabled = true;
    }
}