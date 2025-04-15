using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoldableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent onHoldStart;
    public UnityEvent onHoldEnd;

    private bool isHolding;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isHolding)
            return;

        isHolding = true;
        onHoldStart.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isHolding)
            return;

        isHolding = false;
        onHoldEnd.Invoke();
    }
}
