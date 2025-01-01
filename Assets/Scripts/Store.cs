using UnityEngine;
using UnityEngine.SceneManagement;

public class Store : MonoBehaviour
{
    public static Store Instance { get; private set; }

    public enum Quiz { None = -2, All = -1, Malware, Firewall, Phishing, Cookies, Phone, AI, Passwords, TFA }

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
