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

        // Playerから見た現在のカメラ位置、Vector3.backは後ろ方向(X:0,Y:0.Z:-1)
        Vector3 currentOffset =
            cameraRotation
            * Vector3.back
            * distance;

        Vector3 desiredPosition =
            target.position + currentOffset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        transform.LookAt(target);
    }
}