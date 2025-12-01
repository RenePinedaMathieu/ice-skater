using UnityEngine;

public class SimplePlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Animator animator;
    private bool isWalking = false;  // para no reiniciar la animación cada frame

    private void Awake()
    {
        // Busca un Animator en este objeto o en sus hijos (Mixamo style)
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("SimplePlayerMove: No encontré Animator en el Player o sus hijos");
        }
    }

    private void Update()
    {
        // Input teclado (WASD / flechas)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Dirección en plano XZ
        Vector3 dir = new Vector3(h, 0f, v);

        if (dir.sqrMagnitude > 1f)
            dir.Normalize();

        // Mover al personaje
        transform.position += dir * moveSpeed * Time.deltaTime;

        // Rotar hacia donde se mueve
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                10f * Time.deltaTime
            );
        }

        // ==== ANIMACIÓN: cambiar entre Idle y Walk solo cuando toque ====
        if (animator != null)
        {
            bool wantsToWalk = dir.sqrMagnitude > 0.001f;

            if (wantsToWalk && !isWalking)
            {
                // Pasamos de Idle -> Walk
                animator.Play("Walk");
                isWalking = true;
            }
            else if (!wantsToWalk && isWalking)
            {
                // Pasamos de Walk -> Idle
                animator.Play("Idle");
                isWalking = false;
            }
        }
    }
}
