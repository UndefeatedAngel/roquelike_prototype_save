using System;

namespace NothingAwaits.Core.Inventory.InventorySystem
{
    [Serializable]
    public class GenericItem : BaseInventoryItem
    {
        public GenericItem(string name, string description, int maxStack = 1)
        {
            Id = "item_" + Guid.NewGuid().ToString();
            Name = name;
            Description = description;
            MaxStack = maxStack;
        }
    }
}