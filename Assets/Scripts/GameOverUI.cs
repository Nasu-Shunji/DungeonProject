using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("Player")]

    //Playerの死亡通知を受け取るPlayerHealth
    [SerializeField] private PlayerHealth playerHealth;

    [Header("UI")]

    //ゲームオーバー時に表示するPanel
    [SerializeField] private GameObject gameOverPanel;

    //現在のSceneを最初からやり直すボタン
    [SerializeField] private Button retryButton;

    //タイトル画面へ戻るボタン
    [SerializeField] private Button titleButton;

    private void Awake()
    {
        //Scene開始時はゲームを通常速度に戻す
        Time.timeScale = 1f;

        //Scene開始時はゲームオーバー画面を非表示にする
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            //Playerが死亡したとき、ShowGameOverを実行するよう登録
            playerHealth.Died += ShowGameOver;
        }

        if (retryButton != null)
        {
            //RETRYボタンが押されたとき、RetryGameを実行するよう登録
            retryButton.onClick.AddListener(RetryGame);
        }

        if (titleButton != null)
        {
            //TITLEボタンが押されたとき、ReturnToTitleを実行するよう登録
            titleButton.onClick.AddListener(ReturnToTitle);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            //死亡イベントへの登録を解除
            playerHealth.Died -= ShowGameOver;
        }

        if (retryButton != null)
        {
            //RETRYボタンへの登録を解除
            retryButton.onClick.RemoveListener(RetryGame);
        }

        if (titleButton != null)
        {
            //TITLEボタンへの登録を解除
            titleButton.onClick.RemoveListener(ReturnToTitle);
        }
    }

    private void ShowGameOver()
    {
        //ゲームオーバー画面を表示
        gameOverPanel.SetActive(true);

        //ゲーム内の時間を停止
        Time.timeScale = 0f;

        //マウスカーソルを表示
        Cursor.visible = true;

        //カーソルを画面中央へ固定せず、自由に動かせる状態にする
        Cursor.lockState = CursorLockMode.None;
    }

    private void RetryGame()
    {
        //停止していたゲーム内時間を通常に戻す
        Time.timeScale = 1f;

        //現在開いているSceneを最初から読み込む
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    private void ReturnToTitle()
    {
        //停止していたゲーム内時間を通常に戻す
        Time.timeScale = 1f;

        //Title Sceneへ移動
        SceneManager.LoadScene("Title");
    }
}