using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ChestInteraction : MonoBehaviour
{
    [Header("Chest")]
    [SerializeField] private Transform lidPivot;
    [SerializeField] private GameObject rewardItem;
    [SerializeField] private float openAngle = 100f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private int rewardAmount = 1;

    //宝箱の報酬として上昇する攻撃力
    [SerializeField] private int attackIncrease = 10;

    [Header("UI")]
    [SerializeField] private TMP_Text interactionPrompt;

    private bool isPlayerNearby;
    private bool isOpen;
    private bool hasReward = true;

    private PlayerInventory nearbyInventory;

    //近くにいるPlayerの攻撃処理
    private PlayerAttack nearbyPlayerAttack;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    [Header("Audio")]
    [SerializeField] private AudioClip chestOpenSound;
    [SerializeField] private AudioClip itemPickupSound;

    //AudioSourceはその音声を実際に再生する装置
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

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

                 if (chestOpenSound != null)
                {
                    audioSource.PlayOneShot(chestOpenSound);
                }
            }
            else if (hasReward && nearbyInventory != null)
            {
                // 2回目のEキー：アイテムを取得
                hasReward = false;

                //クリア条件などに使用するItemCountを増やす
                nearbyInventory.AddItem(rewardAmount);

                //Playerの攻撃力を強化
                if (nearbyPlayerAttack != null)
                {
                    nearbyPlayerAttack.IncreaseAttackDamage(attackIncrease);
                }

                 if (itemPickupSound != null)
                {
                    audioSource.PlayOneShot(itemPickupSound);
                }


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

        //Playerの攻撃処理も取得
        nearbyPlayerAttack =
            other.GetComponentInParent<PlayerAttack>();

        if (nearbyInventory == null)
        {
            Debug.LogError(
                "PlayerInventoryがPlayerに付いていません。",
                other
            );
        }

        if (nearbyPlayerAttack == null)
        {
            Debug.LogError(
                "PlayerAttackがPlayerに付いていません。",
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
        nearbyPlayerAttack = null;

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