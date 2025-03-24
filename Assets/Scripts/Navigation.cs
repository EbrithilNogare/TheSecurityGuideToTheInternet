using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour {
    public void GoToMainMenu() {
        LoggingService.Log(LoggingService.LogCategory.Navigation, "Navigated to MainMenu");
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToGame() {
        LoggingService.Log(LoggingService.LogCategory.Navigation, "Navigated to Game");
        SceneManager.LoadScene("Game");
    }

    public void GoToLevelSelection() {
        LoggingService.Log(LoggingService.LogCategory.Navigation, "Navigated to LevelSelection");
        SceneManager.LoadScene("LevelSelection");
    }

    public void GoToTutorial() {
        LoggingService.Log(LoggingService.LogCategory.Navigation, "Navigated to Quiz");
        Store.Instance.quizToLoad = Store.Quiz.All;
        SceneManager.LoadScene("Quiz");
    }

    public void GoToShop() {
        LoggingService.Log(LoggingService.LogCategory.Navigation, "Navigated to Shop");
        SceneManager.LoadScene("Shop");
    }

    public void GoToSettings() {
        LoggingService.Log(LoggingService.LogCategory.Navigation, "Navigated to Settings");
        SceneManager.LoadScene("Settings");
    }
}
