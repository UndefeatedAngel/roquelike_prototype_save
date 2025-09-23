using System;

namespace BlockoutRoguelike.Inventory.Items
{
    public class PotionItem : BaseInventoryItem
    {
        public override string ItemId { get; }
        public override string DisplayName { get; }
        public override int MaxStackSize { get; }
        
        public PotionItem(string itemId, string displayName, int maxStackSize = 10, int stack = 1) : base(stack)
        {
            ItemId = itemId;
            DisplayName = displayName;
            MaxStackSize = Math.Max(1, maxStackSize);
            CurrentStack = Math.Clamp(CurrentStack, 1, MaxStackSize);
        }

        public override IInventoryItem Clone()
        {
            return new PotionItem(ItemId, DisplayName, MaxStackSize, CurrentStack);
        }

        public override void Use()
        {
            if (CurrentStack > 0)
            {
                CurrentStack--;
                OnPotionUsed?.Invoke(this);
            }
        }

        public override bool CanStackWith(IInventoryItem other)
        {
            return other is PotionItem otherPotion && 
                   otherPotion.ItemId == ItemId && 
                   base.CanStackWith(other);
        }

        public event Action<PotionItem> OnPotionUsed;
    }
}