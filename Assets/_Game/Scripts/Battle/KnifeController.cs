using BlockoutRoguelike.Cards.Mobs;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KnifeController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Settings")]
    public float yOffset = 50f;
    public float xOffset = 0f;
    public float returnSpeed = 10f;
    
    [Header("References")]
    public Image knifeImage;
    public LayerMask interactableLayer;
    
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector2 originalAnchoredPosition;
    private bool isDragging = false;
    private Sacrifice highlightedObject;
    private MobUI highlightedMob;
    
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
            Vector2 mousePosition = eventData.position;
            rectTransform.position = new Vector2(mousePosition.x + xOffset, mousePosition.y + yOffset);
            
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

        if (highlightedMob != null)
        {
            highlightedMob.Blood();
        }
        RemoveHighlight();
        
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            var sacrifice = hit.transform.GetComponent<Sacrifice>();
            if (sacrifice!=null && !sacrifice.Equals(highlightedObject))
            {
                RemoveHighlight();
                
                highlightedObject = sacrifice;
                HighlightObject(highlightedObject);
            }

            var mob = hit.transform.GetComponent<MobUI>();
            if (mob!=null&& !mob.Equals(highlightedMob))
            {
                RemoveHighlight();
                highlightedMob = mob;
                HighlightObject(mob);
            }
        }
        else
        {
            RemoveHighlight();
        }
    }
    
    private void HighlightObject(Sacrifice sacrifice)
    {
        sacrifice.Highlight(true);
        Debug.Log("Object highlighted: " + sacrifice.name);
    }
    
    private void HighlightObject(MobUI mob)
    {
        mob.Highlight(true);
        Debug.Log("Mob highlighted: " + mob.name);
    }
    
    private void RemoveHighlight()
    {
        if (highlightedObject != null)
        {
            highlightedObject.Highlight(false);
            Debug.Log("Highlight removed from: " + highlightedObject.name);
            highlightedObject = null;
        }

        if (highlightedMob != null)
        {
            highlightedMob.Highlight(false);
            Debug.Log("Highlight removed from: " + highlightedMob.name);
            highlightedMob = null;
        }
    }
    
    void Update()
    {
        if (isDragging && Input.GetMouseButton(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            rectTransform.position = new Vector2(mousePosition.x + xOffset, mousePosition.y + yOffset);
            
            CheckFor3DObjects();
        }
        
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            StopDragging();
        }
    }
    
    public void SetOffsets(float x, float y)
    {
        xOffset = x;
        yOffset = y;
    }
}