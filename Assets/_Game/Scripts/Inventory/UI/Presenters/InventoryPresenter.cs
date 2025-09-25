using UnityEngine;
using System.Collections;
using BlockoutRoguelike.Core.Inventory.Services;
using BlockoutRoguelike.Core.Inventory.UI.Views;
using BlockoutRoguelike.Core.Inventory.Controllers;
using BlockoutRoguelike.Core.Inventory.Data;
using BlockoutRoguelike.Core.Inventory.Models;

namespace BlockoutRoguelike.Core.Inventory.UI.Presenters
{
    public class InventoryPresenter : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private InventoryView _inventoryView;
        [SerializeField] private PlayerInventoryController _playerInventoryController;
        
        private InventoryService _inventoryService;
        private ItemDatabase _itemDatabase;
        private bool _isInitialized = false;

        private void Start()
        {
            if (_inventoryView == null)
                _inventoryView = GetComponent<InventoryView>();
                
            if (_playerInventoryController == null)
                _playerInventoryController = FindObjectOfType<PlayerInventoryController>();

            StartCoroutine(InitializeCoroutine());
        }

        private IEnumerator InitializeCoroutine()
        {
            // Ждем инициализации контроллера
            yield return new WaitUntil(() => _playerInventoryController != null && 
                                           _playerInventoryController.InventoryService != null);

            _inventoryService = _playerInventoryController.InventoryService;
            _itemDatabase = ServiceLocator.ItemDatabase;
            
            if (_itemDatabase != null)
            {
                ServiceLocator.ItemDatabase = _itemDatabase;
            }

            // Создаем слоты UI
            _inventoryView.CreateSlots(_inventoryService.Inventory.SlotCount);
            
            // Ждем создания слотов
            yield return new WaitUntil(() => _inventoryView.SlotCount == _inventoryService.Inventory.SlotCount);

            // Подписываемся на события
            _inventoryService.OnInventoryChanged += OnInventoryChanged;
            
            // Инициализируем UI
            InitializeUI();
            _isInitialized = true;
            
            Debug.Log("InventoryPresenter initialized successfully");
        }

        private void InitializeUI()
        {
            if (_inventoryService == null || _inventoryView == null) return;

            Debug.Log($"Initializing UI with {_inventoryService.Inventory.SlotCount} slots");
            
            // Обновляем все слоты
            for (int i = 0; i < _inventoryService.Inventory.SlotCount; i++)
            {
                UpdateSlotUI(i);
            }
        }

        private void OnInventoryChanged(int slotIndex)
        {
            Debug.Log($"Presenter: Inventory changed - slot {slotIndex}");
            
            if (slotIndex == -1)
            {
                // Полная перезагрузка
                InitializeUI();
            }
            else
            {
                // Обновление конкретного слота
                UpdateSlotUI(slotIndex);
            }
        }

        private void UpdateSlotUI(int slotIndex)
        {
            if (!IsValidSlotIndex(slotIndex)) return;

            var slot = _inventoryService.Inventory.Slots[slotIndex];
            Sprite itemSprite = GetSpriteForItem(slot.ItemId);
            
            _inventoryView.UpdateSlot(slotIndex, itemSprite, slot.Quantity, slot.ItemId);
        }

        private Sprite GetSpriteForItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return null;
            
            var itemData = ServiceLocator.ItemDatabase?.GetItem(itemId);
            return itemData?.Icon;
        }

        private bool IsValidSlotIndex(int slotIndex)
        {
            return _isInitialized && 
                   _inventoryService != null && 
                   _inventoryView != null &&
                   slotIndex >= 0 && 
                   slotIndex < _inventoryService.Inventory.SlotCount && 
                   slotIndex < _inventoryView.SlotCount;
        }

        public void OnSlotClicked(int slotIndex)
        {
            if (!_isInitialized || !IsValidSlotIndex(slotIndex)) return;
            
            Debug.Log($"Presenter: Using item from slot {slotIndex}");
            _playerInventoryController.UseItem(slotIndex);
        }

        public void OnSlotHoverEnter(int slotIndex)
        {
            if (!_isInitialized || !IsValidSlotIndex(slotIndex)) return;
            _inventoryView.HighlightSlot(slotIndex, true);
        }

        public void OnSlotHoverExit(int slotIndex)
        {
            if (!_isInitialized || !IsValidSlotIndex(slotIndex)) return;
            _inventoryView.HighlightSlot(slotIndex, false);
        }

        private void OnDestroy()
        {
            if (_inventoryService != null)
            {
                _inventoryService.OnInventoryChanged -= OnInventoryChanged;
            }
        }
    }
}