using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    public Transform playerBody;  // the Player transform
    public float sensitivity = 150f;
    public float verticalClamp = 60f;

    private float xRotation = 0f;

    private void Start()
    {
        // start from current rotation
        Vector3 angles = transform.localEulerAngles;
        xRotation = angles.x;
    }

    private void Update()
    {
        // only rotate while mouse button / touch pressed
        if (!Input.GetMouseButton(0))
            return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);

        // look up/down with the camera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // rotate the player horizontally
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
