using UnityEngine;

public class DesktopManager : MonoBehaviour {
    public void AddIconOnDesktop(GameObject icon) {
        icon.SetActive(true);
    }
    public void RemoveIconFromDesktop(GameObject icon) {
        icon.SetActive(false);
    }
}
