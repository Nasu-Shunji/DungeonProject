using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DoorInteraction : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = -90f;
    [SerializeField] private float rotationSpeed = 180f;

    [Header("UI")]
    [SerializeField] private TMP_Text interactionPrompt;

    private bool isPlayerNearby;
    private bool isOpen;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    [Header("Audio")]
    [SerializeField] private AudioClip doorSound;

    //AudioSourceはその音声を実際に再生する装置
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (doorPivot == null)
        {
            Debug.LogError(
                "Door Pivotが設定されていません。",
                this
            );

            enabled = false;
            return;
        }

        closedRotation = doorPivot.localRotation;

        openRotation =
            closedRotation * Quaternion.Euler(0f, openAngle, 0f);

        // ゲーム開始時は操作案内を非表示にする
        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;

            if (doorSound != null)
        {
            audioSource.PlayOneShot(doorSound);
        }

            UpdatePromptText();
        }

        Quaternion targetRotation =
            isOpen ? openRotation : closedRotation;

        doorPivot.localRotation = Quaternion.RotateTowards(
            doorPivot.localRotation,
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

        interactionPrompt.text = isOpen
            ? "Press E to close"
            : "Press E to open";
    }
}