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

    private float verticalVelocity = 0f;
    private bool onIce = false;
    private Vector3 slideDir = Vector3.zero;

    private bool jumpRequested = false;

    private bool isWalking = false;
    private bool isSliding = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        if (controller == null)
        {
            Debug.LogError("SimplePlayerMove: falta CharacterController en el Player");
        }
    }

    // Llamado por el botón Jump (UI)
    public void OnJumpButton()
    {
        // Solo marcamos la intención de saltar, se aplica en Update cuando esté en el suelo
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

        // === 1) Ground general ===
        bool grounded = CheckGrounded();

        // === 2) Detectar hielo solo cuando estamos en el suelo ===
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

        // === 3) INPUT: joystick primero, teclado de backup ===
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
            // en PC / editor
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
        }

        Vector3 inputDir = new Vector3(h, 0f, v);
        if (inputDir.sqrMagnitude > 1f)
            inputDir.Normalize();

        bool hasInput = inputDir.sqrMagnitude > 0.001f;

        // === 4) Rotación (solo si hay input horizontal) ===
        if (hasInput)
        {
            Quaternion targetRot = Quaternion.LookRotation(inputDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                10f * Time.deltaTime
            );
        }

        // === 5) Salto + gravedad (física, sin anim especial) ===
        if (grounded)
        {
            // manténlo pegado al suelo
            if (verticalVelocity < 0f)
                verticalVelocity = -1f;

            // aplica salto aunque esté idle
            if (jumpRequested)
            {
                verticalVelocity = jumpForce;
                jumpRequested = false;
            }
        }
        else
        {
            // en el aire, no acumules más saltos
            jumpRequested = false;
        }

        // gravedad siempre
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 move;

        // === 6) Movimiento: hielo vs normal ===
        if (onIce)
        {
            // modo hielo: inercia solo cuando está en el suelo
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
            // modo normal
            Vector3 horizontal = inputDir * moveSpeed;
            move = new Vector3(horizontal.x, verticalVelocity, horizontal.z);
            slideDir = Vector3.zero;
        }

        // === 7) Mover al personaje ===
        controller.Move(move * Time.deltaTime);

        // === 8) Animaciones: Idle / Walk / Slide (sin Jump) ===
        if (animator != null)
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
                // Quieto => Idle siempre
                animator.Play("Idle");
                isWalking = false;
                isSliding = false;
            }
        }
    }
}
