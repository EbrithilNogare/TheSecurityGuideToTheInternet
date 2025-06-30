using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;


public class LevelCompletedDialogController : MonoBehaviour
{
    private class StringArgs
    {
        public int Points { get; set; }
    }

    public Color enabledStar;
    public Color disabledStar;

    public Image minigameStar0;
    public Image minigameStar1;
    public Image quizStar;
    public LocalizeStringEvent localizedStringEvent;

    private void Awake()
    {
        bool shouldOpenLevelCompletedDialog = Store.Instance.shouldOpenLevelCompletedDialog;
        if (shouldOpenLevelCompletedDialog)
        {
            Store.Instance.shouldOpenLevelCompletedDialog = false;
            OpenDialog();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OpenDialog()
    {
        gameObject.SetActive(true);
        var minigameStars = Store.Instance.minigameStars;
        var quizStars = Store.Instance.quizStars;
        var quizScore = Store.Instance.quizScore;

        LoggingService.Log(LoggingService.LogCategory.Navigation, "{\"message\":\"Displayed level completed dialog\",\"minigameStars\":" + minigameStars + ",\"quizStars\":" + quizStars + ",\"quizScore\":" + quizScore + "}");

        minigameStar0.color = minigameStars >= 1 ? enabledStar : disabledStar;
        minigameStar1.color = minigameStars >= 2 ? enabledStar : disabledStar;
        quizStar.color = Store.Instance.quizStars >= 1 ? enabledStar : disabledStar;

        var localizedString = localizedStringEvent.StringReference;
        (localizedString["Points"] as IntVariable).Value = quizScore;
    }

    public void CloseDialog()
    {
        gameObject.SetActive(false);
    }
}
