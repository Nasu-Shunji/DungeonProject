using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonClearZone : MonoBehaviour
{
    [Header("Clear Condition")]
    [SerializeField] private int requiredItemCount = 1;

    [Header("UI")]
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private TMP_Text clearMessage;
    private bool isCleared;

    private void Awake()
    {
       if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCleared)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerInventory inventory =
            other.GetComponentInParent<PlayerInventory>();

        if (inventory == null)
        {
            Debug.LogError(
                "PlayerInventoryが見つかりません。",
                other
            );

            return;
        }

        if (inventory.ItemCount < requiredItemCount)
        {
            Debug.Log(
                $"Not enough items: {inventory.ItemCount}"
            );

            return;
        }

        ClearDungeon(other);
    }

    private void ClearDungeon(Collider playerCollider)
    {
        isCleared = true;

        if (clearMessage != null)
        {
            clearMessage.text = "Dungeon Clear!";
        }

        if (clearPanel != null)
        {
            clearPanel.SetActive(true);
        }

        PlayerMovement playerMovement =
            playerCollider.GetComponentInParent<PlayerMovement>();

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        Debug.Log("Dungeon cleared!");
    }

    public void RetryDungeon()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }
}