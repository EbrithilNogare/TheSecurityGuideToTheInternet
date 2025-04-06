using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AIMinigameController : MonoBehaviour
{
    public CanvasGroup responseOKCanvasGroup;
    public CanvasGroup responseBadCanvasGroup;
    public TextMeshProUGUI responseOKText;
    public TextMeshProUGUI responseBadText;
    public GameObject prompt;
    public AIMinigameCensorable[] mustBeVisible;
    public AIMinigameCensorable[] mustBeCensored;
    public GameObject interactionLock;
    public GameObject summaryScreen;
    public TextMeshProUGUI summaryLeaked;
    public TextMeshProUGUI summarySafes;

    [NonSerialized] public bool isSubmitted = false;
    private float messageRevealSpeed = 0.5f;
    private float messageTypingSpeed = 20.0f; // chars per second

    void Start()
    {

    }

    public void SubmitButonPressed()
    {
        if (isSubmitted)
            return;

        bool visibleValid = true;
        bool censoredValid = true;

        for (int i = 0; i < mustBeVisible.Length; i++)
        {
            if (mustBeVisible[i].isCensored)
            {
                visibleValid = false;
            }
        }

        for (int i = 0; i < mustBeCensored.Length; i++)
        {
            if (!mustBeCensored[i].isCensored)
            {
                censoredValid = false;
            }
        }

        LoggingService.Log(LoggingService.LogCategory.Minigame, "{\"message\":\"Submit button pressed\",\"visibleValid\":" + visibleValid.ToString() + ",\"censoredValid\":" + censoredValid.ToString() + "}");

        if (visibleValid)
        {
            ContinueWithPrompt(censoredValid);
        }
        else
        {
            ShowBadResponse();
        }

    }

    public void FinishMinigame(int score)
    {
        LoggingService.Log(LoggingService.LogCategory.Minigame, "{\"message\":\"AI minigame completed\",\"score\":" + score + "}");
        Store.Instance.minigameStars = score;
        int scoreForStore = score == 1 ? 0b100 : 0b110;
        Store.Instance.SetLevelScore(Store.Level.AI, scoreForStore);
        Store.Instance.quizToLoad = Store.Quiz.AI;

        DOVirtual.DelayedCall(2.5f, () => SceneManager.LoadScene("Quiz"));
    }

    private void ShowBadResponse()
    {
        responseBadText.maxVisibleCharacters = 0;
        responseBadCanvasGroup.alpha = 0;
        responseBadCanvasGroup.DOFade(1, messageRevealSpeed).OnComplete(() =>
        {
            int length = responseBadText.text.Length;
            DOTween.To(() => 0, i =>
            {
                responseBadText.maxVisibleCharacters = i;
            }, length, length / messageTypingSpeed).SetEase(Ease.Linear);
        });
    }

    private void ContinueWithPrompt(bool censoredValid)
    {
        isSubmitted = true;
        interactionLock.SetActive(true);
        prompt.SetActive(false);
        responseBadCanvasGroup.alpha = 0;
        responseOKText.maxVisibleCharacters = 0;
        responseOKCanvasGroup.DOFade(1, messageRevealSpeed).OnComplete(() =>
        {
            int length = responseOKText.text.Length;
            DOTween.To(() => 0, i =>
            {
                responseOKText.maxVisibleCharacters = i;
            }, length, length / messageTypingSpeed).SetEase(Ease.Linear).OnComplete(() => RevealSummaryScrean(censoredValid));
        });
    }

    private void RevealSummaryScrean(bool censoredValid)
    {
        summaryScreen.SetActive(true);
        summaryLeaked.maxVisibleCharacters = 0;
        summarySafes.maxVisibleCharacters = 0;

        DOVirtual.DelayedCall(1f, () =>
        {
            TextMeshProUGUI text = censoredValid ? summarySafes : summaryLeaked;
            int length = text.text.Length;
            DOTween.To(() => 0, i => { text.maxVisibleCharacters = i; }, length, length / messageTypingSpeed).SetEase(Ease.Linear).OnComplete(() => FinishMinigame(censoredValid ? 2 : 1));
        });
    }
}
