using UnityEngine;

public class DragAndDrop : MonoBehaviour
{

    Vector3 mousePosition;

    private void OnMouseDown()
    {
        mousePosition = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
    }
    private void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
    }
}
