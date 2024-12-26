using UnityEngine;
using UnityEngine.SceneManagement;

public class Store : MonoBehaviour
{
    public static Store Instance { get; private set; }

    public enum Quiz { None, All, Malware, Phishing }

    public Quiz quizToLoad = Quiz.None;
    public int minigameScore = 0;
    public int quizScore = 0;

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoggingService.LogStartGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
