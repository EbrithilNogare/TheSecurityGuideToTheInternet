using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour
{
    public void GoToMainMenu()
    {
        LoggingService.Log(LoggingService.LogCategory.NAVIGATION, "Navigated to MainMenu");
        SceneManager.LoadScene("MainMenu");
    }
    public void GoToGame()
    {
        LoggingService.Log(LoggingService.LogCategory.NAVIGATION, "Navigated to Game");
        SceneManager.LoadScene("Game");
    }
    public void GoToLevelSelection()
    {
        LoggingService.Log(LoggingService.LogCategory.NAVIGATION, "Navigated to LevelSelection");
        SceneManager.LoadScene("LevelSelection");
    }
    public void GoToTutorial()
    {
        LoggingService.Log(LoggingService.LogCategory.NAVIGATION, "Navigated to Tutorial");
        SceneManager.LoadScene("Tutorial");
    }
    public void GoToShop()
    {
        LoggingService.Log(LoggingService.LogCategory.NAVIGATION, "Navigated to Shop");
        SceneManager.LoadScene("Shop");
    }
    public void GoToSettings()
    {
        LoggingService.Log(LoggingService.LogCategory.NAVIGATION, "Navigated to Settings");
        SceneManager.LoadScene("Settings");
    }
}
