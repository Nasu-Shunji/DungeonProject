using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    //現在ゲームを映しているCamera
    private Camera mainCamera;

    private void Start()
    {
        //MainCameraタグが付いているCameraを取得
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        //Cameraが取得できていなければ処理しない
        if (mainCamera == null)
        {
            return;
        }

        //HPバーをCameraと同じ向きにすることで、常にプレイヤーから読める向きにする
        transform.rotation =
            mainCamera.transform.rotation;
    }
}