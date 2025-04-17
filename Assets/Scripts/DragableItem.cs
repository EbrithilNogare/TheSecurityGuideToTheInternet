using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform[] dropZones;
    public UnityEvent[] dropEvents;
    public UnityEvent onMissDrop;
    public Transform dragableParent;

    private Vector3 originalPosition;
    private Transform originalParent;
    private bool isDragging = false;


    void Start()
    {
        originalPosition = transform.position;
        originalParent = transform.parent;
        if (dragableParent == null)
        {
            dragableParent = originalParent;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        transform.SetParent(dragableParent);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            Vector3 newPosition = eventData.position;
            newPosition.z = 0;
            transform.position = newPosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        bool droppedOnZone = false;
        for (int i = 0; i < dropZones.Length; i++)
        {
            if (dropZones[i].gameObject.activeSelf && RectTransformUtility.RectangleContainsScreenPoint(dropZones[i], eventData.position))
            {
                transform.SetParent(dropZones[i]);
                transform.localPosition = Vector3.zero;
                dropEvents[i].Invoke();
                droppedOnZone = true;
                break;
            }
        }
        if (!droppedOnZone)
        {
            transform.SetParent(originalParent);
            transform.position = originalPosition;
            onMissDrop.Invoke();
        }
    }
}