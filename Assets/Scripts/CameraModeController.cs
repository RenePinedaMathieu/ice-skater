using UnityEngine;

public class CameraModeController : MonoBehaviour
{
    [Header("Cameras")]
    public Camera thirdPersonCamera;   // your current main camera
    public Camera firstPersonCamera;   // FP camera on player head

    [Header("Player")]
    public SimplePlayerMove playerMovement; // assign the SimplePlayerMove here

    [Header("UI")]
    public GameObject crosshair;       // UI crosshair object

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
        // Cameras
        if (thirdPersonCamera != null)
            thirdPersonCamera.gameObject.SetActive(!isFirstPerson);

        if (firstPersonCamera != null)
            firstPersonCamera.gameObject.SetActive(isFirstPerson);

        // Movement
        if (playerMovement != null)
            playerMovement.allowMovement = !isFirstPerson;

        // Crosshair
        if (crosshair != null)
            crosshair.SetActive(isFirstPerson);
    }
}
