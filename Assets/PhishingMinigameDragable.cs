using UnityEngine;
using UnityEngine.EventSystems;

public class PhishingMinigameDragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PhishingMinigameManager.itemName objectName;
    Canvas canvas;
    CanvasGroup canvasGroup;
    RectTransform rectTransform;
    private bool isOriginal = true;
    PhishingMinigameManager phishingMinigameManager;
    RectTransform websiteContainer;
    RectTransform trashDropZone;
    Vector3 originalPosition;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>() != null ? GetComponent<CanvasGroup>() : gameObject.AddComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        phishingMinigameManager = FindObjectOfType<PhishingMinigameManager>();
        websiteContainer = phishingMinigameManager.websiteContainer.GetComponent<RectTransform>();
        trashDropZone = phishingMinigameManager.trashDropZone.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.5f;
        originalPosition = rectTransform.localPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / (new Vector2(rectTransform.lossyScale.x, rectTransform.lossyScale.y));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // drop into website container
        if (RectTransformUtility.RectangleContainsScreenPoint(websiteContainer, Input.mousePosition, canvas.worldCamera))
        {
            if (isOriginal)
            {
                var copyForContainer = Instantiate(gameObject, websiteContainer, true);
                copyForContainer.GetComponent<PhishingMinigameDragable>().isOriginal = false;
                rectTransform.localPosition = originalPosition;
            }
            phishingMinigameManager.EvaluateTemplate();
            return;
        }

        // drop into trash
        if (RectTransformUtility.RectangleContainsScreenPoint(trashDropZone, Input.mousePosition, canvas.worldCamera))
        {
            if (isOriginal)
            {
                rectTransform.localPosition = originalPosition;
            }
            else
            {
                Destroy(gameObject);
            }
            phishingMinigameManager.EvaluateTemplate();
            return;
        }

        // droped somewhere else
        if (isOriginal)
        {
            rectTransform.localPosition = originalPosition;
        }
        else
        {
            rectTransform.localPosition = originalPosition;
        }
        phishingMinigameManager.EvaluateTemplate();
    }
}
