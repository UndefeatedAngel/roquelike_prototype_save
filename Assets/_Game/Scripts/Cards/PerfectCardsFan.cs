using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class CardData
{
    public GameObject cardPrefab;
    public int weight = 1;
    public string cardName;
}

public class PerfectCardsFan : MonoBehaviour
{
    [Header("Center Point Settings")]
    public Transform centerPoint; // Точка в центре снизу
    public float centerOffsetY = 50f; // Смещение от центра вверх

    [Header("Fan Settings")]
    public float ellipseWidth = 600f; // Ширина эллипса
    public float ellipseHeight = 200f; // Высота эллипса
    public float maxRotation = 25f; // Максимальный угол вращения
    public float minCardDistance = 80f; // Минимальное расстояние между картами
    public float maxCardDistance = 150f; // Максимальное расстояние при малом количестве

    [Header("Animation Settings")]
    public float animationDuration = 0.7f;
    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float arcHeight = 120f; // Высота дуги анимации

    [Header("Card Database")]
    public CardData[] cardDatabase; // Массив карт с весами
    public int maxCards = 10; // Максимальное количество карт в веере

    private List<RectTransform> activeCards = new List<RectTransform>();
    private List<CardData> availableCards = new List<CardData>();

    void Start()
    {
        // Инициализируем доступные карты
        availableCards = new List<CardData>(cardDatabase);
        
        // Стартовая анимация
        StartCoroutine(InitialAnimation());
    }

    IEnumerator InitialAnimation()
    {
        yield return new WaitForSeconds(1f);
        
        // Добавляем первые 3 карты
        for (int i = 0; i < 3; i++)
        {
            AddRandomCard();
            yield return new WaitForSeconds(0.4f);
        }
    }

    public void AddRandomCard()
    {
        if (activeCards.Count >= maxCards || availableCards.Count == 0) return;

        // Выбираем случайную карту с учетом весов
        CardData selectedCard = GetRandomWeightedCard();
        if (selectedCard == null) return;

        // Создаем карту
        GameObject newCard = Instantiate(selectedCard.cardPrefab, transform);
        RectTransform cardRT = newCard.GetComponent<RectTransform>();
        
        // Настраиваем трансформ
        cardRT.anchorMin = new Vector2(0.5f, 0f);
        cardRT.anchorMax = new Vector2(0.5f, 0f);
        cardRT.pivot = new Vector2(0.5f, 0f);
        
        // Начальная позиция (слева за экраном)
        Vector2 startPos = GetStartPosition();
        cardRT.anchoredPosition = startPos;
        cardRT.localRotation = Quaternion.Euler(0, 0, 15f);
        cardRT.localScale = Vector3.zero;

        activeCards.Add(cardRT);
        
        // Запускаем анимацию
        StartCoroutine(AnimateCardToPosition(cardRT, activeCards.Count - 1));
    }

    private CardData GetRandomWeightedCard()
    {
        if (availableCards.Count == 0) return null;

        // Сумма всех весов
        int totalWeight = availableCards.Sum(card => card.weight);
        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var card in availableCards)
        {
            currentWeight += card.weight;
            if (randomValue < currentWeight)
            {
                return card;
            }
        }

