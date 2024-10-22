using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
    public string activeLevel = "";

    public static readonly List<string> LevelCategories = new List<string> {
        "Malware-1", "Privacy-1", "Phone-1", "Encryption-1",
        "Malware-2", "Privacy-2", "Phone-2", "Encryption-2",
    };
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
                activeLevel = level;
                SceneManager.LoadScene("Malware_Antivirus_Presentation");
                break;
            case "Privacy-1":
                activeLevel = level;
                SceneManager.LoadScene("Library");
                break;
            case "Phone-1": break;
            case "Encryption-1": break;
            default:
                throw new System.Exception("Unknown level: " + level);
        }
    }
}
