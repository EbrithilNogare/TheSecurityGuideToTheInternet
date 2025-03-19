using UnityEngine;
using UnityEngine.EventSystems;

public class CookiesMinigameDragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public CookieIngredient cookieIngredient;

    Canvas canvas;
    CanvasGroup canvasGroup;
    RectTransform rectTransform;
    CookiesMinigameManager cookiesMinigameController;
    RectTransform dropZone;
    Transform draggingParent;
    Vector2 originalPosition;
    Transform originalParent;

    void Awake() {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>() != null ? GetComponent<CanvasGroup>() : gameObject.AddComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        cookiesMinigameController = FindObjectOfType<CookiesMinigameManager>();
        dropZone = cookiesMinigameController.dropZone.GetComponent<RectTransform>();
        draggingParent = cookiesMinigameController.draggingParent.transform;
        originalParent = rectTransform.parent;
    }

    public void Reset() {
        rectTransform.localPosition = originalPosition;
        transform.SetParent(originalParent);
        gameObject.SetActive(true);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f;
        originalPosition = rectTransform.localPosition;
        rectTransform.SetParent(draggingParent);
    }

    public void OnDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition += eventData.delta / (new Vector2(rectTransform.lossyScale.x, rectTransform.lossyScale.y));
    }

    public void OnEndDrag(PointerEventData eventData) {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        transform.SetParent(originalParent);
        EvaluateDrop();
    }

    private void EvaluateDrop() {
        if (RectTransformUtility.RectangleContainsScreenPoint(dropZone, Input.mousePosition, canvas.worldCamera)) {
            // droped in drop zone
            rectTransform.localPosition = originalPosition;
            gameObject.SetActive(false);

            // send information to controller
            cookiesMinigameController.CookieAddedToBowl(this);
        }
        else {
            // droped somewhere else
            rectTransform.localPosition = originalPosition;
        }
    }
}
