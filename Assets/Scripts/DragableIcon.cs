using UnityEngine;
using UnityEngine.EventSystems;

public class DragableIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform trashDropZone;

    CanvasGroup canvasGroup;
    RectTransform rectTransform;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.5f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta * 2.9f; // todo, make it precise by scale, canvas width, scale and in screen converage of canvas
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        if (RectTransformUtility.RectangleContainsScreenPoint(trashDropZone, Input.mousePosition, Camera.main))
        {
            gameObject.SetActive(false);
        }
    }
}
