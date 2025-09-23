using System;
using System.Collections.Generic;
using BlockoutRoguelike.Inventory.Items;

namespace BlockoutRoguelike.Inventory.Containers
{
    public interface IContainer
    {
        string ContainerId { get; }
        string DisplayName { get; }
        int SlotCount { get; }
        bool IsReadOnly { get; }
        
        bool CanAddItem(IInventoryItem item, int slotIndex = -1);
        bool TryAddItem(IInventoryItem item, int slotIndex = -1);
        bool TryRemoveItem(int slotIndex, out IInventoryItem item);
        bool TryGetItem(int slotIndex, out IInventoryItem item);
        bool TrySwapSlots(int slotIndexA, int slotIndexB);
        
        IEnumerable<int> FindSlotsWithItem(string itemId);
        int GetTotalItemCount(string itemId);
        
        event Action<IContainer, int> OnSlotChanged;
        event Action<IContainer> OnContainerChanged;
    }
}