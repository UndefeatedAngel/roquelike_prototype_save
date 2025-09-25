using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldDistanceMarker : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public LayerMask obstructionMask = 1;

    [Header("UI References")]
    public RectTransform directionArrow; // Основная стрелка направления
    public RectTransform distanceMarker; // Маркер расстояния (в мире)
    public TMP_Text distanceText; // Текст расстояния
    public Image arrowGlow;
    public CanvasGroup indicatorGroup;

    [Header("Marker Settings")]
    public float markerOffset = 2f; // Отступ маркера от объекта
    public float viewAngleThreshold = 45f; // Угол обзора для показа маркера
    public float maxViewDistance = 100f; // Максимальная дистанция видимости

    [Header("Visual Settings")]
    public Color normalColor = new Color(0.2f, 0.8f, 1f, 1f);
    public Color obstructedColor = new Color(1f, 0.3f, 0.2f, 1f);
    public float pulseSpeed = 2f;

    private Camera playerCamera;
    private float currentDistance;
    private bool isMarkerVisible = false;
    private bool isObstructed = false;

    void Start()
    {
        playerCamera = Camera.main;
        InitializeUI();
        HideMarker();
    }

    void Update()
    {
        if (target == null)
        {
            HideMarker();
            return;
        }

        UpdateDistance();
        CheckObstruction();
        UpdateWorldMarkerPosition();
        UpdateIndicatorPosition();
        CheckMarkerVisibility();
        UpdateVisualEffects();
    }

    private void InitializeUI()
    {
        if (indicatorGroup != null)
        {
            //indicatorGroup.alpha = 0f;
        }

        if (arrowGlow != null)
        {
            arrowGlow.color = normalColor;
        }
    }

    private void UpdateDistance()
    {
        currentDistance = Vector3.Distance(transform.position, target.position);
        
        if (distanceText != null)
        {
            distanceText.text = $"{currentDistance:F0}m";
        }
    }

    private void CheckObstruction()
    {
        if (target == null) return;

        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        
        RaycastHit hit;
        isObstructed = Physics.Raycast(transform.position, directionToTarget, out hit, distanceToTarget, obstructionMask);
    }

    private void UpdateWorldMarkerPosition()
    {
        if (distanceMarker == null || target == null) return;

        // Позиционируем маркер в мировом пространстве над целью
        Vector3 worldPosition = target.position + Vector3.up * markerOffset;
        
        // Конвертируем мировые координаты в экранные
        Vector3 screenPoint = playerCamera.WorldToScreenPoint(worldPosition);
        
        // Устанавливаем позицию маркера
        distanceMarker.position = screenPoint;
    }

    private void UpdateIndicatorPosition()
    {
        if (directionArrow == null || playerCamera == null) return;

        Vector3 screenPoint = playerCamera.WorldToScreenPoint(target.position);
        bool isOnScreen = screenPoint.x > 0 && screenPoint.x < Screen.width && 
                         screenPoint.y > 0 && screenPoint.y < Screen.height && 
                         screenPoint.z > 0;

        if (isOnScreen)
        {
            directionArrow.position = screenPoint;
            directionArrow.rotation = Quaternion.identity;
        }
        else
        {
            PositionArrowAtScreenEdge(screenPoint);
        }
    }

    private void PositionArrowAtScreenEdge(Vector3 screenPoint)
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Vector3 direction = (screenPoint - screenCenter).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        directionArrow.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        float borderOffset = 80f;
        Vector3 edgePosition = new Vector3(
            Mathf.Clamp(screenPoint.x, borderOffset, Screen.width - borderOffset),
            Mathf.Clamp(screenPoint.y, borderOffset, Screen.height - borderOffset),
            0f
        );

        directionArrow.position = edgePosition;
    }

    private void CheckMarkerVisibility()
    {
        if (target == null || playerCamera == null)
        {
            HideMarker();
            return;
        }

        // Проверяем дистанцию
        if (currentDistance > maxViewDistance)
        {
            HideMarker();
            return;
        }

        // Проверяем угол между взглядом камеры и направлением к цели
        Vector3 directionToTarget = (target.position - playerCamera.transform.position).normalized;
        Vector3 cameraForward = playerCamera.transform.forward;
        
        float angle = Vector3.Angle(cameraForward, directionToTarget);
        bool isInViewCone = angle <= viewAngleThreshold;

        // Проверяем видимость (нет ли препятствий)
        RaycastHit hit;
        bool isVisible = !Physics.Raycast(playerCamera.transform.position, directionToTarget, out hit, currentDistance, obstructionMask);

        if (isInViewCone && isVisible && !isObstructed)
        {
            ShowMarker();
        }
        else
        {
            HideMarker();
        }
    }

    private void ShowMarker()
    {
        if (!isMarkerVisible && distanceMarker != null)
        {
            isMarkerVisible = true;
            distanceMarker.gameObject.SetActive(true);
            
            if (indicatorGroup != null)
            {
                indicatorGroup.alpha = 1f;
            }
        }
    }

    private void HideMarker()
    {
        if (isMarkerVisible && distanceMarker != null)
        {
            isMarkerVisible = false;
            distanceMarker.gameObject.SetActive(false);
            
            if (indicatorGroup != null)
            {
                //indicatorGroup.alpha = 0f;
            }
        }
    }

    private void UpdateVisualEffects()
    {
        // Пульсация свечения
        if (arrowGlow != null && isMarkerVisible)
        {
            float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            arrowGlow.color = new Color(arrowGlow.color.r, arrowGlow.color.g, arrowGlow.color.b, 0.3f + pulse * 0.4f);
        }

        // Изменение цвета при препятствии
        Color targetColor = isObstructed ? obstructedColor : normalColor;
        
        if (directionArrow != null)
        {
            Image arrowImage = directionArrow.GetComponent<Image>();
            if (arrowImage != null)
            {
                arrowImage.color = Color.Lerp(arrowImage.color, targetColor, Time.deltaTime * 5f);
            }
        }

        if (distanceMarker != null && isMarkerVisible)
        {
            Image markerImage = distanceMarker.GetComponent<Image>();
            if (markerImage != null)
            {
                markerImage.color = targetColor;
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        ShowMarker();
    }

    public void ClearTarget()
    {
        target = null;
        HideMarker();
    }

    // Для отладки в редакторе
    void OnDrawGizmos()
    {
        if (target != null && playerCamera != null)
        {
            // Рисуем луч проверки видимости
            Gizmos.color = isMarkerVisible ? Color.green : Color.red;
            Gizmos.DrawLine(playerCamera.transform.position, target.position);

            // Рисуем конус обзора
            Gizmos.color = Color.blue;
            DrawViewConeGizmo();
        }
    }

    private void DrawViewConeGizmo()
    {
        if (playerCamera == null) return;

        Vector3 cameraPos = playerCamera.transform.position;
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;
        Vector3 cameraUp = playerCamera.transform.up;

        float distance = 10f;
        float angle = viewAngleThreshold * Mathf.Deg2Rad;

        // Рисуем конус обзора
        Vector3[] conePoints = new Vector3[8];
        conePoints[0] = cameraPos;
        
        for (int i = 0; i < 4; i++)
        {
            float currentAngle = i * 90f * Mathf.Deg2Rad;
            Vector3 dir = cameraForward * Mathf.Cos(angle) + 
                         (cameraRight * Mathf.Cos(currentAngle) + cameraUp * Mathf.Sin(currentAngle)) * Mathf.Sin(angle);
            conePoints[i + 1] = cameraPos + dir * distance;
        }

        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(conePoints[0], conePoints[i + 1]);
            Gizmos.DrawLine(conePoints[i + 1], conePoints[(i + 1) % 4 + 1]);
        }
    }
}