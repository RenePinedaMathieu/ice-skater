using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI (opcional)")]
    public TextMeshProUGUI timerText;   // si aún no tienes timer, puedes dejarlo vacío

    [Header("Player / Respawn")]
    public Transform player;           // arrastra aquí tu Player
    public Transform respawnPoint;     // punto donde reapareces

    private bool runActive = false;
    private float startTime = 0f;

    private void Start()
    {
        // Si no tienes timer, no pasa nada
        if (timerText != null)
            timerText.text = "00.00";

        // Si no asignaste respawnPoint, usamos la posición inicial del Player
        if (respawnPoint == null && player != null)
        {
            GameObject rp = new GameObject("RespawnPoint");
            rp.transform.position = player.position;
            respawnPoint = rp.transform;
        }
    }

    private void Update()
    {
        if (runActive && timerText != null)
        {
            float t = Time.time - startTime;
            timerText.text = t.ToString("F2");
        }
    }

    // Por ahora, si quieres puedes llamar esto manualmente,
    // más adelante lo conectamos con StartPad
    public void StartRun()
    {
        startTime = Time.time;
        runActive = true;
    }

    public void FinishRun()
    {
        runActive = false;
    }

    // 👇 Este es el importante: lo llama el enemigo cuando te pega
    public void PlayerHit()
    {
        runActive = false; // por si en el futuro usas timer

        if (player != null && respawnPoint != null)
        {
            var controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                player.position = respawnPoint.position;
                controller.enabled = true;
            }
            else
            {
                player.position = respawnPoint.position;
            }
        }

        if (timerText != null)
            timerText.text = "00.00";
    }
}
