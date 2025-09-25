using UnityEngine;

namespace BlockoutRoguelike.Core.Inventory.Data.Items
{
    public abstract class BaseItemData : ScriptableObject
    {
        [Header("Base Item Settings")]
        [SerializeField] private string _itemId = "new_item";
        [SerializeField] private string _displayName = "New Item";
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _maxStack = 1;
        [SerializeField] private bool _isStackable = true;

        public string ItemId => _itemId;
        public string DisplayName => _displayName;
        public Sprite Icon => _icon;
        public int MaxStack => _maxStack;
        public bool IsStackable => _isStackable && _maxStack > 1;

        public virtual string GetDescription() => $"Item: {_displayName}";
    }
}