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


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }

    #region Cross scene communication
    [NonSerialized] public Quiz quizToLoad = Quiz.None;
    [NonSerialized] public int minigameStars = 0;
    [NonSerialized] public int quizStars = 0;
    [NonSerialized] public int quizScore = 0;
    [NonSerialized] public bool shouldOpenLevelCompletedDialog = false;
    #endregion

    #region Personal data
    [NonSerialized] private string _personalDataName;
    [NonSerialized] private string _personalDataAge;
    [NonSerialized] private int _personalDataGender;
    [NonSerialized] private string _personalDataClass;
    [NonSerialized] private string _personalDataRegion;
    [NonSerialized] private bool _personalDataConcent;

    public string PersonalDataName
    {
        get => _personalDataName;
        set { _personalDataName = value; SavePersonalData(); }
    }

    public string PersonalDataAge
    {
        get => _personalDataAge;
        set { _personalDataAge = value; SavePersonalData(); }
    }

    public int PersonalDataGender
    {
        get => _personalDataGender;
        set { _personalDataGender = value; SavePersonalData(); }
    }

    public string PersonalDataClass
    {
        get => _personalDataClass;
        set { _personalDataClass = value; SavePersonalData(); }
    }

    public string PersonalDataRegion
    {
        get => _personalDataRegion;
        set { _personalDataRegion = value; SavePersonalData(); }
    }

    public bool PersonalDataConcent
    {
        get => _personalDataConcent;
        set { _personalDataConcent = value; SavePersonalData(); }
    }

    private void SavePersonalData()
    {
        PlayerPrefs.SetString("PersonalDataName", _personalDataName);
        PlayerPrefs.SetString("PersonalDataAge", _personalDataAge);
        PlayerPrefs.SetInt("PersonalDataGender", _personalDataGender);
        PlayerPrefs.SetString("PersonalDataClass", _personalDataClass);
        PlayerPrefs.SetString("PersonalDataRegion", _personalDataRegion);
        PlayerPrefs.SetInt("PersonalDataConcent", _personalDataConcent ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadPersonalData()
    {
        _personalDataName = PlayerPrefs.GetString("PersonalDataName", "");
        _personalDataAge = PlayerPrefs.GetString("PersonalDataAge", "");
        _personalDataGender = PlayerPrefs.GetInt("PersonalDataGender", 0);
        _personalDataClass = PlayerPrefs.GetString("PersonalDataClass", "");
        _personalDataRegion = PlayerPrefs.GetString("PersonalDataRegion", "");
        _personalDataConcent = PlayerPrefs.GetInt("PersonalDataConcent", 0) == 1;
    }
    #endregion

    #region Tutorial
    [NonSerialized] private bool[] _tutorialDisplayed = new bool[Enum.GetNames(typeof(Level)).Length];
    private void SaveTutorialDisplayed()
    {
        int value = 0;
        for (int i = 0; i < _tutorialDisplayed.Length; i++)
        {
            if (_tutorialDisplayed[i])
                value |= (1 << i);
        }
        PlayerPrefs.SetInt("TutorialDisplayed", value);
        PlayerPrefs.Save();
    }

    private void LoadTutorialDisplayed()
    {
        int value = PlayerPrefs.GetInt("TutorialDisplayed", 0);
        for (int i = 0; i < _tutorialDisplayed.Length; i++)
        {
            _tutorialDisplayed[i] = (value & (1 << i)) != 0;
        }
    }

    public bool IsTutorialDisplayed(Level level)
    {
        return _tutorialDisplayed[(int)level];
    }

    public void SetTutorialDisplayed(Level level)
    {
        _tutorialDisplayed[(int)level] = true;
        SaveTutorialDisplayed();
    }
    #endregion

    #region Init
    [HideInInspector] public static Store Instance { get; private set; }
    private void Init()
    {
        LoadPersonalData();
        LoadTutorialDisplayed();
        LoadLevelSelection();

        LoggingService.LogStartGame();

        ApplyQualityLevel(PlayerPrefs.GetInt("QualityLevel", 2));
    }
    #endregion

    #region Level selection
    [NonSerialized] public int[] levelStars; // level, level, quiz;
    [NonSerialized] public bool[] levelUnlocked;

    public void SetLevelUnlocked(int level, bool unlocked)
    {
        LoggingService.Log(LoggingService.LogCategory.Store, "Level: " + level + ", unlocked: " + unlocked);
        levelUnlocked[level] = unlocked;
        PlayerPrefs.SetString("LevelUnlocked", JSON.ArrayToJson(levelUnlocked));
    }

    public void SetLevelScore(Store.Level level, int score)
    {
        LoggingService.Log(LoggingService.LogCategory.Store, "Level: " + (int)level + ", score set: " + score);
        levelStars[(int)level] |= score;
        PlayerPrefs.SetString("LevelStars", JSON.ArrayToJson(levelStars));
    }

    private void LoadLevelSelection()
    {
        levelUnlocked = new[] {
            true,  // Malware
            true,  // Firewall
            true,  // Phishing
            true,  // Cookies
            true,  // Phone
            true,  // AI
            true,  // Passwords
            true   // TFA
        };

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
        else
        {
            levelStars = new int[] { 0b000, 0b000, 0b000, 0b000, 0b000, 0b000, 0b000, 0b000 };
        }
    }
    #endregion

    #region Quality settings
    [NonSerialized] public int qualityLevel;

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
                Application.runInBackground = false;
                break;
            case 1:
                Application.targetFrameRate = 60;
                Application.runInBackground = true;
                break;
            case 2:
                Application.targetFrameRate = 240;
                Application.runInBackground = true;
                break;
        }

    }
    #endregion

    #region Player prefs reset
    public void ResetAllSettingsAndProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Init();
    }
    #endregion
}
