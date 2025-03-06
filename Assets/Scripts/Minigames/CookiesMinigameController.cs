using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CookieIngredient
{
    Chocolate1, Chocolate2, Chocolate3,
    Egg1, Egg2, Egg3,
    Flour1, Flour2, Flour3,
    Milk1, Milk2, Milk3,
}

public class CookiesMinigameController : MonoBehaviour
{
    public TextMeshProUGUI bowlLabel;
    public GameObject dropZone;
    public GameObject draggingParent;
    public List<GameObject> bowlIngredients;
    public List<Image> cookieScoreImages;
    public List<CookiesMinigameDragable> dragableObjects;

    List<CookiesMinigameDragable> cookieIngredientsInBowl = new List<CookiesMinigameDragable>();
    int currentCookieCategoryIndex = 0;


    public void CookieAddedToBowl(CookiesMinigameDragable cookieIngredient)
    {
        Debug.Log("CookieAddedToBowl: " + cookieIngredient);
        cookieIngredientsInBowl.Add(cookieIngredient);
    }

    public void RestartIngredients()
    {
        foreach (var item in cookieIngredientsInBowl)
        {
            item.Reset();
        }

        cookieIngredientsInBowl.Clear();

        foreach (var bowlIngredient in bowlIngredients)
        {
            bowlIngredient.SetActive(false);
        }

        // todo reset other things

    }

    public void StartAgain()
    {
        RestartIngredients();
    }

    public void FinishAndBake()
    {
        // todo finish and bake

        currentCookieCategoryIndex++;
        cookieIngredientsInBowl.Clear();

        if (currentCookieCategoryIndex < cookieScoreImages.Count)
        {
            // todo evaluate score
        }
    }
}
