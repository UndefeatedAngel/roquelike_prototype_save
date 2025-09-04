using BlockoutRoguelike.Cards.Mobs;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KnifeController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Settings")]
    public float yOffset = 50f; // Смещение над курсором по Y
    public float xOffset = 0f; // Смещение по X
    public float returnSpeed = 10f; // Скорость возвращения на место
    
    [Header("References")]
    public Image knifeImage;
    public LayerMask interactableLayer; // Слой для 3D объектов
    
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector2 originalAnchoredPosition;
    private bool isDragging = false;
    private Sacrifice highlightedObject;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.position;
        originalAnchoredPosition = rectTransform.anchoredPosition;
        
        if (knifeImage == null)
            knifeImage = GetComponent<Image>();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        StartDragging(eventData);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            // Двигаем нож за мышью с учетом смещений
            Vector2 mousePosition = eventData.position;
            rectTransform.position = new Vector2(mousePosition.x + xOffset, mousePosition.y + yOffset);
            
            // Проверяем попадание на 3D объекты
            CheckFor3DObjects();
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        StopDragging();
    }
    
    private void StartDragging(PointerEventData eventData)
    {
        isDragging = true;
        
        // Поднимаем нож над мышью с учетом смещений
        Vector2 mousePosition = eventData.position;
        rectTransform.position = new Vector2(mousePosition.x + xOffset, mousePosition.y + yOffset);
    }
    
    private void StopDragging()
    {
        isDragging = false;

        if (highlightedObject != null)
        {
            highlightedObject.Attack();
        }
        // Убираем подсветку
        RemoveHighlight();
        
        // Возвращаем нож на место
        StartCoroutine(ReturnToPosition());
    }
    
    private System.Collections.IEnumerator ReturnToPosition()
    {
        float elapsedTime = 0f;
        Vector2 startPosition = rectTransform.anchoredPosition;
        
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * returnSpeed;
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, originalAnchoredPosition, elapsedTime);
            yield return null;
        }
        
        rectTransform.anchoredPosition = originalAnchoredPosition;
    }
    
    private void CheckFor3DObjects()
    {
        // Создаем луч из камеры в точку курсора
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            var sacrifice = hit.transform.GetComponent<Sacrifice>();
            if (!sacrifice.Equals(highlightedObject))
            {
                // Убираем старую подсветку
                RemoveHighlight();
                
                // Подсвечиваем новый объект
                highlightedObject = sacrifice;
                HighlightObject(highlightedObject);
            }
        }
        else
        {
            // Если не попали ни на что - убираем подсветку
            RemoveHighlight();
        }
    }
    
    private void HighlightObject(Sacrifice sacrifice)
    {
        sacrifice.Highlight(true);
        Debug.Log("Object highlighted: " + sacrifice.name);
    }
    
    private void RemoveHighlight()
    {
        if (highlightedObject != null)
        {
            highlightedObject.Highlight(false);
            Debug.Log("Highlight removed from: " + highlightedObject.name);
            highlightedObject = null;
        }
    }
    
    void Update()
    {
        // Дополнительное отслеживание мыши, если нужно
        if (isDragging && Input.GetMouseButton(0))
        {
            // Обновляем позицию с учетом смещений (на случай если Drag события не работают)
            Vector2 mousePosition = Input.mousePosition;
            rectTransform.position = new Vector2(mousePosition.x + xOffset, mousePosition.y + yOffset);
            
            CheckFor3DObjects();
        }
        
        // Если отпустили кнопку мыши во время перетаскивания
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            StopDragging();
        }
    }
    
    // Метод для быстрой настройки смещений через код
    public void SetOffsets(float x, float y)
    {
        xOffset = x;
        yOffset = y;
    }
}