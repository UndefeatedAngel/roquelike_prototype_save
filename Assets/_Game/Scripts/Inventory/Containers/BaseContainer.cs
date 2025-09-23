using System;
using System.Collections.Generic;
using BlockoutRoguelike.Inventory.Items;

namespace BlockoutRoguelike.Inventory.Containers
{
    public abstract class BaseContainer : IContainer
    {
        public abstract string ContainerId { get; }
        public abstract string DisplayName { get; }
        public abstract int SlotCount { get; }
        public virtual bool IsReadOnly => false;
        
        protected ContainerSlot[] slots;
        
        public event Action<IContainer, int> OnSlotChanged;
        public event Action<IContainer> OnContainerChanged;

        protected BaseContainer()
        {
            slots = new ContainerSlot[SlotCount];
            for (int i = 0; i < SlotCount; i++)
            {
                slots[i] = new ContainerSlot();
            }
        }

        public virtual bool CanAddItem(IInventoryItem item, int slotIndex = -1)
        {
            if (item == null) return false;
            
            if (slotIndex >= 0)
            {
                if (slotIndex >= SlotCount) return false;
                return slots[slotIndex].CanAcceptItem(item);
            }
            else
            {
                for (int i = 0; i < SlotCount; i++)
                {
                    if (slots[i].CanAcceptItem(item))
                        return true;
                }
                return false;
            }
        }

        public virtual bool TryAddItem(IInventoryItem item, int slotIndex = -1)
        {
            if (!CanAddItem(item, slotIndex)) return false;
            
            if (slotIndex >= 0)
            {
                if (slots[slotIndex].TrySetItem(item))
                {
                    InvokeSlotAndContainerChanged(slotIndex);
                    return true;
                }
            }
            else
            {
                for (int i = 0; i < SlotCount; i++)
                {
                    if (slots[i].TrySetItem(item))
                    {
                        InvokeSlotAndContainerChanged(i);
                        return true;
                    }
                }
            }
            
            return false;
        }

        public virtual bool TryRemoveItem(int slotIndex, out IInventoryItem item)
        {
            item = null;
            if (slotIndex < 0 || slotIndex >= SlotCount) return false;
            if (slots[slotIndex].IsEmpty) return false;
            
            item = slots[slotIndex].GetItemAndClear();
            InvokeSlotAndContainerChanged(slotIndex);
            return true;
        }

        public virtual bool TryGetItem(int slotIndex, out IInventoryItem item)
        {
            item = null;
            if (slotIndex < 0 || slotIndex >= SlotCount) return false;
            if (slots[slotIndex].IsEmpty) return false;
            
            item = slots[slotIndex].Item;
            return true;
        }

        public virtual bool TrySwapSlots(int slotIndexA, int slotIndexB)
        {
            if (slotIndexA < 0 || slotIndexA >= SlotCount) return false;
            if (slotIndexB < 0 || slotIndexB >= SlotCount) return false;
            if (slotIndexA == slotIndexB) return true;
            
            var tempItem = slots[slotIndexA].Item;
            slots[slotIndexA].GetItemAndClear();
            slots[slotIndexB].TrySetItem(tempItem);
            
            InvokeSlotAndContainerChanged(slotIndexA);
            InvokeSlotAndContainerChanged(slotIndexB);
            return true;
        }

        public virtual IEnumerable<int> FindSlotsWithItem(string itemId)
        {
            for (int i = 0; i < SlotCount; i++)
            {
                if (!slots[i].IsEmpty && slots[i].Item.ItemId == itemId)
                    yield return i;
            }
        }

        public virtual int GetTotalItemCount(string itemId)
        {
            int total = 0;
            foreach (var slotIndex in FindSlotsWithItem(itemId))
            {
                if (TryGetItem(slotIndex, out var item))
                    total += item.CurrentStack;
            }
            return total;
        }

        protected void InvokeSlotChanged(int slotIndex)
        {
            OnSlotChanged?.Invoke(this, slotIndex);
        }
        
        protected void InvokeContainerChanged()
        {
            OnContainerChanged?.Invoke(this);
        }
        
        protected void InvokeSlotAndContainerChanged(int slotIndex)
        {
            OnSlotChanged?.Invoke(this, slotIndex);
            OnContainerChanged?.Invoke(this);
        }
    }
}