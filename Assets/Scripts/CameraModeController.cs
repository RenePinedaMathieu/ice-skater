using UnityEngine;

public class CameraModeController : MonoBehaviour
{
    [Header("Cameras")]
    public Camera thirdPersonCamera;   // your current main camera
    public Camera firstPersonCamera;   // the new FP camera

    public bool isFirstPerson = false;

    private void Start()
    {
        UpdateCameraMode();
    }

    public void TogglePowerMode()
    {
        isFirstPerson = !isFirstPerson;
        UpdateCameraMode();
    }

    private void UpdateCameraMode()
    {
        if (thirdPersonCamera != null)
            thirdPersonCamera.gameObject.SetActive(!isFirstPerson);

        if (firstPersonCamera != null)
            firstPersonCamera.gameObject.SetActive(isFirstPerson);
    }
}
