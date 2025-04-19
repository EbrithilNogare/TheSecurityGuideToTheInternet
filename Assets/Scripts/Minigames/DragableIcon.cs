using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DragableIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform trashDropZone;

    Canvas canvas;
    CanvasGroup canvasGroup;
    RectTransform rectTransform;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.5f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            Camera.main,
            out localPoint
        );

        rectTransform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        Vector2 screenPosition = Mouse.current.position.ReadValue();

        if (trashDropZone.gameObject.activeSelf && RectTransformUtility.RectangleContainsScreenPoint(trashDropZone, screenPosition, Camera.main))
        {
            gameObject.SetActive(false);
        }
    }
}
