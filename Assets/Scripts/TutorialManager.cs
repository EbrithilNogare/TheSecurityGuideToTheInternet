using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public Store.Level minigame;
    public GameObject tutorialDialog;
    private string minigameName;


    public void Start()
    {
        if (!Store.Instance.IsTutorialDisplayed(minigame))
        {
            Store.Instance.SetTutorialDisplayed(minigame);
            ShowTutorialDialog();
            return;
        }
        minigameName = minigame.ToString();
    }

    public void ShowTutorialDialog()
    {
        LoggingService.Log(LoggingService.LogCategory.Minigame, "Tutorial dialog opened in minigame: " + minigameName);
        tutorialDialog.SetActive(true);
    }

    public void CloseTutorialDialog()
    {
        LoggingService.Log(LoggingService.LogCategory.Minigame, "Tutorial dialog closed in minigame: " + minigameName);
        tutorialDialog.SetActive(false);
    }
}
