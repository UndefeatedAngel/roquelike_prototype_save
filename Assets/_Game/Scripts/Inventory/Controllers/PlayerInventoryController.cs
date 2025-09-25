using UnityEngine;
using BlockoutRoguelike.Core.Inventory.Services;
using BlockoutRoguelike.Core.Inventory.WorldInteraction;
using BlockoutRoguelike.Core.Inventory.Data;
using BlockoutRoguelike.Core.Inventory.Models;

namespace BlockoutRoguelike.Core.Inventory.Controllers
{
    public class PlayerInventoryController : MonoBehaviour
    {
        [Header("Inventory Settings")]
        [SerializeField] private ItemDatabase _itemDatabase;
        [SerializeField] private int _inventorySize = 6;
        [SerializeField] private float _pickupRadius = 2f;
        [SerializeField] private KeyCode _pickupKey = KeyCode.F;
        [SerializeField] private LayerMask _itemLayerMask = 1;
        
        [Header("Debug")]
        [SerializeField] private bool _showDebugGizmos = true;
        [SerializeField] private bool _logInventoryEvents = true;
        
        private InventoryService _inventoryService;
        private WorldItem _currentTargetItem;
        private bool _isInitialized = false;

        public InventoryService InventoryService => _inventoryService;

        private void Start()
        {
            InitializeInventory();
        }

        private void InitializeInventory()
        {
            if (_itemDatabase == null)
            {
                Debug.LogError("ItemDatabase is not assigned!");
                return;
            }

            // Инициализируем базу данных
            _itemDatabase.Initialize();
    
            // Устанавливаем в ServiceLocator ДО создания InventoryService
            ServiceLocator.ItemDatabase = _itemDatabase;
    
            _inventoryService = new InventoryService(_itemDatabase, _inventorySize);
            _inventoryService.OnInventoryChanged += OnInventoryChanged;
    
            // Загружаем сохранение ДО создания UI
            _inventoryService.Load();
    
            _isInitialized = true;
    
            if (_logInventoryEvents)
                Debug.Log($"Inventory initialized with {_inventorySize} slots");
        }

        private void Update()
        {
            if (!_isInitialized) return;

            FindPickupTarget();
            HandlePickupInput();
        }

        private void FindPickupTarget()
        {
            var hitColliders = Physics.OverlapSphere(transform.position, _pickupRadius, _itemLayerMask);
            WorldItem closestItem = null;
            float closestDistance = float.MaxValue;

            foreach (var collider in hitColliders)
            {
                var worldItem = collider.GetComponent<WorldItem>();
                if (worldItem != null && worldItem.CanBePickedUp)
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestItem = worldItem;
                    }
                }
            }

            _currentTargetItem = closestItem;
        }

        private void HandlePickupInput()
        {
            if (Input.GetKeyDown(_pickupKey) && _currentTargetItem != null)
            {
                var result = _inventoryService.PickupItem(_currentTargetItem);
                
                if (_logInventoryEvents)
                {
                    if (result.IsSuccess)
                        Debug.Log($"Picked up: {result.ItemData.DisplayName} x{result.Quantity}");
                    else
                        Debug.Log($"Pickup failed: {result.ErrorMessage}");
                }
            }
        }

        private void OnInventoryChanged(int slotIndex)
        {
            if (_logInventoryEvents)
            {
                if (slotIndex == -1)
                    Debug.Log("Inventory fully reloaded");
                else
                    Debug.Log($"Slot {slotIndex} changed");
            }
        }

        public void UseItem(int slotIndex)
        {
            if (!_isInitialized) return;
            
            // Простая логика использования - удаляем предмет
            if (_inventoryService.RemoveItem(slotIndex))
            {
                Debug.Log($"Used item from slot {slotIndex}");
            }
        }

        private void OnDrawGizmos()
        {
            if (!_showDebugGizmos) return;
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _pickupRadius);
        }

        private void OnApplicationQuit()
        {
            _inventoryService?.Save();
        }

        private void OnDestroy()
        {
            if (_inventoryService != null)
            {
                _inventoryService.OnInventoryChanged -= OnInventoryChanged;
                _inventoryService.Save();
            }
        }

        // Методы для взаимодействия с дверьми
        public bool HasItem(string itemId, int requiredQuantity = 1)
        {
            return _inventoryService?.GetItemCount(itemId) >= requiredQuantity;
        }

        public bool ConsumeItem(string itemId, int quantity = 1)
        {
            return _inventoryService?.CheckAndConsumeItem(itemId, quantity) ?? false;
        }

        [ContextMenu("Debug Inventory")]
        public void DebugInventory()
        {
            if (!_isInitialized) return;
            
            Debug.Log("=== INVENTORY DEBUG ===");
            for (int i = 0; i < _inventoryService.Inventory.SlotCount; i++)
            {
                var itemId = _inventoryService.Inventory.GetItemId(i);
                Debug.Log($"Slot {i}: {(string.IsNullOrEmpty(itemId) ? "EMPTY" : itemId)}");
            }
            Debug.Log("======================");
        }
    }
}