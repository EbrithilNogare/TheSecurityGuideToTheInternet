using UnityEngine;

public class AIMinigameController : MonoBehaviour {

    public GameObject responseOK;
    public GameObject responseBad;
    public GameObject prompt;
    public AIMinigameCensorable[] mustBeVisible;
    public AIMinigameCensorable[] mustBeCensored;

    void Start() {

    }

    public void SubmitButonPressed() {
        bool visibleValid = true;
        bool censoredValid = true;

        for (int i = 0; i < mustBeVisible.Length; i++) {
            if (mustBeVisible[i].isCensored) {
                visibleValid = false;
            }
        }

        for (int i = 0; i < mustBeCensored.Length; i++) {
            if (!mustBeCensored[i].isCensored) {
                censoredValid = false;
            }
        }

        LoggingService.Log(LoggingService.LogCategory.Minigame, "{\"message\":\"Submit button pressed\",\"visibleValid\":" + visibleValid.ToString() + ",\"censoredValid\":" + censoredValid.ToString() + "}");

    }

    public void FinishMinigame() {

    }
}
