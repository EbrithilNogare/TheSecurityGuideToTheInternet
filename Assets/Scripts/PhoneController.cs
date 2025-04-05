using UnityEngine;

public class PhoneController : MonoBehaviour
{

    public CameraOperator CameraOperator;

    void Start()
    {

    }

    void Update()
    {

    }

    public void LeavePhone()
    {
        CameraOperator.FocusOnRoom();
    }

    public void FocusOnPhone()
    {
        CameraOperator.FocusOnPhone();
    }
}
