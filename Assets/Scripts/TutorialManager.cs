using UnityEngine;

public class TutorialManager : MonoBehaviour {
    public string minigameName;
    public GameObject tutorialDialog;

    public void ShowTutorialDialog() {
        LoggingService.Log(LoggingService.LogCategory.Minigame, "Tutorial dialog opened in minigame: " + minigameName);
        tutorialDialog.SetActive(true);
    }

    public void CloseTutorialDialog() {
        LoggingService.Log(LoggingService.LogCategory.Minigame, "Tutorial dialog closed in minigame: " + minigameName);
        tutorialDialog.SetActive(false);
    }
}
