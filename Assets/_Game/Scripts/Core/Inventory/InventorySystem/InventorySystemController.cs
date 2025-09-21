using System.Collections.Generic;
using UnityEngine;

namespace BlockoutRoguelike.Core.Inventory.InventorySystem
{
    public class InventorySystemController : MonoBehaviour
    {
        [SerializeField] private int inventorySlots = 20;
        
        public static InventorySystemController Instance { get; private set; }
        
        private List<BaseInventoryItem> items = new();
        
        public delegate void OnInventoryChanged();
        public event OnInventoryChanged onInventoryChanged;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        // Добавление предмета в инвентарь
        public bool AddItem(BaseInventoryItem newItem)
        {
            // Проверяем, можно ли добавить к существующему стеку
            if (newItem.IsStackable)
            {
                foreach (var item in items)
                {
                    if (item.GetType() == newItem.GetType() && 
                        item.Name == newItem.Name && 
                        item.CurrentStack < item.MaxStack)
                    {
                        int spaceLeft = item.MaxStack - item.CurrentStack;
                        int toAdd = Mathf.Min(spaceLeft, newItem.CurrentStack);
                        
                        item.CurrentStack += toAdd;
                        newItem.CurrentStack -= toAdd;
                        
                        if (newItem.CurrentStack <= 0)
                        {
                            NotifyInventoryChanged();
                            return true;
                        }
                    }
                }
            }
            
            // Добавляем в новый слот, если есть место
            if (items.Count < inventorySlots)
            {
                items.Add(newItem);
                NotifyInventoryChanged();
                return true;
            }
            
            Debug.Log("Инвентарь полон!");
            return false;
        }
        
        // Удаление предмета
        public bool RemoveItem(BaseInventoryItem itemToRemove)
        {
            if (items.Remove(itemToRemove))
            {
                NotifyInventoryChanged();
                return true;
            }
            return false;
        }
        
        // Использование предмета
        public bool UseItem(BaseInventoryItem itemToUse)
        {
            if (itemToUse.Use())
            {
                if (itemToUse.IsStackable)
                {
                    itemToUse.CurrentStack--;
                    if (itemToUse.CurrentStack <= 0)
                    {
                        RemoveItem(itemToUse);
                    }
                }
                else
                {
                    RemoveItem(itemToUse);
                }

                NotifyInventoryChanged();
                return true;
            }
            return false;
        }
        
        // Получение всех предметов
        public List<BaseInventoryItem> GetItems()
        {
            return new List<BaseInventoryItem>(items);
        }
        
        // Поиск предмета по ID
        public BaseInventoryItem FindItemById(string itemId)
        {
            return items.Find(item => item.Id == itemId);
        }
        
        // Поиск ключа по ID двери
        public KeyItem FindKeyForDoor(string doorId)
        {
            return items.Find(item => item is KeyItem key && key.DoorId == doorId) as KeyItem;
        }
    
        // Поиск ключа по ID ключа (точное совпадение)
        public KeyItem FindKeyById(string keyId)
        {
            return items.Find(item => item is KeyItem key && key.KeyId == keyId) as KeyItem;
        }
    
        // Поиск всех ключей для определённой двери
        public List<KeyItem> FindAllKeysForDoor(string doorId)
        {
            return items.FindAll(item => item is KeyItem key && key.DoorId == doorId)
                .ConvertAll(item => item as KeyItem);
        }
    
        // Поиск всех ключей с определённым ID ключа
        public List<KeyItem> FindAllKeysById(string keyId)
        {
            return items.FindAll(item => item is KeyItem key && key.KeyId == keyId)
                .ConvertAll(item => item as KeyItem);
        }
        
        // Проверка наличия места
        public bool HasSpace()
        {
            return items.Count < inventorySlots;
        }
        
        // Сохранение инвентаря
        public void SaveInventory()
        {
            // Реализация сохранения (PlayerPrefs, JSON, etc.)
            string json = JsonUtility.ToJson(new InventoryData(items));
            PlayerPrefs.SetString("Inventory", json);
            PlayerPrefs.Save();
        }
        
        public void NotifyInventoryChanged()
        {
            onInventoryChanged?.Invoke();
        }
        
        // Загрузка инвентаря
        public void LoadInventory()
        {
            if (PlayerPrefs.HasKey("Inventory"))
            {
                string json = PlayerPrefs.GetString("Inventory");
                InventoryData data = JsonUtility.FromJson<InventoryData>(json);
                items = data.Items;
                NotifyInventoryChanged();
            }
        }
        
        [System.Serializable]
        private class InventoryData
        {
            public List<BaseInventoryItem> Items;
            
            public InventoryData(List<BaseInventoryItem> items)
            {
                Items = items;
            }
        }
    }
}