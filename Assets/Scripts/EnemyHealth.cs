using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]

    //Enemyの最大HP
    [SerializeField] private int maxHealth = 60;

    //Enemyの現在HP
    private int currentHealth;

    //死亡処理を何度も実行しないためのフラグ
    private bool isDead;

    private void Awake()
    {
        //ゲーム開始時は最大HPにする
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        //0以下のダメージ、または死亡済みなら処理しない
        if (damage <= 0 || isDead)
        {
            return;
        }

        //現在HPからダメージを引き、0未満にならないようにする
        currentHealth = Mathf.Max(
            currentHealth - damage,
            0
        );

        Debug.Log(
            $"Enemy took {damage} damage. HP: {currentHealth}"
        );

        //HPが0になったら死亡処理
        if (currentHealth == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //死亡処理を一度だけ実行する
        isDead = true;

        Debug.Log("Enemy died.");

        //EnemyのGameObjectを削除
        Destroy(gameObject);
    }
}