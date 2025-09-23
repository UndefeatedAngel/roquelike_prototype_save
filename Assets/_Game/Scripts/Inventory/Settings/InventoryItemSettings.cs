using UnityEngine;

namespace BlockoutRoguelike.Inventory.Items
{
    [CreateAssetMenu(fileName = "InventoryItemSettings", menuName = "BlouckoutRoquelike/Inventory/InventoryItemSettings")]
    public class InventoryItemSettings : ScriptableObject
    {
        [Header("Basic Info")]
        public string itemId;
        public string displayName;
        public Sprite icon;
        
        [Header("Inventory Settings")]
        public int maxStackSize = 1;
        public ItemType itemType;
        
        [Header("Usage Settings")]
        public bool isUsable = true;
        public float useCooldown = 0f;
        
        [TextArea]
        public string description;
    }

    public enum ItemType
    {
        Potion,
        Key,
        Sacrifice,
        Resource,
        Miscellaneous
    }
}