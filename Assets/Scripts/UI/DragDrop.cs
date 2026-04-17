using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!GameController.isPaused)
            rectTransform.anchoredPosition += eventData.delta / 1.5f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!GameController.isPaused)
            transform.SetAsLastSibling();
    }
}
