using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AVFileStructure : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public TextMeshProUGUI label;
    public Image image;
    public bool isVirus;
    public AVManager avManager;

    [NonSerialized]
    public CanvasGroup canvasGroup;
    [NonSerialized]
    public RectTransform rectTransform;


    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        transform.SetParent(avManager.dragAndDropTemporaryParent.transform);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.5f;
        transform.localScale = Vector3.one;
    }

    public void OnDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition += eventData.delta / (new Vector2(rectTransform.lossyScale.x, rectTransform.lossyScale.y));
    }

    public void OnEndDrag(PointerEventData eventData) {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        avManager.EvaluateDrop(this);
    }
}