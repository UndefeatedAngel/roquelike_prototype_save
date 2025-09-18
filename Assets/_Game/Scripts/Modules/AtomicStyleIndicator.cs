using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace BlockoutRoguelike.Modules
{
    public class AtomicStyleIndicator : MonoBehaviour
    {
        [Header("Target Settings")] public Transform target;
        public LayerMask obstructionMask = 1; // Маска для препятствий

        [Header("UI References")] public RectTransform directionArrow; // Основная стрелка
        public RectTransform distanceMarker; // Маркер расстояния на стрелке
        public TMP_Text distanceText; // Цифровое расстояние
        public Image arrowGlow; // Свечение стрелки
        public CanvasGroup indicatorGroup; // Для плавного появления

        [Header("Visual Settings")] public Color normalColor = new Color(0.2f, 0.8f, 1f, 1f); // Голубой Atomic Heart
        public Color obstructedColor = new Color(1f, 0.3f, 0.2f, 1f); // Красный при препятствии
        public float pulseSpeed = 2f;
        public float maxArrowDistance = 200f;
        public float minScale = 0.7f;
        public float maxScale = 1.5f;

        [Header("Animation Settings")] public float fadeInDuration = 0.3f;
        public float fadeOutDuration = 0.5f;

        private Camera playerCamera;
        private float currentDistance;
        private bool isVisible = false;
        private float visibilityTimer = 0f;
        private bool isObstructed = false;

        private RectTransform _distanceTextRectTransform;

        void Start()
        {
            _distanceTextRectTransform = distanceText.GetComponent<RectTransform>();
            playerCamera = Camera.main;
            InitializeUI();
            HideIndicator();
        }

        void Update()
        {
            if (target == null)
            {
                HideIndicator();
                return;
            }

            UpdateDistance();
            CheckObstruction();
            UpdateIndicatorPosition();
            UpdateVisualEffects();
        }

        private void InitializeUI()
        {
            // Настройка начальных значений
            if (indicatorGroup != null)
            {
                indicatorGroup.alpha = 0f;
                indicatorGroup.blocksRaycasts = false;
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
            isObstructed = Physics.Raycast(transform.position, directionToTarget, out hit, distanceToTarget,
                obstructionMask);
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
                // Цель на экране - позиционируем стрелку рядом с целью
                directionArrow.position = screenPoint;
                directionArrow.rotation = Quaternion.identity;
                _distanceTextRectTransform.rotation = quaternion.identity;
                ShowIndicator();
            }
            else
            {
                // Цель за пределами экрана - показываем на краю
                PositionArrowAtScreenEdge(screenPoint);
                ShowIndicator();
            }

            UpdateDistanceMarker();
            UpdateArrowScale();
        }

        private void PositionArrowAtScreenEdge(Vector3 screenPoint)
        {
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Vector3 direction = (screenPoint - screenCenter).normalized;

            // Вычисляем позицию на краю экрана
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            directionArrow.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
            //_distanceTextRectTransform.rotation = Quaternion.Euler(0f, 0f, -directionArrow.eulerAngles.z);

            // Позиционируем с отступом от края
            float borderOffset = 80f;
            Vector3 edgePosition = new Vector3(
                Mathf.Clamp(screenPoint.x, borderOffset, Screen.width - borderOffset),
                Mathf.Clamp(screenPoint.y, borderOffset, Screen.height - borderOffset),
                0f
            );

            directionArrow.position = edgePosition;
        }

        private void UpdateDistanceMarker()
        {
            if (distanceMarker == null) return;

            // Позиционируем маркер расстояния вдоль стрелки
            float normalizedDistance = Mathf.Clamp01(currentDistance / maxArrowDistance);
            distanceMarker.anchoredPosition = new Vector2(0f, normalizedDistance * 100f);
        }

        private void UpdateArrowScale()
        {
            float scaleFactor = Mathf.Clamp01(currentDistance / maxArrowDistance);
            float scale = Mathf.Lerp(minScale, maxScale, scaleFactor);
            directionArrow.localScale = Vector3.one * scale;
        }

        private void UpdateVisualEffects()
        {
            // Пульсация свечения
            if (arrowGlow != null)
            {
                float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
                arrowGlow.color = new Color(arrowGlow.color.r, arrowGlow.color.g, arrowGlow.color.b,
                    0.3f + pulse * 0.4f);
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

            if (arrowGlow != null)
            {
                arrowGlow.color = Color.Lerp(arrowGlow.color, targetColor, Time.deltaTime * 5f);
            }
        }

        private void ShowIndicator()
        {
            if (!isVisible)
            {
                isVisible = true;
                StopAllCoroutines();
                StartCoroutine(FadeIndicator(1f, fadeInDuration));
            }

            visibilityTimer = 2f; // Сбрасываем таймер скрытия
        }

        private void HideIndicator()
        {
            if (isVisible)
            {
                isVisible = false;
                StopAllCoroutines();
                StartCoroutine(FadeIndicator(0f, fadeOutDuration));
            }
        }

        private System.Collections.IEnumerator FadeIndicator(float targetAlpha, float duration)
        {
            if (indicatorGroup == null) yield break;

            float startAlpha = indicatorGroup.alpha;
            float time = 0f;

            while (time < duration)
            {
                indicatorGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            indicatorGroup.alpha = targetAlpha;
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            ShowIndicator();
        }

        public void ClearTarget()
        {
            target = null;
            HideIndicator();
        }
    }
}
