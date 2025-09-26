using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class DragPiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
   [Header("Referencia a la pieza fija (posición correcta)")]
    [SerializeField] RectTransform referencePiece;

    [Header("Margen de error en píxeles")]
    [SerializeField] float snapDistance = 35f;

    RectTransform rect;        // RectTransform de ESTA pieza interactiva
    RectTransform parentRect;  // Padre común
    CanvasGroup cg;
    bool isLocked;
    Vector2 startAnchored;

    void Awake()
    {
        rect       = GetComponent<RectTransform>();
        parentRect = rect.parent as RectTransform;
        cg         = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (isLocked) return;
        startAnchored = rect.anchoredPosition;
        rect.SetAsLastSibling(); // que quede encima al arrastrar
    }

    public void OnDrag(PointerEventData e)
    {
        if (isLocked) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect, e.position, e.pressEventCamera, out var localPoint))
        {
            rect.anchoredPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (isLocked) return;

        // Distancia entre esta pieza y su referencia
        float dist = DistanceToReference();

        if (dist <= snapDistance)
        {
            SnapAndLock();
        }
        else
        {
            rect.anchoredPosition = startAnchored;
        }
    }

    float DistanceToReference()
    {
        // Si comparten el mismo padre, compara directamente
        if (referencePiece.parent == parentRect)
            return Vector2.Distance(rect.anchoredPosition,
                                    referencePiece.anchoredPosition);

        // Si no, convierte la posición de la referencia al espacio local del padre
        Vector2 refInParent = (Vector2)parentRect.InverseTransformPoint(referencePiece.position);
        return Vector2.Distance(rect.anchoredPosition, refInParent);
    }

    void SnapAndLock()
    {
        // Ajusta la posición exacta de la pieza
        if (referencePiece.parent == parentRect)
            rect.anchoredPosition = referencePiece.anchoredPosition;
        else
            rect.anchoredPosition = (Vector2)parentRect.InverseTransformPoint(referencePiece.position);

        isLocked = true;

        // Bloquea la interacción
        cg.interactable   = false;
        cg.blocksRaycasts = false;

        // Si quieres, desactiva el script
        // enabled = false;
    }
}
