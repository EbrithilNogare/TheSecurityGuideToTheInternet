using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
    public static readonly List<string> LevelCategories = new List<string> {
        "Malware-1", "Privacy-1", "Encryption-1", "Phone-1", "AI-1",
        "Malware-2", "Privacy-2", "Encryption-2",
    };

    public List<LevelSelectionCard> levelCards;

    private void Awake()
    {
        for (int i = 0; i < levelCards.Count; i++)
        {
            levelCards[i].Render(Store.Instance.levelUnlocked[i], Store.Instance.levelStars[i]);
        }
    }

    public void GoToLevelMenu()
    {
        SceneManager.LoadScene("LevelSelection");
    }

    public void LoadLevel(string level)
    {
        LoggingService.Log(LoggingService.LogCategory.Navigation, "Navigated to level: " + level);
        Assert.IsTrue(LevelCategories.Contains(level), "Unknown level: " + level);

        switch (level)
        {
            case "Malware-1":
                Store.Instance.quizToLoad = Store.Quiz.Malware;
                SceneManager.LoadScene("Malware_Presentation");
                break;
            case "Privacy-1":
                Store.Instance.quizToLoad = Store.Quiz.Phishing;
                SceneManager.LoadScene("Phishing_Presentation");
                break;
            case "Encryption-1":
                Store.Instance.quizToLoad = Store.Quiz.Passwords;
                SceneManager.LoadScene("Passwords_Presentation");
                break;
            case "Phone-1":
                Store.Instance.quizToLoad = Store.Quiz.Phone;
                SceneManager.LoadScene("Phone_Presentation");
                break;
            case "AI-1":
                Store.Instance.quizToLoad = Store.Quiz.AI;
                SceneManager.LoadScene("AI_Presentation");
                break;
            case "Malware-2":
                Store.Instance.quizToLoad = Store.Quiz.Firewall;
                break;
            case "Privacy-2":
                Store.Instance.quizToLoad = Store.Quiz.Cookies;
                SceneManager.LoadScene("Cookies_Presentation");
                break;
            case "Encryption-2":
                Store.Instance.quizToLoad = Store.Quiz.TFA;
                SceneManager.LoadScene("TFA_Presentation");
                break;
            default:
                throw new System.Exception("Unknown level: " + level);
        }
    }
}
