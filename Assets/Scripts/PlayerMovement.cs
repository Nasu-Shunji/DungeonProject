using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
     [SerializeField] private float moveSpeed = 5f;
     [SerializeField] private float gravity = -9.81f;
     [SerializeField] private float rotationSpeed = 10f;

     [Header("Respawn")]
     [SerializeField] private Transform respawnPoint;
     [SerializeField] private float fallLimitY = -5f;

     [SerializeField] private Transform cameraTransform;

    private CharacterController controller;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (transform.position.y < fallLimitY)
        {
            Respawn();
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 horizontalMovement =
    (cameraForward * vertical + cameraRight * horizontal).normalized;

    if (horizontalMovement.sqrMagnitude > 0.01f)
    {
        Quaternion targetRotation =
            Quaternion.LookRotation(horizontalMovement);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 movement = horizontalMovement * moveSpeed;
        movement.y = verticalVelocity;

        controller.Move(movement * Time.deltaTime);
    }

    private void Respawn()
    {
        if (respawnPoint == null)
        {
            Debug.LogError(
                "Respawn Pointが設定されていません。",
                this
            );

            return;
        }

        controller.enabled = false;

        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;

        verticalVelocity = 0f;

        controller.enabled = true;
    }
}