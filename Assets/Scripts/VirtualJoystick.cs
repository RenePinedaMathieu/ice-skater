using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform handle;  // the JoystickHandle image

    private RectTransform bgRect;
    private Vector2 inputDir = Vector2.zero;

    public Vector2 Direction => inputDir;

    private void Awake()
    {
        bgRect = GetComponent<RectTransform>();
        if (handle == null)
        {
            Debug.LogWarning("VirtualJoystick: handle no asignado");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (bgRect == null) return;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            bgRect, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            // normaliza a (-1,1) en X/Y
            Vector2 normalized = new Vector2(
                localPoint.x / (bgRect.sizeDelta.x * 0.5f),
                localPoint.y / (bgRect.sizeDelta.y * 0.5f)
            );

            normalized = Vector2.ClampMagnitude(normalized, 1f);
            inputDir = normalized;

            if (handle != null)
            {
                handle.anchoredPosition = new Vector2(
                    normalized.x * (bgRect.sizeDelta.x * 0.5f),
                    normalized.y * (bgRect.sizeDelta.y * 0.5f)
                );
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputDir = Vector2.zero;
        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }
    }
}
