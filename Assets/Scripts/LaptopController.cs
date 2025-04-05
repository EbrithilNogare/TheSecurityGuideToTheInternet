using UnityEngine;

public class LaptopController : MonoBehaviour
{

    public CameraOperator CameraOperator;

    void Start()
    {

    }

    void Update()
    {

    }

    public void LockPC()
    {
        CameraOperator.FocusOnRoom();
    }

    public void FocusOnMonitor()
    {
        CameraOperator.FocusOnMonitor();
    }
}
