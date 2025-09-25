using System;
using UnityEngine;
using BlockoutRoguelike.Core.Inventory.Models;
using BlockoutRoguelike.Core.Inventory.WorldInteraction;
using BlockoutRoguelike.Core.Inventory.Data;
using BlockoutRoguelike.DataStorage;

namespace BlockoutRoguelike.Core.Inventory.Services
{
    public class InventoryService
    {
        private const string SaveKey = "inventory.main";
        
        private readonly InventoryModel _inventory;
        private readonly IDataStorage _storage;
        private readonly ItemDatabase _itemDatabase;

        public InventoryModel Inventory => _inventory;
        public event Action<int> OnInventoryChanged;

        public InventoryService(ItemDatabase itemDatabase, int slotCount = 3)
        {
            if (itemDatabase == null)
                throw new ArgumentNullException(nameof(itemDatabase));
                
            _itemDatabase = itemDatabase;
            _inventory = new InventoryModel(slotCount);
            _storage = StorageManager.Storage;
            
            // Подписываемся на изменения слотов
            _inventory.OnSlotChanged += slotIndex => OnInventoryChanged?.Invoke(slotIndex);
        }

        public void Load()
        {
            if (_storage.HasKey(SaveKey))
            {
                var data = _storage.LoadString(SaveKey);
                Debug.Log($"Loading inventory: {data}");
                
                if (!string.IsNullOrEmpty(data))
                {
                    try
                    {
                        var inventoryData = JsonUtility.FromJson<InventoryData>(data);
                        _inventory.FromData(inventoryData);
                        Debug.Log("Inventory loaded successfully");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to load inventory: {e.Message}");
                    }
                }
            }
            else
            {
                Debug.Log("No inventory save found");
            }
        }

        public void Save()
        {
            var data = _inventory.ToData();
            var json = JsonUtility.ToJson(data);
            _storage.SaveString(SaveKey, json);
            Debug.Log("Inventory saved");
        }

        public PickupResult PickupItem(WorldItem worldItem)
        {
            if (worldItem == null)
                return PickupResult.CreateFailed("WorldItem is null");

            var pickupResult = worldItem.TryPickUp();
            if (!pickupResult.IsSuccess)
                return pickupResult;

            bool added = _inventory.TryAddItem(pickupResult.ItemData.ItemId, pickupResult.Quantity);
            
            if (added)
            {
                worldItem.OnPickupSuccess();
                Save();
                return new PickupResult { IsSuccess = true, ItemData = pickupResult.ItemData, Quantity = pickupResult.Quantity };
            }
            else
            {
                string error = "Inventory is full";
                worldItem.OnPickupFailed(error);
                return PickupResult.CreateFailed(error);
            }
        }

        public AddItemResult AddItem(string itemId, int quantity = 1)
        {
            if (!_itemDatabase.ItemExists(itemId))
                return AddItemResult.CreateFailed($"Item not found: {itemId}");

            bool success = _inventory.TryAddItem(itemId, quantity);
            if (success)
            {
                Save();
                return AddItemResult.CreateSuccess(itemId, quantity);
            }
            else
            {
                return AddItemResult.CreateFailed("No space in inventory");
            }
        }

        public bool RemoveItem(int slotIndex)
        {
            bool success = _inventory.RemoveItem(slotIndex);
            if (success) Save();
            return success;
        }

        public int GetItemCount(string itemId)
        {
            int count = 0;
            for (int i = 0; i < _inventory.SlotCount; i++)
            {
                if (_inventory.GetItemId(i) == itemId)
                {
                    // Для простоты считаем каждый слот как 1 предмет
                    count++;
                }
            }
            return count;
        }

        public bool CheckAndConsumeItem(string itemId, int requiredQuantity)
        {
            int currentCount = GetItemCount(itemId);
            if (currentCount < requiredQuantity) return false;

            // Упрощенная логика списания - удаляем первые найденные предметы
            int consumed = 0;
            for (int i = 0; i < _inventory.SlotCount && consumed < requiredQuantity; i++)
            {
                if (_inventory.GetItemId(i) == itemId)
                {
                    RemoveItem(i);
                    consumed++;
                }
            }
            return consumed == requiredQuantity;
        }
    }

    public struct AddItemResult
    {
        public bool IsSuccess;
        public string ItemId;
        public int Quantity;
        public string ErrorMessage;
        
        public static AddItemResult CreateSuccess(string itemId, int quantity) => new AddItemResult 
        { 
            IsSuccess = true, 
            ItemId = itemId, 
            Quantity = quantity 
        };
        
        public static AddItemResult CreateFailed(string error) => new AddItemResult 
        { 
            IsSuccess = false, 
            ErrorMessage = error 
        };
    }
}