using System;
using UnityEngine;

namespace NothingAwaits.Core.Inventory.InventorySystem
{
    public class GenericImageItem: BaseInventoryItem
    {
        public GenericImageItem(Sprite sprite, int hp)
        {
            Id = "item_" + Guid.NewGuid().ToString();
            Icon = sprite;
            CurrentStack = MaxStack = hp;
        }
    }
}