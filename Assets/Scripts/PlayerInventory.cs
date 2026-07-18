using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int itemCount;

    public int ItemCount => itemCount;

    // 所持数が変わったことを他のスクリプトへ通知する
    public event Action<int> ItemCountChanged;

    public void AddItem(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        itemCount += amount;

        // 最新の所持数をUIなどへ通知
        ItemCountChanged?.Invoke(itemCount);

        Debug.Log($"Item count: {itemCount}");
    }
}