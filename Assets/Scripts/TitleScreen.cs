using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private string dungeonSceneName = "Dungeon";

    public void StartGame()
    {
        SceneManager.LoadScene(dungeonSceneName);
    }
}