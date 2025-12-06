using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;   // 👈 IMPORTANTE para usar Image

public class HoldPowerButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Charge Settings")]
    public float holdTime = 2f;
    public CameraModeController controller;

    [Header("UI Feedback")]
    public Image fillImage;          // 👈 ChargeFill
    public Transform buttonVisual;   // opcional: para escalar el botón
    public Vector3 pressedScale = new Vector3(1.1f, 1.1f, 1f);

    private bool isHolding = false;
    private float timer = 0f;
    private Vector3 originalScale;

    private void Start()
    {
        if (buttonVisual == null)
        {
            // por defecto, usamos el propio transform del botón
            buttonVisual = transform;
        }

        originalScale = buttonVisual.localScale;

        if (fillImage != null)
        {
            fillImage.fillAmount = 0f;
        }
    }

    private void Update()
    {
        if (!isHolding) return;

        timer += Time.deltaTime;

        // Actualizar fill visual
        if (fillImage != null)
        {
            float t = Mathf.Clamp01(timer / holdTime);
            fillImage.fillAmount = t;
        }

        // Cuando se completa la carga
        if (timer >= holdTime)
        {
            isHolding = false;

            // Reset visual
            if (fillImage != null)
                fillImage.fillAmount = 0f;

            if (buttonVisual != null)
                buttonVisual.localScale = originalScale;

            // Lanzar el poder
            if (controller != null)
            {
                controller.FirePowerFromChargeButton();
            }
            else
            {
                Debug.LogWarning("HoldPowerButton: controller no asignado.");
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        timer = 0f;

        if (buttonVisual != null)
            buttonVisual.localScale = pressedScale;

        if (fillImage != null)
            fillImage.fillAmount = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        timer = 0f;

        // Reset visual si suelta antes de tiempo
        if (fillImage != null)
            fillImage.fillAmount = 0f;

        if (buttonVisual != null)
            buttonVisual.localScale = originalScale;
    }
}