        return availableCards[0];
    }

    private Vector2 GetStartPosition()
    {
        // Стартовая позиция слева за экраном
        float screenWidth = Screen.width;
        return new Vector2(-screenWidth / 2 - 200f, -100f);
    }

    private void CalculateFanPositions(out List<Vector2> positions, out List<float> rotations)
    {
        positions = new List<Vector2>();
        rotations = new List<float>();

        int cardCount = activeCards.Count;
        if (cardCount == 0) return;

        // Динамическое расстояние между картами
        float totalWidth = Mathf.Min(ellipseWidth, maxCardDistance * cardCount);
        float spacing = totalWidth / Mathf.Max(1, cardCount - 1);
        spacing = Mathf.Clamp(spacing, minCardDistance, maxCardDistance);

        float startX = -totalWidth / 2f;

        for (int i = 0; i < cardCount; i++)
        {
            // Позиция по X
            float x = startX + (spacing * i);
            
            // Позиция по Y (эллипс)
            float normalizedX = (x + totalWidth / 2f) / totalWidth; // 0 to 1
            float angle = normalizedX * Mathf.PI; // 0 to π
            float y = Mathf.Sin(angle) * ellipseHeight;
            
            // Вращение (больше на краях, меньше в центре)
            float rotation = Mathf.Lerp(-maxRotation, maxRotation, normalizedX);
            
            // Смещение относительно центральной точки
            Vector2 centerPos = centerPoint != null ? 
                (Vector2)centerPoint.position : 
                new Vector2(Screen.width / 2f, centerOffsetY);
            
            Vector2 screenPos = new Vector2(centerPos.x + x, centerPos.y + y);
            Vector2 localPos = transform.InverseTransformPoint(screenPos);

            positions.Add(localPos);
            rotations.Add(rotation);
        }
    }

    private IEnumerator AnimateCardToPosition(RectTransform card, int index)
    {
        CalculateFanPositions(out List<Vector2> targetPositions, out List<float> targetRotations);

        if (index >= targetPositions.Count) yield break;

        float elapsed = 0f;
        Vector2 startPos = card.anchoredPosition;
        Quaternion startRot = card.localRotation;
        Vector3 startScale = card.localScale;

        Vector2 targetPos = targetPositions[index];
        Quaternion targetRot = Quaternion.Euler(0, 0, targetRotations[index]);

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = movementCurve.Evaluate(elapsed / animationDuration);

            // Основное движение
            Vector2 currentPos = Vector2.Lerp(startPos, targetPos, t);
            
            // Дугообразная траектория
            float arc = Mathf.Sin(t * Mathf.PI) * arcHeight;
            currentPos.y += arc;

            card.anchoredPosition = currentPos;
            card.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            card.localScale = Vector3.Lerp(startScale, Vector3.one, t);

            yield return null;
        }

        // Точная финальная позиция
        card.anchoredPosition = targetPos;
        card.localRotation = targetRot;
        card.localScale = Vector3.one;

        // Обновляем все карты для точного позиционирования
        UpdateAllCards();
    }

    private void UpdateAllCards()
    {
        CalculateFanPositions(out List<Vector2> positions, out List<float> rotations);

        for (int i = 0; i < activeCards.Count; i++)
        {
            if (activeCards[i] != null)
            {
                activeCards[i].anchoredPosition = positions[i];
                activeCards[i].localRotation = Quaternion.Euler(0, 0, rotations[i]);
            }
        }
    }

    public void RemoveCard(int index)
    {
        if (index < 0 || index >= activeCards.Count) return;

        Destroy(activeCards[index].gameObject);
        activeCards.RemoveAt(index);
        
        // Плавное обновление позиций
        StartCoroutine(UpdateCardsAnimation());
    }

    private IEnumerator UpdateCardsAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateAllCards();
    }

    public void RemoveLastCard()
    {
        if (activeCards.Count > 0)
            RemoveCard(activeCards.Count - 1);
    }

    public void ClearAllCards()
    {
        foreach (var card in activeCards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }
        activeCards.Clear();
    }

    // Для визуализации в редакторе
    void OnDrawGizmosSelected()
    {
        if (centerPoint == null) return;

        // Рисуем эллипс
        Gizmos.color = Color.green;
        int segments = 20;
        Vector3 prevPoint = centerPoint.position + new Vector3(-ellipseWidth / 2f, Mathf.Sin(0) * ellipseHeight);

        for (int i = 1; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angle = t * Mathf.PI;
            float x = Mathf.Lerp(-ellipseWidth / 2f, ellipseWidth / 2f, t);
            float y = Mathf.Sin(angle) * ellipseHeight;
            
            Vector3 point = centerPoint.position + new Vector3(x, y);
            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }
    }

    [ContextMenu("Add Random Card")]
    public void TestAddRandomCard() => AddRandomCard();

    [ContextMenu("Remove Last Card")]
    public void TestRemoveLastCard() => RemoveLastCard();

    [ContextMenu("Clear All Cards")]
    public void TestClearAllCards() => ClearAllCards();
}