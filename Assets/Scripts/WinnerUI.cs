using UnityEngine;
using UnityEngine.SceneManagement;

public class WinnerUI : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Scene";  
    public void RetryGame() {
        SceneManager.LoadScene(gameSceneName);
    }
}
