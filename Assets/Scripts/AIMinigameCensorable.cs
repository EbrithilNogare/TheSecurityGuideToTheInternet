using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AIMinigameCensorable : MonoBehaviour
{

    public GameObject prefabOfCensoreField;
    public GameObject prefabOfMarker;
    public GameObject markerParent;
    [NonSerialized] public bool isCensored = false;

    private RectTransform fieldWithSensitiveData;
    private bool animationInProgress = false;
    private float censoringSpeed = 300f;
    private GameObject censoredOverlay;

    void Start()
    {
        fieldWithSensitiveData = GetComponent<RectTransform>();
    }

    public void ClickedOnField()
    {
        if (animationInProgress)
            return;
        animationInProgress = true;

        string text = fieldWithSensitiveData.name;
        LoggingService.Log(LoggingService.LogCategory.Minigame, "{\"message\":\"Censored field\",\"filed\":\"" + text + "\",\"isCensored\":" + (!isCensored).ToString() + "}");

        if (isCensored)
        {
            UncensorThis();
        }
        else
        {
            ShowMarker();
            CensorThis();
        }

        isCensored = !isCensored;
    }

    private void ShowMarker()
    {
        var marker = Instantiate(prefabOfMarker, fieldWithSensitiveData);
        marker.transform.SetParent(markerParent.transform, true);
        var markerRect = marker.GetComponent<RectTransform>();
        float censoringDuration = fieldWithSensitiveData.rect.width / censoringSpeed;

        // todo make it in front of everything

        marker.transform.DOMoveX(markerRect.position.x + fieldWithSensitiveData.rect.width, censoringDuration).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            Destroy(marker);
            animationInProgress = false;
        });
    }

    private void CensorThis()
    {
        censoredOverlay = Instantiate(prefabOfCensoreField, fieldWithSensitiveData);
        var censoredFieldRect = censoredOverlay.GetComponent<RectTransform>();
        float censoringDuration = fieldWithSensitiveData.rect.width / censoringSpeed;

        censoredFieldRect.anchorMin = new Vector2(0, 0);
        censoredFieldRect.anchorMax = new Vector2(0, 1);
        censoredFieldRect.pivot = new Vector2(0, 0.5f);
        censoredFieldRect.anchoredPosition = Vector2.zero;
        censoredFieldRect.sizeDelta = new Vector2(0, 0);

        float targetWidth = fieldWithSensitiveData.rect.width;
        censoredFieldRect.DOSizeDelta(new Vector2(targetWidth, 0), censoringDuration).SetEase(Ease.OutCubic);
    }

    private void UncensorThis()
    {
        var censoredFieldRect = censoredOverlay.GetComponent<Image>();
        float censoringDuration = fieldWithSensitiveData.rect.width / censoringSpeed;

        censoredFieldRect.DOFade(0, censoringDuration).OnComplete(() =>
        {
            Destroy(censoredOverlay);
            animationInProgress = false;
        });
    }
}
