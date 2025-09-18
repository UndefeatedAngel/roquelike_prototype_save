using UnityEngine;
using UnityEngine.EventSystems;

namespace BlockoutRoguelike.Modules
{
    public class ModelRotator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("References")] public Transform modelToRotate; // Ссылка на вашу 3D модель

        [Header("Rotation Settings")] public float rotationSpeed = 2f;
        public float maxRotationSpeed = 10f;
        public float inertiaDamping = 0.95f;
        public float stopThreshold = 0.1f;

        [Header("UI Settings")] public RectTransform dragArea; // Ваш Rect2D UI элемент

        private bool isDragging = false;
        private Vector2 lastMousePosition;
        private float currentRotationVelocity = 0f;
        private float targetRotation = 0f;

        void Update()
        {
            HandleDragging();
            HandleInertia();
            ApplyRotation();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            isDragging = true;
            lastMousePosition = Input.mousePosition;
            currentRotationVelocity = 0f; // Сбрасываем инерцию при начале drag
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            isDragging = false;
            // Инерция уже установлена в HandleDragging
        }

        private void HandleDragging()
        {
            if (!isDragging) return;

            Vector2 currentMousePosition = Input.mousePosition;
            float deltaX = currentMousePosition.x - lastMousePosition.x;

            // Вычисляем скорость вращения на основе движения мыши
            currentRotationVelocity = Mathf.Clamp(
                deltaX * rotationSpeed * Time.deltaTime,
                -maxRotationSpeed,
                maxRotationSpeed
            );

            lastMousePosition = currentMousePosition;
        }

        private void HandleInertia()
        {
            if (!isDragging)
            {
                // Применяем инерцию и замедление
                currentRotationVelocity *= inertiaDamping;

                // Останавливаем вращение если скорость слишком мала
                if (Mathf.Abs(currentRotationVelocity) < stopThreshold)
                {
                    currentRotationVelocity = 0f;
                }
            }
        }

        private void ApplyRotation()
        {
            if (modelToRotate != null && Mathf.Abs(currentRotationVelocity) > 0.01f)
            {
                modelToRotate.Rotate(Vector3.up, -currentRotationVelocity, Space.World);
            }
        }

        // Для отладки - сброс вращения по двойному клику
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 2)
            {
                modelToRotate.rotation = Quaternion.identity;
                currentRotationVelocity = 0f;
            }
        }
    }
}