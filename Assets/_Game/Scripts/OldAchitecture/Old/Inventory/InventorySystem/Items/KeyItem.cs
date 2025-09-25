using System;

namespace BlockoutRoguelike.Core.Inventory.InventorySystem
{
    [Serializable]
    public class KeyItem : BaseInventoryItem
    {
        public string KeyId;
        public string DoorId;
    
        public KeyItem(string keyId, string doorId)
        {
            Id = "key_" + Guid.NewGuid().ToString();
            Name = "Ключ";
            Description = "Ключ от двери";
            KeyId = keyId;
            DoorId = doorId;
            MaxStack = 1;
        }
    }
}