using UnityEngine;
using UnityEngine.SceneManagement;

public class Store : MonoBehaviour
{
    public static Store Instance { get; private set; }

    public enum Level { Menu, Malware, Phishing }

    public Level selectedLevel = Level.Menu;

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
