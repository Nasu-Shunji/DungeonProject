using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class DungeonClearZone : MonoBehaviour
{
    [Header("Clear Condition")]
    [SerializeField] private int requiredItemCount = 1;

    [Header("UI")]
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private TMP_Text clearMessage;
    private bool isCleared;

    [Header("Audio")]
    [SerializeField] private AudioClip clearSound;

    //AudioSourceはその音声を実際に再生する装置
    private AudioSource audioSource;

    [Header("Menu")]
    [SerializeField] private PauseMenu pauseMenu;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
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

        if (clearSound != null)
        {
            audioSource.PlayOneShot(clearSound);
        }

        if (clearMessage != null)
        {
            clearMessage.text = "Dungeon Clear!";
        }

        if (clearPanel != null)
        {
            clearPanel.SetActive(true);
        }

        if (pauseMenu != null)
        {
            pauseMenu.EnterUiMode();

            // クリア画面中にEscでポーズ画面が開くのを防ぐ
            pauseMenu.enabled = false;
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
        if (pauseMenu != null)
        {
            pauseMenu.ExitUiMode();
        }
        else
        {
            Time.timeScale = 1f;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }
}