using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pausePanel;

    [Header("Scene")]
    [SerializeField] private string titleSceneName = "Title";

    private bool isPaused;

    private void Awake()
    {
        // 前のシーンで停止状態が残らないようにする
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        LockCursor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        EnterUiMode();
    }

    public void ResumeGame()
    {
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        ExitUiMode();
    }

    public void ReturnToTitle()
    {
        Time.timeScale = 1f;

        UnlockCursor();

        SceneManager.LoadScene(titleSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EnterUiMode()
    {
        Time.timeScale = 0f;

        UnlockCursor();
    }

public void ExitUiMode()
    {
        Time.timeScale = 1f;

        LockCursor();
    }
}