using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Scene";  
    public void RetryGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
