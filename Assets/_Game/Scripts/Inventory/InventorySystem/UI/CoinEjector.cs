using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

namespace NothingAwaits.Core.Inventory.InventorySystem.UI
{
    public class CoinEjector : MonoBehaviour
    {
        [Header("Настройки выброса")]
        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private Transform ejectPosition;
        [SerializeField] private int minCoins = 5;
        [SerializeField] private int maxCoins = 15;
        
        [Header("Направление выброса")]
        [SerializeField] private Vector3 ejectDirection = Vector3.forward;
        [SerializeField] private float ejectAngle = 45f;
        [SerializeField] private float minDistanceFromChest = 1f;
        
        [Header("Сила выброса")]
        [SerializeField] private float minForce = 8f;
        [SerializeField] private float maxForce = 15f;
        [SerializeField] private float minTorque = 3f;
        [SerializeField] private float maxTorque = 8f;
        
        [Header("Задержка между монетами")]
        [SerializeField] private float delayBetweenCoins = 0.1f;
        
        [Header("Визуализация")]
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private Color gizmoColor = Color.yellow;
        
        private bool isEjecting = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                TestEject();
            }
        }

        public void EjectCoins()
        {
            if (isEjecting) return;
            
            if (coinPrefab == null)
            {
                Debug.LogWarning("Coin prefab is not assigned!", this);
                return;
            }
            
            StartCoroutine(EjectCoinsCoroutine());
        }
        
        private IEnumerator EjectCoinsCoroutine()
        {
            isEjecting = true;
            
            int coinCount = Random.Range(minCoins, maxCoins + 1);
            Debug.Log($"Выбрасываем {coinCount} монет");
            
            for (int i = 0; i < coinCount; i++)
            {
                EjectSingleCoin();
                yield return new WaitForSeconds(delayBetweenCoins);
            }
            
            isEjecting = false;
        }
        
        private void EjectSingleCoin()
        {
            Vector3 spawnPosition = GetEjectPosition();
            Vector3 direction = GetEjectDirection();
            float force = Random.Range(minForce, maxForce);
            
            GameObject coin = Instantiate(coinPrefab, spawnPosition, Random.rotation);
            
            Rigidbody rb = coin.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Основная сила выброса
                rb.AddForce(direction * force, ForceMode.Impulse);
                
                // Случайное вращение
                Vector3 randomTorque = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                ) * Random.Range(minTorque, maxTorque);
                
                rb.AddTorque(randomTorque, ForceMode.Impulse);
            }
            
            // Автоуничтожение через время
            Destroy(coin, 7f);
        }
        
        private Vector3 GetEjectPosition()
        {
            if (ejectPosition != null)
            {
                return ejectPosition.position;
            }
            
            // Если точка выброса не задана, используем позицию объекта + смещение
            return transform.position + transform.forward * 0.5f;
        }
        
        private Vector3 GetEjectDirection()
        {
            // Нормализуем направление
            Vector3 normalizedDirection = ejectDirection.normalized;
            
            // Преобразуем направление в мировые координаты
            Vector3 worldDirection = transform.TransformDirection(normalizedDirection);
            
            // Добавляем случайный угол
            float randomAngle = Random.Range(-ejectAngle, ejectAngle);
            Quaternion randomRotation = Quaternion.Euler(0, randomAngle, 0);
            
            return randomRotation * worldDirection;
        }
        
        private void OnDrawGizmos()
        {
            if (!showGizmos) return;
            
            Gizmos.color = gizmoColor;
            
            Vector3 position = GetEjectPosition();
            Vector3 baseDirection = transform.TransformDirection(ejectDirection.normalized);
            
            // Рисуем основное направление
            Gizmos.DrawRay(position, baseDirection * 2f);
            
            // Рисуем конус возможных направлений
            DrawEjectCone(position, baseDirection);
            
            // Рисуем минимальную дистанцию от сундука
            DrawMinDistanceZone();
        }
        
        private void DrawEjectCone(Vector3 position, Vector3 direction)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            
            // Боковые линии конуса
            Quaternion leftRotation = Quaternion.Euler(0, -ejectAngle, 0);
            Quaternion rightRotation = Quaternion.Euler(0, ejectAngle, 0);
            
            Vector3 leftDirection = leftRotation * direction;
            Vector3 rightDirection = rightRotation * direction;
            
            Gizmos.DrawRay(position, leftDirection * 2f);
            Gizmos.DrawRay(position, rightDirection * 2f);
            
            // Дуга конуса
            DrawDirectionArc(position, direction, ejectAngle, 2f);
        }
        
        private void DrawDirectionArc(Vector3 center, Vector3 direction, float angle, float length)
        {
            int segments = 20;
            float angleStep = angle * 2 / segments;
            
            Vector3 previousPoint = center + Quaternion.Euler(0, -angle, 0) * direction * length;
            
            for (int i = 1; i <= segments; i++)
            {
                float currentAngle = -angle + angleStep * i;
                Vector3 currentPoint = center + Quaternion.Euler(0, currentAngle, 0) * direction * length;
                
                Gizmos.DrawLine(previousPoint, currentPoint);
                previousPoint = currentPoint;
            }
        }
        
        private void DrawMinDistanceZone()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            
            // Рисуем зону, куда монеты не должны попадать
            Vector3 chestPosition = transform.position;
            Gizmos.DrawWireSphere(chestPosition, minDistanceFromChest);
            
            // Красная зона - запретная область
            Gizmos.color = new Color(1f, 0f, 0f, 0.1f);
            Gizmos.DrawSphere(chestPosition, minDistanceFromChest);
        }
        
        // Метод для проверки, что позиция не слишком близко к сундуку
        private bool IsValidPosition(Vector3 position)
        {
            return Vector3.Distance(position, transform.position) >= minDistanceFromChest;
        }
        
        [ContextMenu("Тест выброса монет")]
        private void TestEject()
        {
            EjectCoins();
        }
        
        [ContextMenu("Настроить направление вперед")]
        private void SetForwardDirection()
        {
            ejectDirection = Vector3.forward;
            Debug.Log("Направление установлено вперед");
        }
        
        [ContextMenu("Настроить направление вверх")]
        private void SetUpwardDirection()
        {
            ejectDirection = Vector3.up;
            Debug.Log("Направление установлено вверх");
        }
    }
}