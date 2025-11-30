using UnityEngine;

public class RTSCameraMobile : MonoBehaviour
{
    [Header("Movement (Pan)")]
    public float panSpeed = 0.1f;       // velocidad de arrastre con dedo
    public float keyboardSpeed = 20f;   // para probar con WASD en PC
    public float minX = -50f;
    public float maxX = 50f;
    public float minZ = -50f;
    public float maxZ = 50f;

    [Header("Zoom")]
    public float zoomSpeed = 0.5f;      // sensibilidad del pinch / rueda
    public float minHeight = 10f;       // altura mínima de la cámara
    public float maxHeight = 60f;       // altura máxima de la cámara

    private Camera cam;
    private Vector2 lastPanPosition;
    private bool isPanning;

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        if (cam == null)
        {
            Debug.LogError("RTSCameraMobile: No child Camera found");
        }
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        HandleKeyboardMovement();
        HandleMouseZoom();
        HandleMousePan();
#endif

        HandleTouchInput();
    }

    // --------- PC / Editor (para probar cómodo) ---------

    private void HandleKeyboardMovement()
    {
        float h = Input.GetAxisRaw("Horizontal"); // A/D o flechas
        float v = Input.GetAxisRaw("Vertical");   // W/S o flechas

        Vector3 dir = new Vector3(h, 0f, v).normalized;
        Vector3 move = dir * keyboardSpeed * Time.deltaTime;

        MoveCamera(move);
    }

    private void HandleMouseZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            Zoom(scroll * 10f); // 10 para acercar la sensación al pinch
        }
    }

    private void HandleMousePan()
    {
        if (Input.GetMouseButtonDown(2)) // botón central (click rueda)
        {
            lastPanPosition = Input.mousePosition;
            isPanning = true;
        }
        else if (Input.GetMouseButton(2) && isPanning)
        {
            Vector2 pos = Input.mousePosition;
            Vector2 delta = pos - lastPanPosition;
            lastPanPosition = pos;

            Pan(delta);
        }
        else if (Input.GetMouseButtonUp(2))
        {
            isPanning = false;
        }
    }

    // --------- Input táctil (celular) ---------

    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                lastPanPosition = t.position;
                isPanning = true;
            }
            else if (t.phase == TouchPhase.Moved && isPanning)
            {
                Vector2 delta = t.position - lastPanPosition;
                lastPanPosition = t.position;

                Pan(delta);
            }
            else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                isPanning = false;
            }
        }
        else if (Input.touchCount == 2)
        {
            // Pinch zoom
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 t0PrevPos = t0.position - t0.deltaPosition;
            Vector2 t1PrevPos = t1.position - t1.deltaPosition;

            float prevDist = (t0PrevPos - t1PrevPos).magnitude;
            float currDist = (t0.position - t1.position).magnitude;

            float diff = currDist - prevDist; // >0 separan dedos, <0 juntan

            Zoom(diff * zoomSpeed * Time.deltaTime);
        }
    }

    // --------- Lógica común ---------

    private void Pan(Vector2 screenDelta)
    {
        if (cam == null) return;

        Vector3 right = cam.transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 forward = cam.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        // Invertimos para que arrastrar el mapa se sienta natural
        Vector3 move =
            (-screenDelta.x * right + -screenDelta.y * forward) * panSpeed * Time.deltaTime;

        MoveCamera(move);
    }

    private void Zoom(float zoomAmount)
    {
        Vector3 pos = transform.position;

        Vector3 forward = transform.forward;
        forward.y = Mathf.Abs(forward.y);

        pos += forward * zoomAmount;

        pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight);

        transform.position = pos;
    }

    private void MoveCamera(Vector3 move)
    {
        Vector3 newPos = transform.position + move;

        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);

        transform.position = newPos;
    }
}
