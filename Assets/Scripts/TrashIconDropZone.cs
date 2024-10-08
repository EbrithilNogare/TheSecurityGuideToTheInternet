using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrashIconDropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image image;
    private Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalColor = image.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            image.color = originalColor.WithAlphaMultiplied(2.0f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            image.color = originalColor;
        }
    }

}
