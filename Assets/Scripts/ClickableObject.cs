using UnityEngine;
using UnityEngine.Events;

public class ClickableObject : MonoBehaviour
{

    [SerializeField] private UnityEvent unityEvent;

    void OnMouseDown()
    {
        unityEvent.Invoke();
    }
}
