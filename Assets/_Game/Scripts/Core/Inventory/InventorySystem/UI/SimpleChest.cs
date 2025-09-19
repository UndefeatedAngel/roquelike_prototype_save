using UnityEngine;

namespace BlockoutRoguelike.Core.Inventory.InventorySystem.UI
{
    public class SimpleChest : MonoBehaviour
    {
        [Header("Настройки сундука")]
        public string requiredKeyId; // Должен совпадать с keyId ключа
        public string doorId; // Должен совпадать с doorId ключа
        public Transform chestOpenTransform;
        
        [Header("Визуальные настройки")]
        [SerializeField] private float openAngle = 45f;
        [SerializeField] private float openSpeed = 2f;
        
        [Header("Подсказка")]
        [SerializeField] private GameObject tooltipPrefab;
        [SerializeField] private Vector3 tooltipOffset = new Vector3(0, 1.5f, 0);
        
        private GameObject tooltipInstance;
        private bool playerInRange = false;
        private bool isOpen = false;
        private Quaternion closedRotation;
        private Quaternion openRotation;
        
        private void Start()
        {
            closedRotation = chestOpenTransform.rotation;
            openRotation = Quaternion.Euler(openAngle, 0, 0) * closedRotation;
            CreateTooltip();
            UpdateTooltip();
        }
        
        private void CreateTooltip()
        {
            if (tooltipPrefab != null)
            {
                tooltipInstance = Instantiate(tooltipPrefab, transform.position + tooltipOffset, Quaternion.identity);
                tooltipInstance.transform.SetParent(transform);
                tooltipInstance.SetActive(false);
            }
        }
        
        private void UpdateTooltip()
        {
            if (tooltipInstance != null)
            {
                ItemTooltip tooltip = tooltipInstance.GetComponent<ItemTooltip>();
                if (tooltip != null)
                {
                    tooltip.SetItemInfo("Сундук", "В сундуке можно найти сокровища!");
                    if (isOpen)
                    {
                        tooltip.SetInteractMessage("Сундук уже открыт");
                    }
                    else
                    {
                        tooltip.SetInteractMessage("Нажмите F чтобы открыть");
                    }
                }
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !isOpen)
            {
                playerInRange = true;
                ShowTooltip();
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
                HideTooltip();
            }
        }
        
        private void Update()
        {
            if (playerInRange && Input.GetKeyDown(KeyCode.F) && !isOpen)
            {
                TryOpenChest();
            }
            
            // Плавное открытие/закрытие
            if (isOpen)
            {
                chestOpenTransform.rotation = Quaternion.Lerp(chestOpenTransform.rotation, openRotation, openSpeed * Time.deltaTime);
            }
            else
            {
                chestOpenTransform.rotation = Quaternion.Lerp(chestOpenTransform.rotation, closedRotation, openSpeed * Time.deltaTime);
            }
        }
        
        private void TryOpenChest()
        {
            // Ищем ключ в инвентаре
            KeyItem key = InventorySystemController.Instance.FindKeyForDoor(doorId);
            
            if (key != null && key.KeyId == requiredKeyId)
            {
                // Ключ найден - открываем сундук
                OpenChest();
                Debug.Log($"Сундук открыт ключом: {key.KeyId}");
            }
            else
            {
                ItemTooltip tooltip = tooltipInstance.GetComponent<ItemTooltip>();
                if (tooltip != null)
                {
                    tooltip.SetItemInfo("Сундук", "В сундуке можно найти сокровища!");
                    if (!isOpen)
                    {
                        tooltip.SetInteractMessage("Найдите ключ чтобы открыть сундук!");
                    }
                }
                // Ключ не найден
                Debug.Log("Нужен ключ чтобы открыть этот сундук!");
                Debug.Log($"Требуется ключ с ID: {requiredKeyId} для двери: {doorId}");
            }
        }
        
        private void OpenChest()
        {
            isOpen = true;
            UpdateTooltip();
            HideTooltip();
            
            // Можно добавить звук открытия
            // Можно добавить выдачу лута здесь
            
            Debug.Log("Сундук открыт!");
        }
        
        private void ShowTooltip()
        {
            if (tooltipInstance != null && !isOpen)
            {
                tooltipInstance.SetActive(true);
            }
        }
        
        private void HideTooltip()
        {
            if (tooltipInstance != null)
            {
                tooltipInstance.SetActive(false);
            }
        }
        
        // Метод для принудительного открытия (для тестов)
        public void ForceOpen()
        {
            OpenChest();
        }
        
        // Метод для проверки, открыт ли сундук
        public bool IsOpen()
        {
            return isOpen;
        }
        
        private void OnDestroy()
        {
            if (tooltipInstance != null)
            {
                Destroy(tooltipInstance);
            }
        }
    }
}