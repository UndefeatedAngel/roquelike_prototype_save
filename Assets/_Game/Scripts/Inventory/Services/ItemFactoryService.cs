using BlockoutRoguelike.Inventory.Items;
using UnityEngine;

namespace BlockoutRoguelike.Inventory.Services
{
    public class ItemFactoryService
    {
        public IInventoryItem CreateItem(InventoryItemSettings inventoryItemSettings, int stackAmount = 1)
        {
            if (inventoryItemSettings == null) return null;
            
            return inventoryItemSettings.itemType switch
            {
                ItemType.Potion => CreatePotionItem(inventoryItemSettings, stackAmount),
                ItemType.Key => CreateKeyItem(inventoryItemSettings, stackAmount),
                ItemType.Sacrifice => CreateSacrificeItem(inventoryItemSettings),
                _ => CreateBasicItem(inventoryItemSettings, stackAmount)
            };
        }
        
        private PotionItem CreatePotionItem(InventoryItemSettings inventoryItemSettings, int stackAmount)
        {
            return new PotionItem(
                inventoryItemSettings.itemId,
                inventoryItemSettings.displayName,
                inventoryItemSettings.maxStackSize,
                stackAmount
            );
        }
        
        private KeyItem CreateKeyItem(InventoryItemSettings inventoryItemSettings, int stackAmount)
        {
            return new KeyItem(
                inventoryItemSettings.itemId,
                inventoryItemSettings.displayName,
                "door_generic", // Можно сделать настраиваемым
                stackAmount
            );
        }
        
        private SacrificeItem CreateSacrificeItem(InventoryItemSettings inventoryItemSettings)
        {
            return new SacrificeItem(100f); // Полное здоровье по умолчанию
        }
        
        private PotionItem CreateBasicItem(InventoryItemSettings inventoryItemSettings, int stackAmount)
        {
            return new PotionItem(
                inventoryItemSettings.itemId,
                inventoryItemSettings.displayName,
                inventoryItemSettings.maxStackSize,
                stackAmount
            );
        }
        
        public InventoryItemSettings LoadItemData(string itemId)
        {
            // Загрузка ItemData из Resources
            var itemData = Resources.Load<InventoryItemSettings>($"Items/{itemId}");
            if (itemData == null)
            {
                Debug.LogError($"ItemData not found: {itemId}");
            }
            return itemData;
        }
    }
}