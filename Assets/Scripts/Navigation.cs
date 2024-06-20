using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void GoToGame()
    {
        SceneManager.LoadScene("Game");
    }
    public void GoToLevelSelection()
    {
        SceneManager.LoadScene("LevelSelection");
    }
    public void GoToTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
    public void GoToShop()
    {
        SceneManager.LoadScene("Shop");
    }
    public void GoToSettings()
    {
        SceneManager.LoadScene("Settings");
    }
}
