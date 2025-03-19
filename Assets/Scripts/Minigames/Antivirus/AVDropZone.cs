using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AVDropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private Image image;
    private Color originalColor;

    private void Awake() {
        image = GetComponent<Image>();
        originalColor = image.color;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (eventData.pointerDrag != null) {
            image.color = originalColor * 1.2f;
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (eventData.pointerDrag != null) {
            image.color = originalColor;
        }
    }
}
