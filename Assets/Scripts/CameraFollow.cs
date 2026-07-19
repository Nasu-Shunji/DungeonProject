using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Fixed Camera Position")]
    [SerializeField] private Vector3 offset =
        new Vector3(8f, 6f, 0f);

    [SerializeField] private float followSpeed = 5f;

    [Header("Mouse Rotation")]
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 70f;

    [Header("Camera Collision")]
    //どのLayerを壁として判定するか
    [SerializeField] private LayerMask collisionLayers;
    //壁を調べる球の太さ
    [SerializeField] private float cameraRadius = 0.3f;
    //壁から少し離す距離
    [SerializeField] private float wallPadding = 0.15f;
    //カメラがPlayerへ近づける限界
    [SerializeField] private float minDistance = 1.5f;

    //左右の角度
    private float yaw;
    //上下の角度
    private float pitch;
    //Playerとカメラの距離
    private float distance;

    private void Start()
    {
        if (target == null)
        {
            return;
        }

        // 元のOffsetから、カメラとPlayerの距離を取得
        distance = offset.magnitude;

        // 元のOffsetから、最初のカメラ角度を計算
        Quaternion initialRotation =
            Quaternion.LookRotation(-offset.normalized);

        Vector3 initialAngles =
            initialRotation.eulerAngles;

        yaw = initialAngles.y;
        pitch = initialAngles.x;

        // 340度などの表現を-20度などへ変換
        if (pitch > 180f)
        {
            pitch -= 360f;
        }

        // 開始時に元の固定位置へ即座に配置
        transform.position =
            target.position + offset;

        transform.LookAt(target);
    }

    private void LateUpdate()
    {
        if (target == null || Time.timeScale == 0f)
        {
            return;
        }

        // 右クリック中だけ角度を変更
        if (Input.GetMouseButton(1))
        {
            float mouseX =
                Input.GetAxis("Mouse X");

            float mouseY =
                Input.GetAxis("Mouse Y");

            yaw += mouseX * mouseSensitivity;
            pitch -= mouseY * mouseSensitivity;

            pitch = Mathf.Clamp(
                pitch,
                minPitch,
                maxPitch
            );
        }

        Quaternion cameraRotation =
            Quaternion.Euler(pitch, yaw, 0f);

        // Playerからカメラへ向かう方向、Vector3.backは後ろ方向(X:0,Y:0.Z:-1)
        Vector3 cameraDirection =
            cameraRotation * Vector3.back;

        // 通常時は元の距離を使用
        float currentDistance = distance;

        // Playerとカメラの間に壁があるか確認
        bool isBlocked = Physics.SphereCast(
            target.position,
            cameraRadius,
            cameraDirection,
            //壁に当たった場合の情報をhitへ
            out RaycastHit hit,
            distance,
            collisionLayers,
            //Is TriggerがオンのColliderを無視(宝箱や扉の操作範囲を壁として誤認しないため)
            QueryTriggerInteraction.Ignore
        );

        if (isBlocked)
        {
            // 壁の少し手前までカメラを近づける
            currentDistance = Mathf.Max(
                hit.distance - wallPadding,
                minDistance
            );
        }

        Vector3 desiredPosition =
            target.position
            + cameraDirection * currentDistance;

        if (isBlocked)
        {
            // 壁を見つけたときは即座に安全な位置へ移動
            transform.position = desiredPosition;
        }
        else
        {
            // 壁がなくなったら滑らかに元の距離へ戻る
            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                followSpeed * Time.deltaTime
            );
        }

        transform.LookAt(target);
    }
}