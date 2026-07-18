using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = -90f;
    [SerializeField] private float rotationSpeed = 180f;

    private bool isPlayerNearby;
    private bool isOpen;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Awake()
    {
        if (doorPivot == null)
        {
            Debug.LogError("Door Pivotが設定されていません。", this);
            enabled = false;
            return;
        }

        // ゲーム開始時の向きを「閉じた状態」として保存
        closedRotation = doorPivot.localRotation;

        // 閉じた状態からY軸方向へ90度回した向き
        openRotation =
            closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    private void Update()
    {
        // Playerが近くにいて、Eキーを押した瞬間
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

        Quaternion targetRotation =
            isOpen ? openRotation : closedRotation;

        // 目標角度まで徐々に回転
        doorPivot.localRotation = Quaternion.RotateTowards(
            doorPivot.localRotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("扉を操作できます：Eキー");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}