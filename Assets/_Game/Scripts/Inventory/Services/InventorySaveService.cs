using System;
using System.Collections.Generic;
using BlockoutRoguelike.Inventory.Containers;
using BlockoutRoguelike.Inventory.Items;
using BlockoutRoguelike.DataStorage;
using UnityEngine;

namespace BlockoutRoguelike.Inventory.Services
{
    public class InventorySaveService
    {
        private const string SaveKey = "roguelike.inventory";
        private IDataStorage _storage;
        
        public InventorySaveService(IDataStorage storage = null)
        {
            _storage = storage ?? StorageManager.Storage;
        }
        
        [System.Serializable]
        private class InventorySaveData
        {
            public List<ContainerSaveData> containers = new List<ContainerSaveData>();
        }
        
        [System.Serializable]
        private class ContainerSaveData
        {
            public string containerId;
            public List<SlotSaveData> slots = new List<SlotSaveData>();
        }
        
        [System.Serializable]
        private class SlotSaveData
        {
            public string itemId;
            public int stackSize;
            public string itemData;
        }
        
        [System.Serializable]
        private class SacrificeItemData
        {
            public float health;
        }
        
        [System.Serializable]
        private class PotionItemData
        {
            public string displayName;
        }
        
        [System.Serializable]
        private class KeyItemData
        {
            public string doorId;
            public string displayName;
        }
        
        public void SaveInventory(InventoryService inventory)
        {
            try
            {
                var saveData = new InventorySaveData();
                
                SaveContainer(inventory.QuickSlotsWithKeys, saveData);
                SaveContainer(inventory.QuickSlotsWithoutKeys, saveData);
                SaveContainer(inventory.Backpack, saveData);
                SaveContainer(inventory.SacrificeSlots, saveData);
                SaveContainer(inventory.Bench, saveData);
                
                string jsonData = JsonUtility.ToJson(saveData);
                _storage.SaveString($"{SaveKey}_data", jsonData);
                
                Debug.Log("Inventory saved successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save inventory: {e.Message}");
            }
        }
        
        public bool LoadInventory(InventoryService inventory)
        {
            try
            {
                if (!_storage.HasKey($"{SaveKey}_data"))
                {
                    Debug.Log("No saved inventory found");
                    return false;
                }
                
                string jsonData = _storage.LoadString($"{SaveKey}_data", "");
                if (string.IsNullOrEmpty(jsonData))
                {
                    Debug.Log("Empty inventory save data");
                    return false;
                }
                
                var saveData = JsonUtility.FromJson<InventorySaveData>(jsonData);
                if (saveData == null)
                {
                    Debug.LogError("Failed to parse inventory save data");
                    return false;
                }
                
                LoadContainer(inventory.QuickSlotsWithKeys, saveData);
                LoadContainer(inventory.QuickSlotsWithoutKeys, saveData);
                LoadContainer(inventory.Backpack, saveData);
                LoadContainer(inventory.SacrificeSlots, saveData);
                LoadContainer(inventory.Bench, saveData);
                
                Debug.Log("Inventory loaded successfully");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load inventory: {e.Message}");
                return false;
            }
        }
        
        private void SaveContainer(IContainer container, InventorySaveData saveData)
        {
            var containerData = new ContainerSaveData
            {
                containerId = container.ContainerId
            };
            
            for (int i = 0; i < container.SlotCount; i++)
            {
                if (container.TryGetItem(i, out var item))
                {
                    var slotData = new SlotSaveData
                    {
                        itemId = item.ItemId,
                        stackSize = item.CurrentStack,
                        itemData = SerializeItemData(item)
                    };
                    containerData.slots.Add(slotData);
                }
                else
                {
                    containerData.slots.Add(new SlotSaveData()); // Пустой слот
                }
            }
            
            saveData.containers.Add(containerData);
        }
        
        private void LoadContainer(IContainer container, InventorySaveData saveData)
        {
            var containerData = saveData.containers.Find(c => c.containerId == container.ContainerId);
            if (containerData == null) return;
            
            for (int i = 0; i < container.SlotCount; i++)
            {
                container.TryRemoveItem(i, out _);
            }
            
            for (int i = 0; i < Math.Min(containerData.slots.Count, container.SlotCount); i++)
            {
                var slotData = containerData.slots[i];
                if (!string.IsNullOrEmpty(slotData.itemId))
                {
                    var item = CreateItemFromSaveData(slotData);
                    if (item != null)
                    {
                        container.TryAddItem(item, i);
                    }
                }
            }
        }
        
        private string SerializeItemData(IInventoryItem item)
        {
            try
            {
                if (item is SacrificeItem sacrifice)
                {
                    var data = new SacrificeItemData { health = sacrifice.Health };
                    return JsonUtility.ToJson(data);
                }
                else if (item is PotionItem potion)
                {
                    var data = new PotionItemData { displayName = potion.DisplayName };
                    return JsonUtility.ToJson(data);
                }
                else if (item is KeyItem key)
                {
                    var data = new KeyItemData { doorId = key.DoorId, displayName = key.DisplayName };
                    return JsonUtility.ToJson(data);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to serialize item data: {e.Message}");
            }
            
            return "";
        }
        
        private IInventoryItem CreateItemFromSaveData(SlotSaveData slotData)
        {
            try
            {
                if (slotData.itemId.StartsWith("potion_"))
                {
                    var data = string.IsNullOrEmpty(slotData.itemData) 
                        ? new PotionItemData() 
                        : JsonUtility.FromJson<PotionItemData>(slotData.itemData);
                    
                    return new PotionItem(
                        slotData.itemId,
                        data.displayName ?? "Зелье",
                        maxStackSize: 10,
                        stack: slotData.stackSize
                    );
                }
                else if (slotData.itemId.StartsWith("key_"))
                {
                    var data = string.IsNullOrEmpty(slotData.itemData) 
                        ? new KeyItemData() 
                        : JsonUtility.FromJson<KeyItemData>(slotData.itemData);
                    
                    return new KeyItem(
                        slotData.itemId,
                        data.displayName ?? "Ключ",
                        data.doorId ?? "unknown",
                        stack: slotData.stackSize
                    );
                }
                else if (slotData.itemId == "sacrifice")
                {
                    var data = string.IsNullOrEmpty(slotData.itemData) 
                        ? new SacrificeItemData() 
                        : JsonUtility.FromJson<SacrificeItemData>(slotData.itemData);
                    
                    return new SacrificeItem(data.health);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create item from save data: {e.Message}");
            }
            
            return null;
        }
        
        public void DeleteSaveData()
        {
            _storage.DeleteKey($"{SaveKey}_data");
            Debug.Log("Inventory save data deleted");
        }
        
        public bool HasSaveData()
        {
            return _storage.HasKey($"{SaveKey}_data");
        }
    }
}