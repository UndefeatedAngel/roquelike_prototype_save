using System;
using BlockoutRoguelike.Core.Inventory.Data.Items;

namespace BlockoutRoguelike.Core.Inventory.Models
{
    [Serializable]
    public class InventorySlot
    {
        private string _itemId = string.Empty;
        private int _quantity = 0;
        
        public string ItemId => _itemId;
        public int Quantity => _quantity;
        public bool IsEmpty => string.IsNullOrEmpty(_itemId) || _quantity <= 0;

        // Ссылка на данные предмета (будет установлена позже через ServiceLocator)
        public BaseItemData ItemData { get; set; }

        public void SetItem(string itemId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemId))
                throw new ArgumentException("ItemId cannot be null or empty");
                
            _itemId = itemId;
            _quantity = Math.Max(1, quantity);
        }

        public bool CanAddItem(string itemId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemId)) return false;
            if (IsEmpty) return true;
            return _itemId == itemId;
        }

        public void Clear()
        {
            _itemId = string.Empty;
            _quantity = 0;
            ItemData = null;
        }

        // Для сохранения
        public SlotData ToData() => new SlotData { itemId = _itemId, quantity = _quantity };
        
        public void FromData(SlotData data)
        {
            _itemId = data.itemId ?? string.Empty;
            _quantity = data.quantity;
        }
    }

    [Serializable]
    public struct SlotData
    {
        public string itemId;
        public int quantity;
    }
}