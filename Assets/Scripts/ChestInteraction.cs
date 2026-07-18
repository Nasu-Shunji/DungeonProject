using TMPro;
using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    [Header("Chest")]
    [SerializeField] private Transform lidPivot;
    [SerializeField] private GameObject rewardItem;
    [SerializeField] private float openAngle = 100f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private int rewardAmount = 1;

    [Header("UI")]
    [SerializeField] private TMP_Text interactionPrompt;

    private bool isPlayerNearby;
    private bool isOpen;
    private bool hasReward = true;

    private PlayerInventory nearbyInventory;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Awake()
    {
        if (lidPivot == null)
        {
            Debug.LogError(
                "Lid Pivotが設定されていません。",
                this
            );

            enabled = false;
            return;
        }

        closedRotation = lidPivot.localRotation;

        // 今回の宝箱はZ軸を中心に開く
        openRotation =
            closedRotation * Quaternion.Euler(0f, 0f, openAngle);

        if (rewardItem != null)
        {
            rewardItem.SetActive(true);
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (!isOpen)
            {
                // 1回目のEキー：宝箱を開く
                isOpen = true;
            }
            else if (hasReward && nearbyInventory != null)
            {
                // 2回目のEキー：アイテムを取得
                hasReward = false;

                nearbyInventory.AddItem(rewardAmount);

                if (rewardItem != null)
                {
                    rewardItem.SetActive(false);
                }
            }

            UpdatePromptText();
        }

        Quaternion targetRotation =
            isOpen ? openRotation : closedRotation;

        lidPivot.localRotation = Quaternion.RotateTowards(
            lidPivot.localRotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        isPlayerNearby = true;

        nearbyInventory =
            other.GetComponentInParent<PlayerInventory>();

        if (nearbyInventory == null)
        {
            Debug.LogError(
                "PlayerInventoryがPlayerに付いていません。",
                other
            );
        }

        if (interactionPrompt != null)
        {
            UpdatePromptText();
            interactionPrompt.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        isPlayerNearby = false;
        nearbyInventory = null;

        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }

    private void UpdatePromptText()
    {
        if (interactionPrompt == null)
        {
            return;
        }

        if (!isOpen)
        {
            interactionPrompt.text =
                "Press E to open chest";
        }
        else if (hasReward)
        {
            interactionPrompt.text =
                "Press E to take item";
        }
        else if (nearbyInventory != null)
        {
            interactionPrompt.text =
                $"Item obtained! Total: {nearbyInventory.ItemCount}";
        }
        else
        {
            interactionPrompt.text =
                "Chest is empty";
        }
    }
}