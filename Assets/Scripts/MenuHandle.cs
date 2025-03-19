using DG.Tweening;
using UnityEngine;

public class MenuHandler : MonoBehaviour {
    public GameObject targetObject;
    public float duration = 0.5f;

    private bool isHidden = false;
    private Vector3 hiddenPosition;
    private Vector3 revealedPosition;
    private RectTransform rectTransform;

    void Start() {
        rectTransform = targetObject.GetComponent<RectTransform>();
        revealedPosition = rectTransform.localPosition;
        hiddenPosition = revealedPosition + new Vector3(rectTransform.rect.width, 0, 0);
    }

    public void ToggleObject() {
        if (isHidden)
            rectTransform.DOLocalMove(revealedPosition, duration);
        else
            rectTransform.DOLocalMove(hiddenPosition, duration);

        isHidden = !isHidden;
    }
}
