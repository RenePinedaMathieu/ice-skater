using UnityEngine;

public class EnemyHitPlayer : MonoBehaviour
{
    private GameManager gameManager;

    // 👇 aquí guardamos la posición y rotación inicial del monstruo
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("EnemyHitPlayer: no se encontró GameManager en la escena");
        }

        // posición/rotación inicial del monstruo
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Solo reaccionamos al Player
        if (!other.CompareTag("Player")) return;

        // 1) Reseteamos AL MONSTRUO a su posición original
        transform.position = startPosition;
        transform.rotation = startRotation;

        // 2) Avisar al GameManager que el jugador fue golpeado (y lo resetea)
        gameManager.PlayerHit();
    }
}
