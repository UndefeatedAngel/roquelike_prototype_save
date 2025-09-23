using System;

namespace BlockoutRoguelike.Inventory.Items
{
    public class KeyItem : BaseInventoryItem
    {
        public override string ItemId { get; }
        public override string DisplayName { get; }
        public override int MaxStackSize => 5;
        
        public string DoorId { get; private set; }
        
        public KeyItem(string itemId, string displayName, string doorId, int stack = 1) : base(stack)
        {
            ItemId = itemId;
            DisplayName = displayName;
            DoorId = doorId;
        }

        public override IInventoryItem Clone()
        {
            return new KeyItem(ItemId, DisplayName, DoorId, CurrentStack);
        }

        public override void Use()
        {
            OnKeyUsed?.Invoke(this);
        }

        public override bool CanStackWith(IInventoryItem other)
        {
            return other is KeyItem otherKey && 
                   otherKey.ItemId == ItemId && 
                   base.CanStackWith(other);
        }

        public event Action<KeyItem> OnKeyUsed;
    }
}