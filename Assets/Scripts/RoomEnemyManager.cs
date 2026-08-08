using UnityEngine;

public class RoomEnemyManager : MonoBehaviour
{
    [Header("Enemies")]

    //この部屋にいるEnemy一覧
    [SerializeField] private EnemyHealth[] enemies;

    [Header("Door")]

    //Enemy全滅後に開く扉
    [SerializeField] private DoorInteraction door;

    //現在生き残っているEnemyの数
    private int remainingEnemies;

    private void Start()
    {
        //配列に登録されているEnemyの数を生存Enemy数として保存
        remainingEnemies = enemies.Length;

        //各Enemyの死亡イベントへOnEnemyDiedを登録する
        foreach (EnemyHealth enemy in enemies)
        {
            //Enemyが設定されていなければ飛ばす
            if (enemy == null)
            {
                continue;
            }

            //このEnemyが死亡したらOnEnemyDiedを実行するよう登録
            enemy.Died += OnEnemyDied;
        }

        //Enemyが最初から0体の場合はすぐ扉を開く
        if (remainingEnemies == 0)
        {
            OpenDoor();
        }
    }

    private void OnEnemyDied()
    {
        //Enemyが1体死亡したため、残りEnemy数を1減らす
        remainingEnemies--;

        Debug.Log(
            $"Remaining enemies: {remainingEnemies}"
        );

        //まだEnemyが残っているなら何もしない
        if (remainingEnemies > 0)
        {
            return;
        }

        //全Enemyを倒したので扉を開く
        OpenDoor();
    }

    private void OpenDoor()
    {
        //DoorInteractionが設定されていなければ処理しない
        if (door == null)
        {
            return;
        }

        //扉のロックを解除して自動で開く
        door.UnlockAndOpen();
    }

    private void OnDisable()
    {
        //イベント登録を解除する
        foreach (EnemyHealth enemy in enemies)
        {
            //すでにEnemyが削除されている場合は飛ばす
            if (enemy == null)
            {
                continue;
            }

            enemy.Died -= OnEnemyDied;
        }
    }
}