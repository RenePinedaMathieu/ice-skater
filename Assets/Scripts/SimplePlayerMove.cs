using UnityEngine;

public class SimplePlayerMove : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float gravity = -20f;
    public float jumpForce = 8f;

    [Header("Ice Settings")]
    public float iceSpeedMultiplier = 1.5f;
    public float iceFriction = 0.5f;

    [Header("Mobile Controls")]
    public VirtualJoystick joystick;  // asignar en el inspector

    private CharacterController controller;
    private Animator animator;

    private bool isWalking = false;
    private bool isSliding = false;
    private bool isJumping = false;

    private float verticalVelocity = 0f;
    private bool onIce = false;
    private Vector3 slideDir = Vector3.zero;

    private bool jumpRequested = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        if (controller == null)
        {
            Debug.LogError("SimplePlayerMove: falta CharacterController en el Player");
        }
    }

    // Llamado por el botón de salto (JumpButton → OnClick → Player.SimplePlayerMove.OnJumpButton)
    public void OnJumpButton()
    {
        // Solo registramos la intención de saltar
        jumpRequested = true;
    }

    // Chequeo robusto de si estamos tocando el suelo
    private bool CheckGrounded(float extraDistance = 0.3f)
    {
        bool controllerGrounded = controller.isGrounded;

        bool rayGrounded = false;
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, extraDistance))
        {
            rayGrounded = true;
        }

        return controllerGrounded || rayGrounded;
    }

    private void Update()
    {
        if (controller == null) return;

        // === 1) GROUND CHECK GENERAL ===
        bool grounded = CheckGrounded();

        // === 2) DETECTAR HIELO SOLO CUANDO ESTAMOS EN EL SUELO ===
        onIce = false;
        if (grounded)
        {
            Vector3 originIce = transform.position + Vector3.up * 0.2f;
            if (Physics.Raycast(originIce, Vector3.down, out RaycastHit hitInfo, 2f))
            {
                if (hitInfo.collider.CompareTag("Ice"))
                    onIce = true;
            }
        }

        // === 3) INPUT: JOYSTICK PRIMERO, TECLADO COMO BACKUP ===
        float h = 0f;
        float v = 0f;

        if (joystick != null && joystick.Direction.sqrMagnitude > 0.01f)
        {
            Vector2 joy = joystick.Direction;
            h = joy.x;
            v = joy.y;
        }
        else
        {
            // en editor/PC
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
        }

        Vector3 inputDir = new Vector3(h, 0f, v);
        if (inputDir.sqrMagnitude > 1f)
            inputDir.Normalize();

        bool hasInput = inputDir.sqrMagnitude > 0.001f;

        // === 4) ROTACIÓN (solo si hay input horizontal) ===
        if (hasInput)
        {
            Quaternion targetRot = Quaternion.LookRotation(inputDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                10f * Time.deltaTime
            );
        }

        // === 5) SALTO + GRAVEDAD ===
        if (grounded)
        {
            // Mantenerlo pegado al suelo
            if (verticalVelocity < 0f)
                verticalVelocity = -1f;

            // Si se pidió salto, aplicarlo aunque esté idle
            if (jumpRequested)
            {
                verticalVelocity = jumpForce;
                jumpRequested = false;
                isJumping = true;

                if (animator != null)
                    animator.Play("Jump");
            }
        }
        else
        {
            // En el aire no seguimos acumulando "requests" de salto
            jumpRequested = false;
        }

        // Aplicar gravedad siempre
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 move;

        // === 6) MOVIMIENTO: HIELO VS NORMAL ===
        if (onIce && !isJumping) // solo modo hielo cuando ESTÁ en el suelo, no volando
        {
            // Modo hielo: inercia
            if (hasInput)
            {
                slideDir = inputDir;
            }
            else
            {
                slideDir = Vector3.Lerp(slideDir, Vector3.zero, iceFriction * Time.deltaTime);
            }

            Vector3 horizontal = slideDir * moveSpeed * iceSpeedMultiplier;
            move = new Vector3(horizontal.x, verticalVelocity, horizontal.z);
        }
        else
        {
            // Normal o en el aire
            Vector3 horizontal = inputDir * moveSpeed;
            move = new Vector3(horizontal.x, verticalVelocity, horizontal.z);
            if (!onIce)
                slideDir = Vector3.zero;
        }

        // === 7) MOVER AL PERSONAJE ===
        controller.Move(move * Time.deltaTime);

        // Re-chequeo de grounded después del movimiento (para cerrar el salto bien)
        bool groundedAfterMove = CheckGrounded();

        // === 8) LÓGICA DE FIN DE SALTO ===
        if (isJumping)
        {
            // si ya tocó suelo y está cayendo o quieto en Y, terminamos el salto
            if (groundedAfterMove && verticalVelocity <= 0f)
            {
                isJumping = false;
            }
        }

        // === 9) ANIMACIONES: NO PISAR JUMP MIENTRAS DURA ===
        if (animator != null && !isJumping)
        {
            bool isMovingHoriz = onIce ? (slideDir.sqrMagnitude > 0.001f) : hasInput;

            if (onIce && isMovingHoriz)
            {
                // Sliding
                if (!isSliding)
                {
                    animator.Play("Slide");
                    isSliding = true;
                    isWalking = false;
                }
            }
            else if (!onIce && isMovingHoriz)
            {
                // Walking
                if (!isWalking)
                {
                    animator.Play("Walk");
                    isWalking = true;
                    isSliding = false;
                }
            }
            else
            {
                // SIN movimiento horizontal → SIEMPRE forzamos Idle
                animator.Play("Idle");
                isWalking = false;
                isSliding = false;
            }
        }

    }
}
