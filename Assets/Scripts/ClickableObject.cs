using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickableObject : MonoBehaviour {

    [SerializeField] private UnityEvent unityEvent;
    [SerializeField] private GraphicRaycaster canvas;
    private BoxCollider boxCollider;

    void Start() {
        boxCollider = GetComponent<BoxCollider>();
    }


    void OnMouseDown() {
        if (EventSystem.current.IsPointerOverGameObject()) {
            // clicked on UI, which has priority
            return;
        }

        unityEvent.Invoke();
    }
    public void TurnCanvasOnOff(Boolean turnOn) {
        if (canvas != null) {
            canvas.enabled = turnOn;
        }
    }
    public void TurnColiderOnOff(Boolean turnOn) {
        if (boxCollider != null) {
            boxCollider.enabled = turnOn;
        }
    }
}
