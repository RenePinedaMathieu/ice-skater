using UnityEngine;

public enum ControlMode
{
    PlayerThirdPerson,
    PlayerFirstPerson,
    BallControl
}

public class CameraModeController : MonoBehaviour
{
    [Header("Cameras")]
    public Camera thirdPersonCamera;   // cámara normal
    public Camera firstPersonCamera;   // cámara en la cabeza

    [Header("Player")]
    public SimplePlayerMove playerMovement; // script de movimiento
    public Renderer[] renderersToHide;      // se llenará solo en Start

    [Header("UI")]
    public GameObject crosshair;        // mira
    public GameObject jumpButton;       // botón de salto normal
    public GameObject powerButton;      // botón Power normal
    public GameObject chargeButton;     // botón de mantener 2s (solo 1ª persona)
    public GameObject ballJoystickRoot; // joystick para la bola

    [Header("Power Ball")]
    public GameObject powerBallPrefab;  // prefab de la bola azul

    private ControlMode mode = ControlMode.PlayerThirdPerson;
    private PowerBall activeBall;

    void Start()
    {
        // auto-detectar renderers si no los llenaste a mano
        if ((renderersToHide == null || renderersToHide.Length == 0) && playerMovement != null)
        {
            renderersToHide = playerMovement.GetComponentsInChildren<Renderer>();
        }

        ApplyMode();
    }

    // ====== Cambiar de modo con el botón Power ======

    public void TogglePowerMode()
    {
        if (mode == ControlMode.PlayerThirdPerson)
        {
            EnterFirstPerson();
        }
        else if (mode == ControlMode.PlayerFirstPerson)
        {
            EnterThirdPerson();
        }
        // En BallControl no hacemos nada con Power por ahora
    }

    private void EnterThirdPerson()
    {
        mode = ControlMode.PlayerThirdPerson;
        ApplyMode();
    }

    private void EnterFirstPerson()
    {
        mode = ControlMode.PlayerFirstPerson;
        ApplyMode();
    }

    private void EnterBallControl()
    {
        mode = ControlMode.BallControl;
        ApplyMode();
    }

private void ApplyMode()
{
    bool tp   = mode == ControlMode.PlayerThirdPerson;
    bool fp   = mode == ControlMode.PlayerFirstPerson;
    bool ball = mode == ControlMode.BallControl;

    // ===== Cámaras =====
    if (thirdPersonCamera != null)
        thirdPersonCamera.gameObject.SetActive(tp || ball);

    if (firstPersonCamera != null)
        firstPersonCamera.gameObject.SetActive(fp);

    // ===== Movimiento del player =====
    // -> Se mueve en modo normal y cuando controlas la bola
    if (playerMovement != null)
        playerMovement.allowMovement = tp || ball;

    // ===== Modelo del player (renderers) =====
    // -> Solo se oculta en 1ª persona
    if (renderersToHide != null)
    {
        foreach (var r in renderersToHide)
        {
            if (r != null)
                r.enabled = !fp;
        }
    }

    // ===== UI =====

    // Crosshair: solo en 1ª persona
    if (crosshair != null)
        crosshair.SetActive(fp);

    // Jump: se ve en modo normal y en BallControl
    if (jumpButton != null)
        jumpButton.SetActive(tp || ball);

    // Power:
    // - visible en modo normal y en 1ª persona
    // - oculto cuando controlas la bola (porque ahí va el joystick de la bola)
    if (powerButton != null)
        powerButton.SetActive(tp || fp);

    // Charge: solo en 1ª persona (para cargar el poder)
    if (chargeButton != null)
        chargeButton.SetActive(fp);

    // Joystick de la bola: solo en BallControl
    if (ballJoystickRoot != null)
        ballJoystickRoot.SetActive(ball);
}


    // ====== Llamado por el botón de carga cuando se cumple el tiempo ======

    public void FirePowerFromChargeButton()
    {
        if (mode != ControlMode.PlayerFirstPerson) return;
        if (powerBallPrefab == null || firstPersonCamera == null) return;

        // spawn un poco delante de la cámara
        Vector3 spawnPos = firstPersonCamera.transform.position +
                           firstPersonCamera.transform.forward * 1.5f;
        Quaternion spawnRot = Quaternion.LookRotation(firstPersonCamera.transform.forward);

        GameObject go = Instantiate(powerBallPrefab, spawnPos, spawnRot);
        activeBall = go.GetComponent<PowerBall>();
        if (activeBall != null)
        {
            activeBall.Init(this);
        }

        // Pasar a controlar la bola (cámara vuelve a 3ª persona)
        EnterBallControl();
    }

    // ====== Input del joystick de la bola ======

    public Vector2 GetBallInput()
    {
        if (mode != ControlMode.BallControl || ballJoystickRoot == null)
            return Vector2.zero;

        // Usamos el mismo tipo de joystick que el player
        var joy = ballJoystickRoot.GetComponentInChildren<VirtualJoystick>();
        if (joy == null) return Vector2.zero;

        return joy.Direction; // Vector2 (x,y)
    }

    // ====== Llamado por PowerBall cuando se destruye ======

    public void OnBallDestroyed()
    {
        activeBall = null;
        EnterThirdPerson();   // volver al control normal del player
    }
}
