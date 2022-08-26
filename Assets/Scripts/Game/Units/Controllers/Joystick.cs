using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEngine.Mathf;

// custom joystick class
public class Joystick : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool IsDragging { get; private set; }
    public Vector2 DragDirection { get; private set; }

    public UnityEvent OnClick;

    private RectTransform rect;
    [SerializeField]
    private RectTransform meter;
    [SerializeField]
    private RectTransform stick;

    private Vector2 defaultStickPosition;
    private Vector2 anchoredShiftVector;

    private float DragRange { get => meter.sizeDelta.x / 2f; }

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        defaultStickPosition = stick.anchoredPosition;

        anchoredShiftVector = new Vector2();
        anchoredShiftVector.x = Screen.width * rect.anchorMin.x;
        anchoredShiftVector.y = Screen.height * rect.anchorMin.y;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDragging = true;
        DragDirection = Vector3.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - eventData.pressPosition;
        direction.x /= DragRange;
        direction.y /= DragRange;
        
        direction.x = Abs(direction.x) > 1f ? Sign(direction.x) : direction.x;
        direction.y = Abs(direction.y) > 1f ? Sign(direction.y) : direction.y;

        DragDirection = (direction.magnitude > 1f) ? direction.normalized : direction;
        stick.anchoredPosition = DragDirection * DragRange;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDragging = false;
        DragDirection = Vector3.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        stick.anchoredPosition = defaultStickPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnClick.Invoke();
        stick.anchoredPosition = defaultStickPosition;
    }
}
