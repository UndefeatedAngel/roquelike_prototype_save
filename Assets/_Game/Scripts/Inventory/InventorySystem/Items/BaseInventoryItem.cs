using System;
using UnityEngine;

namespace NothingAwaits.Core.Inventory.InventorySystem
{
    [Serializable]
    public abstract class BaseInventoryItem
    {
        public string Id;
        public string Name;
        public string Description;
        public Sprite Icon;
        public int MaxStack = 1;
        public int CurrentStack = 1;
        public bool IsStackable => MaxStack > 1;
    
        public virtual bool Use() { return false; }
    }
}