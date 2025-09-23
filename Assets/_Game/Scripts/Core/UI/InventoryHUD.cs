using UnityEngine;
using BlockoutRoguelike.Inventory.Services;
using BlockoutRoguelike.Inventory.UI;

namespace BlockoutRoguelike.Core.UI
{
    public class InventoryHUD : MonoBehaviour
    {
        [SerializeField] private QuickSlotsUI activeSlotsUI; 
        [SerializeField] private QuickSlotsUI resourceSlotsUI;
        
        private InventoryService _inventoryService;
        
        public void Initialize(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;

            if (activeSlotsUI != null)
                activeSlotsUI.Initialize(inventoryService.QuickSlotsWithKeys, true);
            
            if (resourceSlotsUI != null)
                resourceSlotsUI.Initialize(inventoryService.QuickSlotsWithoutKeys, false);
        }
        
        public void Refresh()
        {
            
        }
    }
}