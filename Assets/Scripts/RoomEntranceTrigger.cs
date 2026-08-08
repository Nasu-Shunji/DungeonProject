using UnityEngine;

public class RoomEntranceTrigger : MonoBehaviour
{
    //閉じてロックする扉
    [SerializeField] private DoorInteraction door;

    //一度だけ実行するためのフラグ
    private bool hasEntered;

    private void OnTriggerEnter(Collider other)
    {
        //Player以外が入ってきた場合は処理しない
        if (!other.CompareTag("Player"))
        {
            return;
        }

        //すでに一度部屋へ入っている場合は処理しない
        if (hasEntered)
        {
            return;
        }

        //部屋に入ったことを記録
        hasEntered = true;

        //扉を閉じてロックする
        door.LockAndClose();
    }
}