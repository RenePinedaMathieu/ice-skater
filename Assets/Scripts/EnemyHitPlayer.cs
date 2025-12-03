using UnityEngine;

public class EnemyHitPlayer : MonoBehaviour
{
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("EnemyHitPlayer: no se encontró GameManager en la escena");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Solo reaccionamos al Player
        if (!other.CompareTag("Player")) return;

        // Avisar al GameManager que el jugador fue golpeado
        gameManager.PlayerHit();
    }
}
