using System;

namespace NothingAwaits.Core.Inventory.InventorySystem
{
    [Serializable]
    public class BottleItem : BaseInventoryItem
    {
        public float Capacity;
        public float CurrentLiquid;
    
        public BottleItem()
        {
            Id = "bottle_" + Guid.NewGuid().ToString();
            Name = "Бутылка";
            Description = "Стеклянная бутылка для жидкостей";
            MaxStack = 3;
        }
    }
}