using System.Collections.Generic;
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
    public List<CookiesMinigameDragable> dragableObjects;
    public GameObject dropZone;
    public GameObject draggingParent;
    public List<Image> cookieScoreImages;

    List<CookiesMinigameDragable> cookieIngredientsInBowl = new List<CookiesMinigameDragable>();
    int currentCookieCategoryIndex = 0;


    public void CookieAddedToBowl(CookiesMinigameDragable cookieIngredient)
    {
        Debug.Log("CookieAddedToBowl: " + cookieIngredient);
        cookieIngredientsInBowl.Add(cookieIngredient);
    }

    public void StartAgain()
    {
        foreach (var item in cookieIngredientsInBowl)
        {
            item.Reset();
        }

        cookieIngredientsInBowl.Clear();

        // todo reset other things
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
