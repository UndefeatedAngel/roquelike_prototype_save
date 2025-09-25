using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

public class DragAndHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Hover Settings")]
    public float hoverScale = 1.2f;
    public float hoverDuration = 0.3f;
    public Ease hoverEase = Ease.OutBack;

    [Header("Drag Settings")]
    public float dragScale = 1.1f;
    public bool returnToOriginalPosition = true;
    public float returnDuration = 0.5f;
    public Ease returnEase = Ease.OutBack;

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Transform originalParent;
    private int originalSiblingIndex;
    private HorizontalLayoutGroup layoutGroup;
    private CanvasGroup canvasGroup;
    private bool isDragging = false;
    private bool isHovering = false;
    private Canvas canvas;
    private RectTransform canvasRect;
    private Vector2 offset;

    [SerializeField]private bool IsInput = true;

    public void UpdateInput(bool state)
    {
        IsInput = state;
    }
    
    public void Setup()
    {
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
        
        layoutGroup = transform.parent.GetComponent<HorizontalLayoutGroup>();
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsInput) return;
        
        if (isDragging) return;
        
        isHovering = true;
        transform.DOScale(originalScale * hoverScale, hoverDuration)
            .SetEase(hoverEase);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsInput) return;
        
        if (isDragging) return;
        
        isHovering = false;
        transform.DOScale(originalScale, hoverDuration)
            .SetEase(hoverEase);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsInput) return;
        
        isDragging = true;
        
        originalPosition = transform.localPosition;
        originalSiblingIndex = transform.GetSiblingIndex();
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );
        
        offset = Vector2.zero;
        
        transform.DOScale(originalScale * dragScale, 0.2f);
        
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;
        
        if (layoutGroup != null)
            layoutGroup.enabled = false;
        
        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsInput) return;
        
        if (!isDragging) return;
        
        if (canvas != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            transform.localPosition = localPoint + offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsInput) return;
        
        isDragging = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        if (returnToOriginalPosition)
        {
            ReturnToOriginalPosition();
        }
        else
        {
            transform.DOScale(originalScale, 0.3f);
        }
    }

    public void EndDrag()
    {
        isDragging = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        if (returnToOriginalPosition)
        {
            ReturnToOriginalPosition();
        }
        else
        {
            transform.DOScale(originalScale, 0.3f);
        }
    }

    private void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalSiblingIndex);
        
        Sequence returnSequence = DOTween.Sequence();
        
        returnSequence.Append(transform.DOLocalMove(originalPosition, returnDuration)
            .SetEase(returnEase));
        
        returnSequence.Join(transform.DOScale(originalScale, returnDuration * 0.8f)
            .SetEase(Ease.OutBack));
        
        returnSequence.OnComplete(() =>
        {
            if (layoutGroup != null)
            {
                StartCoroutine(EnableLayoutGroup());
            }
            
            if (isHovering)
            {
                transform.DOScale(originalScale * hoverScale, hoverDuration)
                    .SetEase(hoverEase);
            }
        });
    }

    private IEnumerator EnableLayoutGroup()
    {
        yield return new WaitForEndOfFrame();
        if (layoutGroup != null)
            layoutGroup.enabled = true;
    }

    public void UpdateOriginalPosition()
    {
        originalPosition = transform.localPosition;
    }

    public void ForceReturnToPosition()
    {
        if (isDragging) return;
        
        StopAllCoroutines();
        DOTween.Kill(transform);
        
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalSiblingIndex);
        transform.localPosition = originalPosition;
        transform.localScale = originalScale;
        
        if (layoutGroup != null)
            layoutGroup.enabled = true;
    }

    void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}