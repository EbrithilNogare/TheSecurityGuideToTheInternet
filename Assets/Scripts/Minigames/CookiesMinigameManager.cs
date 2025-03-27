using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum CookieIngredient {
    Chocolate1, Chocolate2, Chocolate3,
    Egg1, Egg2, Egg3,
    Flour1, Flour2, Flour3,
    Milk1, Milk2, Milk3,
}

public class CookiesMinigameManager : MonoBehaviour {
    public GameObject dropZone;
    public GameObject draggingParent;
    public Button finishAndBakeButton;
    public List<GameObject> bowlIngredients;
    public List<GameObject> bowlLabels;
    public List<Image> cookieScoreImages;

    List<CookiesMinigameDragable> cookieIngredientsInBowl = new List<CookiesMinigameDragable>();
    int currentCookieCategoryIndex = 0;
    int correctCookiesFinished = 0;
    List<CookieIngredient[]> correctIngredients = new List<CookieIngredient[]>()
    {
        new CookieIngredient[] { CookieIngredient.Milk1, CookieIngredient.Egg2, CookieIngredient.Flour2, CookieIngredient.Chocolate1 },
        new CookieIngredient[] { CookieIngredient.Milk2, CookieIngredient.Egg1, CookieIngredient.Flour3, CookieIngredient.Chocolate3 },
        new CookieIngredient[] { CookieIngredient.Milk3, CookieIngredient.Egg3, CookieIngredient.Flour1, CookieIngredient.Chocolate2 },
    };

    void Start() {
        RestartIngredients();
    }

    public void CookieAddedToBowl(CookiesMinigameDragable cookieIngredient) {
        LoggingService.Log(LoggingService.LogCategory.Minigame, "{\"message\":\"Cookie added to the bowl\",\"cookieIngredient\":\"" + cookieIngredient.cookieIngredient.ToString() + "\",\"currentCookieCategoryIndex\":" + currentCookieCategoryIndex.ToString() + ",\"correct\":" + (correctIngredients[currentCookieCategoryIndex].Contains(cookieIngredient.cookieIngredient) ? "1" : "0") + "}");
        cookieIngredientsInBowl.Add(cookieIngredient);

        switch (cookieIngredient.cookieIngredient) {
            case CookieIngredient.Milk1:
            case CookieIngredient.Milk2:
            case CookieIngredient.Milk3:
                bowlIngredients[0].SetActive(true);
                break;
            case CookieIngredient.Egg1:
            case CookieIngredient.Egg2:
            case CookieIngredient.Egg3:
                bowlIngredients[1].SetActive(true);
                break;
            case CookieIngredient.Flour1:
            case CookieIngredient.Flour2:
            case CookieIngredient.Flour3:
                bowlIngredients[2].SetActive(true);
                break;
            case CookieIngredient.Chocolate1:
            case CookieIngredient.Chocolate2:
            case CookieIngredient.Chocolate3:
                bowlIngredients[3].SetActive(true);
                break;
        }

        finishAndBakeButton.interactable = cookieIngredientsInBowl.Count >= 4;
    }

    public void RestartIngredients() {
        LoggingService.Log(LoggingService.LogCategory.Minigame, "{\"message\":\"Cooking restarted\"}");

        foreach (var item in cookieIngredientsInBowl) {
            item.Reset();
        }
        cookieIngredientsInBowl.Clear();

        foreach (var bowlIngredient in bowlIngredients) {
            bowlIngredient.SetActive(false);
        }

        foreach (var bowlLabel in bowlLabels) {
            bowlLabel.SetActive(false);
        }
        bowlLabels[currentCookieCategoryIndex].SetActive(true);

        finishAndBakeButton.interactable = cookieIngredientsInBowl.Count >= 4;
    }

    public void StartAgain() {
        RestartIngredients();
    }

    public void FinishAndBake() {
        LoggingService.Log(LoggingService.LogCategory.Minigame, "{\"message\":\"Cooking finished\"}");
        // check if the ingredients are correct
        bool correct = true;
        for (int i = 0; i < cookieIngredientsInBowl.Count; i++) {
            if (!correctIngredients[currentCookieCategoryIndex].Contains(cookieIngredientsInBowl[i].cookieIngredient)) {
                correct = false;
                break;
            }
        }

        if (correct) {
            correctCookiesFinished += 1;
            cookieScoreImages[currentCookieCategoryIndex].color = Color.green;
        }
        else {
            cookieScoreImages[currentCookieCategoryIndex].color = Color.red;
        }

        if (currentCookieCategoryIndex + 1 >= cookieScoreImages.Count) {
            FinishMinigame();
            return;
        }


        currentCookieCategoryIndex++;
        RestartIngredients();
    }


    public void FinishMinigame() {
        int score = correctCookiesFinished == 0 ? 0 : correctCookiesFinished < 2 ? 1 : 2;
        LoggingService.Log(LoggingService.LogCategory.Minigame, "{\"message\":\"Cookie minigame completed\",\"correctCookiessFinished\":" + correctCookiesFinished + ",\"score\":" + score + "}");
        Store.Instance.minigameStars = score;
        int scoreForStore = score == 0 ? 0b000 : score == 1 ? 0b100 : 0b110;
        Store.Instance.SetLevelScore(Store.Level.Cookies, scoreForStore);
        Store.Instance.quizToLoad = Store.Quiz.Cookies;

        DOVirtual.DelayedCall(2, () => SceneManager.LoadScene("Quiz"));
    }
}
