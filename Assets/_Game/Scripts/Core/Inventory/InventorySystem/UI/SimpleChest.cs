using System.Collections.Generic;
using _Game.Scripts.Modules;
using BlockoutRoguelike.Modules;
using UnityEngine;

namespace BlockoutRoguelike.Core.Inventory.InventorySystem.UI
{
    public class SimpleChest : MonoBehaviour
    {
        [Header("Настройки сундука")]
        public Transform chestOpenTransform;
        public List<string> requiredKeyIds; // Должен совпадать с keyId ключа
        public string doorId; // Должен совпадать с doorId ключа
        public List<int> requiredKeyCounts; // Количество каждого ключа
        
        [Header("Визуальные настройки")]
        [SerializeField] private float openAngle = 45f;
        [SerializeField] private float openSpeed = 2f;
        
        [Header("Подсказка")]
        [SerializeField] private GameObject tooltipPrefab;
        [SerializeField] private Vector3 tooltipOffset = new(0, 1.5f, 0);
        
        [Header("Открыт сундука")]
        [SerializeField] private CoinEjector _coinEjector;
        [SerializeField] private GameObject[] _disableObjects;
        [SerializeField] private MoveObject _doorExit;
        
        private GameObject tooltipInstance;
        private bool playerInRange;
        private bool isOpen;
        private Quaternion closedRotation;
        private Quaternion openRotation;
        private Dictionary<string, int> foundKeyCounts = new();
        
        private void Start()
        {
            closedRotation = chestOpenTransform.rotation;
            openRotation = Quaternion.Euler(openAngle, 0, 0) * closedRotation;
            
            // Инициализируем словарь найденных ключей
            foreach (var keyId in requiredKeyIds)
            {
                foundKeyCounts[keyId] = 0;
            }
        
            // Если количества не заданы, устанавливаем по умолчанию 1
            if (requiredKeyCounts == null || requiredKeyCounts.Count != requiredKeyIds.Count)
            {
                requiredKeyCounts = new List<int>();
                foreach (var keyId in requiredKeyIds)
                {
                    requiredKeyCounts.Add(1);
                }
            }
            
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
                    if (isOpen)
                    {
                        tooltip.SetItemInfo("Сундук уже открыт", "Вы все забрали!");
                        tooltip.SetInteractMessage("");
                    }
                    else
                    {
                        if (IsReadyToOpen())
                        {
                            tooltip.SetItemInfo("Сундук", $"Здесь вы получите сокровища!");
                            tooltip.SetInteractMessage("Нажмите F чтобы открыть");
                        }
                        else
                        {
                            string status = GetKeyProgressText();
                            tooltip.SetItemInfo("Сундук заперт", $"Найдите ключи {status} чтобы забрать сокровища!");
                            tooltip.SetInteractMessage("");
                        }
                    }
                }
            }
        }
        
        private string GetKeyProgressText()
        {
            var foundCounts = 0;
            var requiredCounts = 0;
            for (var i = 0; i < requiredKeyIds.Count; i++)
            {
                var keyId = requiredKeyIds[i];
                requiredCounts+= requiredKeyCounts[i];
                foundCounts+= foundKeyCounts.ContainsKey(keyId) ? foundKeyCounts[keyId] : 0;
            }
            return $"{foundCounts}/{requiredCounts}";
        }

        private bool IsReadyToOpen()
        {
            var foundCounts = 0;
            var requiredCounts = 0;
            for (var i = 0; i < requiredKeyIds.Count; i++)
            {
                var keyId = requiredKeyIds[i];
                requiredCounts+= requiredKeyCounts[i];
                foundCounts+= foundKeyCounts.ContainsKey(keyId) ? foundKeyCounts[keyId] : 0;
            }
            return foundCounts == requiredCounts;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !isOpen)
            {
                playerInRange = true;
                CheckPlayerKeys();
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
            chestOpenTransform.rotation = Quaternion.Lerp(chestOpenTransform.rotation, isOpen ? openRotation : closedRotation, openSpeed * Time.deltaTime);
        }
        
        private void CheckPlayerKeys()
        {
            // Сбрасываем счётчики
            foreach (var keyId in requiredKeyIds)
            {
                foundKeyCounts[keyId] = 0;
            }
        
            // Проверяем все ключи в инвентаре
            var allKeys = InventorySystemController.Instance.FindAllKeysForDoor(doorId);
        
            foreach (var key in allKeys)
            {
                if (requiredKeyIds.Contains(key.KeyId))
                {
                    foundKeyCounts[key.KeyId] += key.CurrentStack;
                }
            }
        
            UpdateTooltip();
        }
        
        private void TryOpenChest()
        {
            CheckPlayerKeys();
        
            bool hasAllKeys = true;
            for (int i = 0; i < requiredKeyIds.Count; i++)
            {
                string keyId = requiredKeyIds[i];
                int requiredCount = requiredKeyCounts[i];
                int foundCount = foundKeyCounts[keyId];
            
                if (foundCount < requiredCount)
                {
                    hasAllKeys = false;
                    Debug.Log($"Не хватает ключа {keyId}: {foundCount}/{requiredCount}");
                }
            }
        
            if (hasAllKeys)
            {
                OpenChest();
            }
            else
            {
                Debug.Log("Не все ключи собраны!");
            }
        }
        
        private void OpenChest()
        {
            isOpen = true;
            RemoveUsedKeys();
            UpdateTooltip();
            HideTooltip();
            Debug.Log("Сундук открыт!");
            foreach (var disableObject in _disableObjects)
            {
                disableObject.SetActive(false);
            }
            Invoke("CoinEject", 1f);
            Invoke("DoorExitOpen", 3f);
        }

        private void CoinEject()
        {
            _coinEjector.EjectCoins();
        }

        private void DoorExitOpen()
        {
            _doorExit.Move();
        }
        
        private void RemoveUsedKeys()
        {
            var allKeys = InventorySystemController.Instance.FindAllKeysForDoor(doorId);
        
            for (int i = 0; i < requiredKeyIds.Count; i++)
            {
                string keyId = requiredKeyIds[i];
                int requiredCount = requiredKeyCounts[i];
                int remainingToRemove = requiredCount;
            
                foreach (var key in allKeys)
                {
                    if (key.KeyId == keyId && remainingToRemove > 0)
                    {
                        int toRemove = Mathf.Min(remainingToRemove, key.CurrentStack);
                        key.CurrentStack -= toRemove;
                        remainingToRemove -= toRemove;
                    
                        if (key.CurrentStack <= 0)
                        {
                            InventorySystemController.Instance.RemoveItem(key);
                        }
                    
                        if (remainingToRemove <= 0) break;
                    }
                }
            }
            
            InventorySystemController.Instance.NotifyInventoryChanged();
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
        
        // Метод для добавления требуемого ключа
        public void AddRequiredKey(string keyId, int count = 1)
        {
            if (!requiredKeyIds.Contains(keyId))
            {
                requiredKeyIds.Add(keyId);
                requiredKeyCounts.Add(count);
                foundKeyCounts[keyId] = 0;
            }
            UpdateTooltip();
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