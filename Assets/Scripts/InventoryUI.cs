using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;

    private TMP_Text itemCountText;

    private void Awake()
    {
        itemCountText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        if (inventory == null)
        {
            Debug.LogError(
                "PlayerInventoryが設定されていません。",
                this
            );

            enabled = false;
            return;
        }

        // 所持数の変更通知を受け取る
        // PlayerInventoryのアイテム数が変わったら、InventoryUIのUpdateItemCountを呼んでください
        inventory.ItemCountChanged += UpdateItemCount;

        // ゲーム開始時の値を表示
        UpdateItemCount(inventory.ItemCount);
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.ItemCountChanged -= UpdateItemCount;
        }
    }

    private void UpdateItemCount(int count)
    {
        itemCountText.text = $"Items: {count}";
    }
}