using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using BlockoutRoguelike.Cards;

public class DraggableCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private int _numberCard;
    public int NumberCard => _numberCard;
    
    [Header("Hover Settings")]
    public float hoverScale = 1.2f;
    public float hoverDuration = 0.2f;
    
    [Header("Drag Settings")]
    public float dragScale = 1.1f;
    public float dragDuration = 0.15f;
    public bool returnToPosition = true;
    public bool destroyOnRelease = false;
    
    [Header("References")]
    public Canvas canvas;
    public GraphicRaycaster raycaster;

    private Vector3 originalScale;
    private Vector3 originalLocalPosition;
    private Vector3 originalWorldPosition;
    private Transform originalParent;
    private int originalSiblingIndex;
    private Vector3 dragOffset;
    
    private bool isHovered = false;
    private bool isDragging = false;
    private Coroutine scaleCoroutine;
    private CanvasGroup canvasGroup;

    void Start()
    {
        // Сохраняем оригинальные значения
        originalScale = transform.localScale;
        originalLocalPosition = transform.localPosition;
        originalWorldPosition = transform.position;
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
        
        // Получаем или создаем CanvasGroup
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            
        // Находим canvas если не установлен
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
            
        if (raycaster == null && canvas != null)
            raycaster = canvas.GetComponent<GraphicRaycaster>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging) return;
        
        isHovered = true;
        ScaleTo(originalScale * hoverScale, hoverDuration);
        transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging) return;
        
        isHovered = false;
        ScaleTo(originalScale, hoverDuration);
        transform.SetSiblingIndex(originalSiblingIndex);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        StartDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || !isDragging) return;
        
        EndDrag();
    }

    private void StartDrag(PointerEventData eventData)
    {
        isDragging = true;
        InputController.ActiveCard = this; 
        
        // Сохраняем смещение между позицией карты и курсором
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            dragOffset = transform.position - Input.mousePosition;
        }
        else
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out localPoint
            );
            dragOffset = transform.localPosition - (Vector3)localPoint;
        }
        
        // Увеличиваем немного при захвате
        ScaleTo(originalScale * dragScale, dragDuration);
        
        // Делаем карту полупрозрачной
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;
        
        // Перемещаем на верхний уровень canvas
        if (canvas != null)
            transform.SetParent(canvas.transform, true);
            
        transform.SetAsLastSibling();
    }

    private void EndDrag()
    {
        isDragging = false;

        StartCoroutine(WaitEndDrag());
        // Возвращаем прозрачность
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        if (destroyOnRelease)
        {
            // Анимация перед уничтожением
            ScaleTo(Vector3.zero, 0.2f);
            StartCoroutine(DestroyAfterDelay(0.2f));
        }
        else if (returnToPosition)
        {
            // Возвращаем на место
            ReturnToOriginalPosition();
        }
    }

    private IEnumerator WaitEndDrag()
    {
        yield return new WaitForSeconds(0.5f);
        
        InputController.ActiveCard = null;
    }

    private void ReturnToOriginalPosition()
    {
        // Возвращаем к оригинальному scale
        ScaleTo(originalScale, dragDuration);
        
        // Возвращаем в оригинальный parent
        transform.SetParent(originalParent, false);
        
        // Анимация возврата позиции
        StartCoroutine(AnimateReturnToPosition());
    }

    private IEnumerator AnimateReturnToPosition()
    {
        Vector3 startPosition = transform.localPosition;
        float elapsed = 0f;
        
        while (elapsed < dragDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dragDuration;
            
            // Плавно возвращаем к оригинальной локальной позиции
            transform.localPosition = Vector3.Lerp(startPosition, originalLocalPosition, t);
            yield return null;
        }
        
        // Точное позиционирование
        transform.localPosition = originalLocalPosition;
        transform.SetSiblingIndex(originalSiblingIndex);
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void ScaleTo(Vector3 targetScale, float duration)
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);
            
        scaleCoroutine = StartCoroutine(AnimateScale(targetScale, duration));
    }

    private IEnumerator AnimateScale(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        
        transform.localScale = targetScale;
    }
    
    void Update()
    {
        if (isDragging)
        {
            FollowMouse();
        }
    }

    private void FollowMouse()
    {
        if (canvas == null) return;
        
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Для ScreenSpaceOverlay просто используем позицию мыши
            transform.position = (Vector3)Input.mousePosition + dragOffset;
        }
        else
        {
            // Для других режимов используем правильное преобразование координат
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out localPoint
            );
            
            transform.localPosition = (Vector3)localPoint + dragOffset;
        }
    }

    // Принудительный сброс
    public void ResetCard()
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);
            
        isHovered = false;
        isDragging = false;
        
        transform.localScale = originalScale;
        transform.SetParent(originalParent, false);
        transform.localPosition = originalLocalPosition;
        transform.SetSiblingIndex(originalSiblingIndex);
        
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    // Обновить оригинальные значения (вызвать после изменения позиции)
    public void UpdateOriginalValues()
    {
        originalScale = transform.localScale;
        originalLocalPosition = transform.localPosition;
        originalWorldPosition = transform.position;
        originalSiblingIndex = transform.GetSiblingIndex();
    }

    void OnDisable()
    {
        ResetCard();
    }

    void OnTransformParentChanged()
    {
        // Обновляем оригинальные значения при изменении родителя
        UpdateOriginalValues();
    }
}