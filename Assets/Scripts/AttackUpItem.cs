using UnityEngine;

public class AttackUpItem : MonoBehaviour
{
    //取得したときに上昇する攻撃力
    [SerializeField] private int attackIncrease = 10;

    private void OnTriggerEnter(Collider other)
    {
        //接触したPlayerからPlayerAttackを探す
        PlayerAttack playerAttack =
            other.GetComponentInParent<PlayerAttack>();

        //Playerではなければ何もしない
        if (playerAttack == null)
        {
            return;
        }

        //Playerの攻撃力を上げる
        playerAttack.IncreaseAttackDamage(
            attackIncrease
        );

        //取得したアイテムを削除
        Destroy(gameObject);
    }
}