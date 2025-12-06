using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class PowerBall : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 5f;

    private Rigidbody rb;
    private CameraModeController controller;

    // Llamado desde CameraModeController cuando se crea la bola
    public void Init(CameraModeController ctrl)
    {
        controller = ctrl;
        rb = GetComponent<Rigidbody>();

        // por si acaso, destruimos la bola después de X segundos
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        if (controller == null || rb == null) return;

        // Input del joystick de la bola
        Vector2 input = controller.GetBallInput();
        Vector3 dir = new Vector3(input.x, 0f, input.y);

        if (dir.sqrMagnitude > 1f)
            dir.Normalize();

        rb.linearVelocity = dir * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si impacta al monstruo
        if (other.CompareTag("Monster"))
        {
            Destroy(other.gameObject); // mata al monstruo
            Destroy(gameObject);       // destruye la bola
        }
        else if (!other.isTrigger)
        {
            // Cualquier otra cosa sólida (pared, piso, etc.)
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (controller != null)
        {
            controller.OnBallDestroyed();
        }
    }
}
