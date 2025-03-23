using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompletedDialogController : MonoBehaviour {

    public Color enabledStar;
    public Color disabledStar;

    public Image minigameStar0;
    public Image minigameStar1;
    public Image quizStar;
    public TextMeshProUGUI quizScoreText;


    private void Awake() {
        bool shouldOpenLevelCompletedDialog = Store.Instance.shouldOpenLevelCompletedDialog;
        if (shouldOpenLevelCompletedDialog) {
            Store.Instance.shouldOpenLevelCompletedDialog = false;
            OpenDialog();
        }
        else {
            gameObject.SetActive(false);
        }
    }

    private void OpenDialog() {
        gameObject.SetActive(true);
        minigameStar0.color = Store.Instance.minigameStars >= 1 ? enabledStar : disabledStar;
        minigameStar1.color = Store.Instance.minigameStars >= 2 ? enabledStar : disabledStar;
        quizStar.color = Store.Instance.quizStars >= 1 ? enabledStar : disabledStar;
        quizScoreText.text = Store.Instance.quizScore.ToString();
    }

    public void CloseDialog() {
        gameObject.SetActive(false);
    }
}
