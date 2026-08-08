using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile")]
    //弾が進む速度
    [SerializeField] private float speed = 8f;

    //Playerへ与えるダメージ
    [SerializeField] private int damage = 15;

    //弾が永遠に残らないようにするための生存時間
    [SerializeField] private float lifeTime = 5f;

    private void Start()
    {
        //lifeTime秒後に弾を自動的に削除
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        //弾自身の前方向へ少しずつ移動
        transform.position +=
            transform.forward
            * speed
            * Time.deltaTime;
    }

    //弾を生成したEnemy側からダメージを設定する
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        //RoomEntranceTriggerなどのTriggerには反応しない
        if (other.isTrigger)
        {
            return;
        }

        //ぶつかった相手、またはその親からPlayerHealthを探す
        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        //Playerだった場合はダメージを与える
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        //Playerや壁など、何かに当たったら弾を削除
        Destroy(gameObject);
    }
}