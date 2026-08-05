using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Heal")]

    //Playerへ与える回復量
    [SerializeField] private int healAmount = 30;

    private void OnTriggerEnter(Collider other)
    {
        //接触したCollider、またはその親からPlayerHealthを探す
        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        //PlayerHealthを持っていないオブジェクトなら処理しない
        if (playerHealth == null)
        {
            return;
        }

        //PlayerのHPを回復し、実際に回復できたかを受け取る
        bool healed =
            playerHealth.Heal(healAmount);

        //回復できた場合だけアイテムを削除
        if (healed)
        {
            Destroy(gameObject);
        }
    }
}