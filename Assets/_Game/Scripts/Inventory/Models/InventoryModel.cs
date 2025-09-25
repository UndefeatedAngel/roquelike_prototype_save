using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlockoutRoguelike.Core.Inventory.Models
{
    [Serializable]
    public class InventoryModel
    {
        private readonly InventorySlot[] _slots;

        public IReadOnlyList<InventorySlot> Slots => _slots;
        public int SlotCount => _slots.Length;
        public event Action<int> OnSlotChanged;

        public InventoryModel(int slotCount = 3)
        {
            if (slotCount <= 0) 
                throw new ArgumentException("Slot count must be positive", nameof(slotCount));
                
            _slots = new InventorySlot[slotCount];
            for (int i = 0; i < slotCount; i++)
            {
                _slots[i] = new InventorySlot();
            }
        }

        public bool TryAddItem(string itemId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemId) || quantity <= 0) 
            {
                Debug.LogWarning($"Invalid item data: {itemId} x{quantity}");
                return false;
            }

            // Сначала ищем пустой слот
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].IsEmpty)
                {
                    _slots[i].SetItem(itemId, quantity);
                    OnSlotChanged?.Invoke(i);
                    Debug.Log($"Item added to empty slot {i}: {itemId} x{quantity}");
                    return true;
                }
            }

            Debug.Log($"No empty slots for item: {itemId}");
            return false;
        }

        public bool RemoveItem(int slotIndex, int quantity = 1)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Length) 
            {
                Debug.LogWarning($"Invalid slot index: {slotIndex}");
                return false;
            }
            
            if (_slots[slotIndex].IsEmpty) 
            {
                Debug.LogWarning($"Slot {slotIndex} is empty");
                return false;
            }
            
            _slots[slotIndex].Clear();
            OnSlotChanged?.Invoke(slotIndex);
            Debug.Log($"Item removed from slot {slotIndex}");
            return true;
        }

        public string GetItemId(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Length) return string.Empty;
            return _slots[slotIndex].ItemId;
        }

        public bool HasEmptySlots()
        {
            foreach (var slot in _slots)
            {
                if (slot.IsEmpty) return true;
            }
            return false;
        }

        // Для сохранения
        public InventoryData ToData()
        {
            var data = new InventoryData(_slots.Length);
            for (int i = 0; i < _slots.Length; i++)
            {
                data.slots[i] = _slots[i].ToData();
            }
            return data;
        }

        public void FromData(InventoryData data)
        {
            if (data.slots == null) return;

            int slotsToLoad = Math.Min(_slots.Length, data.slots.Length);
            for (int i = 0; i < slotsToLoad; i++)
            {
                _slots[i].FromData(data.slots[i]);
            }

            // Уведомляем об изменении всех загруженных слотов
            for (int i = 0; i < slotsToLoad; i++)
            {
                OnSlotChanged?.Invoke(i);
            }
        }
    }

    [Serializable]
    public struct InventoryData
    {
        public SlotData[] slots;

        public InventoryData(int slotCount)
        {
            slots = new SlotData[slotCount];
        }
    }
}