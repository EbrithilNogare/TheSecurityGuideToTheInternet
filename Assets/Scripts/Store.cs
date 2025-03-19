using System;
using UnityEngine;

public record LevelData
{
    public int score;
    public bool isUnlocked;
}

public class Store : MonoBehaviour
{
    public enum Quiz { None = -2, All = -1, Malware = 0, Firewall = 1, Phishing = 2, Cookies = 3, Phone = 4, AI = 5, Passwords = 6, TFA = 7 }
    public enum Level { Malware = 0, Firewall = 1, Phishing = 2, Cookies = 3, Phone = 4, AI = 5, Passwords = 6, TFA = 7 }

    [HideInInspector] public static Store Instance { get; private set; }

    public bool[] levelUnlocked;

    [NonSerialized] public Quiz quizToLoad = Quiz.None;
    [NonSerialized] public int minigameScore = 0;
    [NonSerialized] public int quizScore = 0;
    [NonSerialized] public int[] levelStars = new int[] { 0b000, 0b000, 0b000, 0b000, 0b000, 0b000, 0b000, 0b000 }; // level, level, quiz;
    [NonSerialized] public int qualityLevel = 2;

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

        ApplyQualityLevel(PlayerPrefs.GetInt("QualityLevel", qualityLevel));
        if (PlayerPrefs.HasKey("LevelUnlocked"))
        {
            var levelUnlockedFromPlayerPref = JSON.FromJson<bool>(PlayerPrefs.GetString("LevelUnlocked"));
            for (int i = 0; i < levelUnlocked.Length; i++)
            {
                levelUnlocked[i] |= levelUnlockedFromPlayerPref[i];
            }
        }
        if (PlayerPrefs.HasKey("LevelStars"))
        {
            levelStars = JSON.FromJson<int>(PlayerPrefs.GetString("LevelStars"));
        }
    }

    public void SetLevelUnlocked(int level, bool unlocked)
    {
        LoggingService.Log(LoggingService.LogCategory.Store, "Level: " + level + ", unlocked: " + unlocked);
        levelUnlocked[level] = unlocked;
        PlayerPrefs.SetString("LevelUnlocked", JSON.ArrayToJson(levelUnlocked));
    }

    public void SetLevelScore(int level, int score)
    {
        LoggingService.Log(LoggingService.LogCategory.Store, "Level: " + level + ", score set: " + score);
        levelStars[level] |= score;
        PlayerPrefs.SetString("LevelStars", JSON.ArrayToJson(levelStars));
    }

    public void SetQualityLevel(int level)
    {
        LoggingService.Log(LoggingService.LogCategory.Settings, "Quality changed to: " + level);
        PlayerPrefs.SetInt("QualityLevel", level);
        ApplyQualityLevel(level);
    }

    private void ApplyQualityLevel(int level)
    {
        qualityLevel = level;
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
