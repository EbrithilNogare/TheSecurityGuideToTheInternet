using Cinemachine;
using UnityEngine;

public class CameraOperator : MonoBehaviour {
    public CinemachineVirtualCamera RoomCamera;
    public CinemachineVirtualCamera MonitorCamera;
    public CinemachineVirtualCamera PhoneCamera;

    private void ResetFocus() {
        RoomCamera.Priority = 0;
        MonitorCamera.Priority = 0;
        PhoneCamera.Priority = 0;
    }

    public void FocusOnRoom() {
        ResetFocus();
        RoomCamera.Priority = 1;
    }

    public void FocusOnMonitor() {
        ResetFocus();
        MonitorCamera.Priority = 1;
    }

    public void FocusOnPhone() {
        ResetFocus();
        PhoneCamera.Priority = 1;
    }
}
