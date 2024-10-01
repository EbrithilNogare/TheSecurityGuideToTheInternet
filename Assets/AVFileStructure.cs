using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AVFileStructure : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TextMeshProUGUI label;
    public Image image;
    public bool isVirus;

    [NonSerialized]
    public CanvasGroup canvasGroup;
    [NonSerialized]
    public RectTransform rectTransform;

    private AVFileManager avFileManager;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        avFileManager = GetComponentInParent<AVFileManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.5f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / avFileManager.canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        avFileManager.EvaluateDrop(this);
    }
}