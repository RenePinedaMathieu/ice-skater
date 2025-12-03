using UnityEngine;

public class EnemyChasePlayer : MonoBehaviour
{
    public Transform player;         // referencia al player
    public float speed = 3f;         // velocidad de persecución
    public float detectionRadius = 15f; // distancia a la que empieza a perseguir
    public float stopDistance = 1.5f;   // distancia mínima (no se sube encima del player)

    private void Update()
    {
        if (player == null) return;

        // Vector en el plano XZ hacia el player
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;  // no nos importa la altura

        float distance = toPlayer.magnitude;

        // Si está fuera del radio, no hacer nada
        if (distance > detectionRadius)
            return;

        // Rotar para mirar al player
        if (toPlayer.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(toPlayer);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                5f * Time.deltaTime
            );
        }

        // Si está a una distancia razonable, avanzar
        if (distance > stopDistance)
        {
            Vector3 dir = toPlayer.normalized;
            transform.position += dir * speed * Time.deltaTime;
        }
    }
}
