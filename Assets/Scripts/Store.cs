using UnityEngine;

public class Store : MonoBehaviour
{
    public static Store Instance { get; private set; }

    public enum Quiz { None = -2, All = -1, Malware, Firewall, Phishing, Cookies, Phone, AI, Passwords, TFA }

    public Quiz quizToLoad = Quiz.None;
    public int minigameScore = 0;
    public int quizScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        LoggingService.LogStartGame();
        ApplyQualityLevel(PlayerPrefs.GetInt("QualityLevel", 2));
    }

    // Quality level
    public void SelectQualityLevel(int level)
    {
        LoggingService.Log(LoggingService.LogCategory.Settings, "Quality changed to: " + level);
        PlayerPrefs.SetInt("QualityLevel", level);
        ApplyQualityLevel(level);
    }

    private void ApplyQualityLevel(int level)
    {
        QualitySettings.vSyncCount = 1;
        switch (level)
        {
            case 0:
                Application.targetFrameRate = 30;
                break;
            case 1:
                Application.targetFrameRate = 60;
                break;
            case 2:
                Application.targetFrameRate = 240;
                break;
        }

    }
}
