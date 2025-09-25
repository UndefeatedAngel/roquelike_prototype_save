using System.Collections.Generic;
using BlockoutRoguelike.Core.Inventory.Data.Items;
using UnityEngine;

namespace BlockoutRoguelike.Core.Inventory.Data
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Roguelike/Items/Database")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField] private List<BaseItemData> _items = new List<BaseItemData>();
        private Dictionary<string, BaseItemData> _itemsDictionary;

        public void Initialize()
        {
            _itemsDictionary = new Dictionary<string, BaseItemData>();
            foreach (var item in _items)
            {
                if (item != null && !string.IsNullOrEmpty(item.ItemId))
                {
                    if (!_itemsDictionary.ContainsKey(item.ItemId))
                    {
                        _itemsDictionary.Add(item.ItemId, item);
                    }
                    else
                    {
                        Debug.LogWarning($"Duplicate item ID: {item.ItemId}");
                    }
                }
            }
            Debug.Log($"ItemDatabase initialized with {_itemsDictionary.Count} items");
        }

        public BaseItemData GetItem(string itemId)
        {
            if (_itemsDictionary == null) Initialize();
            if (string.IsNullOrEmpty(itemId)) return null;
            
            if (_itemsDictionary.TryGetValue(itemId, out BaseItemData item))
            {
                return item;
            }
            
            Debug.LogWarning($"Item not found in database: {itemId}");
            return null;
        }

        public bool ItemExists(string itemId)
        {
            if (_itemsDictionary == null) Initialize();
            return _itemsDictionary.ContainsKey(itemId);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Initialize();
            }
        }
#endif
    }
}