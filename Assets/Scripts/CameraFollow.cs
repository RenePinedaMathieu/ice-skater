using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 8f, -6f);
    public float followSpeed = 10f;
    public float fixedPitch = 45f;  // inclinación hacia abajo

    void LateUpdate()
    {
        if (target == null) return;

        // Seguir la posición del player
        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);

        // Rotación fija (no gira con el personaje)
        transform.rotation = Quaternion.Euler(fixedPitch, 0f, 0f);
    }
}
