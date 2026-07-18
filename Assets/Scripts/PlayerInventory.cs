using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int itemCount;

    public int ItemCount => itemCount;

    public void AddItem(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        itemCount += amount;

        Debug.Log($"Item count: {itemCount}");
    }
}