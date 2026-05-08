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
        if (!GameController.isPaused && eventData.button == PointerEventData.InputButton.Left)
            rectTransform.anchoredPosition += eventData.delta / GameController.instance.GetCanvas().scaleFactor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!GameController.isPaused)
            transform.SetAsLastSibling();
    }
}
