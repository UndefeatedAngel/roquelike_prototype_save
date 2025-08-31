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

    public void Setup()
    {
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
        
        layoutGroup = transform.parent.GetComponent<HorizontalLayoutGroup>();
        
        // Добавляем CanvasGroup для управления прозрачностью при перетаскивании
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        // Получаем ссылку на Canvas
        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }
    }

    // Обработка наведения мыши
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging) return;
        
        isHovering = true;
        transform.DOScale(originalScale * hoverScale, hoverDuration)
            .SetEase(hoverEase);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging) return;
        
        isHovering = false;
        transform.DOScale(originalScale, hoverDuration)
            .SetEase(hoverEase);
    }

    // Начало перетаскивания - ИСПРАВЛЕННАЯ ЧАСТЬ
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        
        // Сохраняем оригинальные значения
        originalPosition = transform.localPosition;
        originalSiblingIndex = transform.GetSiblingIndex();
        
        // Рассчитываем offset между позицией мыши и центром объекта
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );
        
        offset = (Vector2)transform.localPosition - localPoint-new Vector2(0, 500);
        
        // Увеличиваем немного при перетаскивании
        transform.DOScale(originalScale * dragScale, 0.2f);
        
        // Делаем полупрозрачным
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;
        
        // Отключаем layout group чтобы не мешал
        if (layoutGroup != null)
            layoutGroup.enabled = false;
        
        // Перемещаем в корень канваса чтобы был поверх других элементов
        transform.SetParent(canvas.transform);
    }

    // Процесс перетаскивания - ИСПРАВЛЕННАЯ ЧАСТЬ
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        // Преобразуем позицию мыши в локальные координаты канваса
        if (canvas != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            // Устанавливаем позицию с учетом offset
            transform.localPosition = localPoint + offset;
        }
    }

    // Конец перетаскивания
    public void OnEndDrag(PointerEventData eventData)
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
            // Если не возвращаем, оставляем там где бросили
            transform.DOScale(originalScale, 0.3f);
        }
    }

    // Плавное возвращение на исходную позицию
    private void ReturnToOriginalPosition()
    {
        // Возвращаем в оригинального родителя
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalSiblingIndex);
        
        // Анимация возвращения
        Sequence returnSequence = DOTween.Sequence();
        
        returnSequence.Append(transform.DOLocalMove(originalPosition, returnDuration)
            .SetEase(returnEase));
        
        returnSequence.Join(transform.DOScale(originalScale, returnDuration * 0.8f)
            .SetEase(Ease.OutBack));
        
        returnSequence.OnComplete(() =>
        {
            // Включаем layout group обратно
            if (layoutGroup != null)
            {
                StartCoroutine(EnableLayoutGroup());
            }
            
            // Если курсор все еще над объектом, применяем hover эффект
            if (isHovering)
            {
                transform.DOScale(originalScale * hoverScale, hoverDuration)
                    .SetEase(hoverEase);
            }
        });
    }

    // Корутина для включения layout group с задержкой
    private IEnumerator EnableLayoutGroup()
    {
        yield return new WaitForEndOfFrame();
        if (layoutGroup != null)
            layoutGroup.enabled = true;
    }

    // Обновление оригинальной позиции при изменении layout
    public void UpdateOriginalPosition()
    {
        originalPosition = transform.localPosition;
    }

    // Для принудительного возврата если нужно
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