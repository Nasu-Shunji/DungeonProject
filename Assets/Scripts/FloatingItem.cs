using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 90f;

    [Header("Floating")]
    [SerializeField] private float floatingHeight = 0.2f;
    [SerializeField] private float floatingSpeed = 2f;

    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        RotateItem();
        FloatItem();
    }

    private void RotateItem()
    {
        transform.Rotate(
            0f,
            rotationSpeed * Time.deltaTime,
            0f,
            Space.Self
        );
    }

    private void FloatItem()
    {
        //渡された数値に応じて、-1から1の間を滑らかに変化する値を返す
        float heightOffset =
            Mathf.Sin(Time.time * floatingSpeed)
            * floatingHeight;

        transform.localPosition =
            startPosition + Vector3.up * heightOffset;
    }
}