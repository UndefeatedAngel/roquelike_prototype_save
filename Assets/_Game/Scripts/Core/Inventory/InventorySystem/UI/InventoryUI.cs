using System.Collections.Generic;
using UnityEngine;

namespace BlockoutRoguelike.Core.Inventory.InventorySystem.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("Inventory Slots")]
        [SerializeField] private InventorySlotUI[] _slots;
        
        private List<InventorySlotUI> inventorySlots = new();
        
        private void Start()
        {
            inventorySlots = new List<InventorySlotUI>();
            // Инициализируем список слотов
            foreach (var slot in _slots)
            {
                inventorySlots.Add(slot);
            }
            
            InventorySystemController.Instance.onInventoryChanged += UpdateInventoryUI;
            
            // Инициализируем все слоты как пустые
            InitializeAllSlots();
        }
        
        private void InitializeAllSlots()
        {
            foreach (var slot in inventorySlots)
            {
                if (slot != null)
                {
                    slot.Initialize(null);
                }
            }
        }
        
        private void UpdateInventoryUI()
        {
            var allItems = InventorySystemController.Instance.GetItems();
            UpdateSlotsUI(allItems);
        }
        
        private void UpdateSlotsUI(List<BaseInventoryItem> items)
        {
            // Очищаем все слоты
            InitializeAllSlots();
            
            // Заполняем слоты предметами (максимум 3)
            for (int i = 0; i < Mathf.Min(items.Count, inventorySlots.Count); i++)
            {
                if (inventorySlots[i] != null)
                {
                    inventorySlots[i].Initialize(items[i]);
                }
            }
        }
        
        private void OnDestroy()
        {
            if (InventorySystemController.Instance != null)
            {
                InventorySystemController.Instance.onInventoryChanged -= UpdateInventoryUI;
            }
        }
    }
}