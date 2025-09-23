using BlockoutRoguelike.Core.Person.Models;
using BlockoutRoguelike.Core.Person.Models.Items;
using BlockoutRoguelike.Inventory.Items;
using BlockoutRoguelike.Inventory.Services;

namespace BlockoutRoguelike.Adapters
{
    public class InventoryToCoreAdapter
    {
        private InventoryService _inventoryService;
        
        public InventoryToCoreAdapter(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }
        
        public IInventoryItem ConvertToInventoryItem(IItem coreItem)
        {
            if (coreItem is Health healthItem)
            {
                // Health -> PotionItem
                return new PotionItem(
                    "health_potion", 
                    "Зелье здоровья", 
                    maxStackSize: 5, 
                    stack: 1
                );
            }
            else if (coreItem is Coins coinsItem)
            {
                return new PotionItem(
                    "coins",
                    "Монеты",
                    maxStackSize: 9999,
                    stack: coinsItem.Current
                );
            }
            
            return null;
        }
        
        public IItem ConvertToCoreItem(IInventoryItem inventoryItem)
        {
            if (inventoryItem is PotionItem potion)
            {
                if (potion.ItemId == "health_potion")
                {
                    return new Health(50, 100);
                }
                else if (potion.ItemId == "coins")
                {
                    return new Coins(potion.CurrentStack);
                }
            }
            
            return null;
        }
        
        public bool UseInventoryItemOnPlayer(IInventoryItem item, PlayerModel playerModel)
        {
            if (item is PotionItem potion)
            {
                if (potion.ItemId == "health_potion")
                {
                    playerModel.Health.Increase(30);
                    return true;
                }
            }
            else if (item is SacrificeItem sacrifice)
            {
                // TODO: Реализация ритуалов
                return true;
            }
            
            return false;
        }
        
        public bool AddCoreItemToInventory(IItem coreItem)
        {
            var inventoryItem = ConvertToInventoryItem(coreItem);
            if (inventoryItem != null)
            {
                return _inventoryService.TryAddItemToAnyContainer(inventoryItem);
            }
            return false;
        }
        
        public bool HasItemForCore(string coreItemId)
        {
            var inventoryItemId = coreItemId switch
            {
                "health" => "health_potion",
                "coins" => "coins",
                _ => coreItemId
            };
            
            return _inventoryService.HasItem(inventoryItemId);
        }
        
        public int GetItemCountForCore(string coreItemId)
        {
            var inventoryItemId = coreItemId switch
            {
                "health" => "health_potion",
                "coins" => "coins",
                _ => coreItemId
            };
            
            return _inventoryService.GetTotalItemCount(inventoryItemId);
        }
    }
}