using UnityEngine;
using BlockoutRoguelike.Inventory.Items;
using BlockoutRoguelike.Inventory.Services;

namespace BlockoutRoguelike.Core.Interactable
{
    public class CollectableItem : MonoBehaviour
    {
        [Header("Item Data")]
        [SerializeField] private InventoryItemSettings itemData;
        [SerializeField] private int stackAmount = 1;
        
        [Header("Collection Settings")]
        [SerializeField] private bool destroyOnCollect = true;
        [SerializeField] private string playerTag = "Player";
        
        [Header("Visuals")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        // Событие для уведомления о сборе предмета
        public System.Action<CollectableItem, IInventoryItem> OnItemCollected;
        
        private void Start()
        {
            // Автоматически настраиваем визуал по ItemData
            if (spriteRenderer != null && itemData != null && itemData.icon != null)
            {
                spriteRenderer.sprite = itemData.icon;
            }
        }
        
        public void TryCollectItem()
        {
            if (itemData == null)
            {
                Debug.LogError("ItemData is not assigned!");
                return;
            }
            
            var inventoryService = FindInventoryService();
            if (inventoryService == null)
            {
                Debug.LogError("InventoryService not found!");
                return;
            }
            
            var itemFactory = new ItemFactoryService();
            IInventoryItem item = itemFactory.CreateItem(itemData, stackAmount);
            
            if (item != null && inventoryService.TryAddItemToAnyContainer(item))
            {
                Debug.Log($"Item collected: {item.DisplayName}");
                OnItemCollected?.Invoke(this, item);
                
                if (destroyOnCollect)
                    Destroy(gameObject);
            }
            else
            {
                Debug.Log("Failed to collect item - inventory full?");
            }
        }
        
        private InventoryService FindInventoryService()
        {
            var playerController = FindObjectOfType<PlayerController.PlayerController>();
            return playerController?.GetInventoryService();
        }
        
        // Методы для изменения параметров в runtime
        public void SetItemData(InventoryItemSettings newItemData, int newStackAmount = 1)
        {
            itemData = newItemData;
            stackAmount = newStackAmount;
            
            // Обновляем визуал
            if (spriteRenderer != null && itemData != null)
            {
                spriteRenderer.sprite = itemData.icon;
            }
        }
        
        public InventoryItemSettings GetItemData() => itemData;
        public int GetStackAmount() => stackAmount;
    }
}