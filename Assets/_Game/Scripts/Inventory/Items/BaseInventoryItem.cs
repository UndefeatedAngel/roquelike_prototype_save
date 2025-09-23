using UnityEngine;

namespace BlockoutRoguelike.Inventory.Items
{
    public abstract class BaseInventoryItem : IInventoryItem
    {
        public abstract string ItemId { get; }
        public abstract string DisplayName { get; }
        public abstract int MaxStackSize { get; }
        public int CurrentStack { get; protected set; }
        public bool IsStackable => MaxStackSize > 1;

        protected BaseInventoryItem(int initialStack = 1)
        {
            CurrentStack = Mathf.Clamp(initialStack, 1, MaxStackSize);
        }

        public virtual bool CanStackWith(IInventoryItem other)
        {
            return other is BaseInventoryItem otherItem && 
                   otherItem.ItemId == ItemId && 
                   CurrentStack < MaxStackSize;
        }

        public virtual bool TryStack(ref IInventoryItem other)
        {
            if (!CanStackWith(other)) return false;

            var otherItem = (BaseInventoryItem)other;
            int total = CurrentStack + otherItem.CurrentStack;
            
            if (total <= MaxStackSize)
            {
                CurrentStack = total;
                otherItem.CurrentStack = 0;
                return true;
            }
            else
            {
                int spaceLeft = MaxStackSize - CurrentStack;
                CurrentStack = MaxStackSize;
                otherItem.CurrentStack -= spaceLeft;
                return true;
            }
        }

        public abstract IInventoryItem Clone();

        public virtual void Use() 
        {
            Debug.Log($"Using {DisplayName}");
        }
    }
}