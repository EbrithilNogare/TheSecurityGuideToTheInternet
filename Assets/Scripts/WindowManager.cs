using UnityEngine;

public class WindowManager : MonoBehaviour {
    public void CloseWindow(GameObject window) {
        window.SetActive(false);
    }
    public void OpenWindow(GameObject window) {
        window.SetActive(true);
    }
}
