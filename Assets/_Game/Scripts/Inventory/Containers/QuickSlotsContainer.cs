using System;
using BlockoutRoguelike.Inventory.Items;

namespace BlockoutRoguelike.Inventory.Containers
{
    public class QuickSlotsContainer : BaseContainer
    {
        public override string ContainerId => "quick_slots";
        public override string DisplayName => "Быстрые слоты";
        public override int SlotCount { get; }
        
        public QuickSlotsContainer(int slotCount = 3)
        {
            if (slotCount <= 0) throw new ArgumentException("Slot count must be positive");
            SlotCount = slotCount;
            slots = new ContainerSlot[SlotCount];
            for (int i = 0; i < SlotCount; i++)
            {
                slots[i] = new ContainerSlot();
            }
        }
        
        public bool TryUseItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotCount) return false;
            if (!TryGetItem(slotIndex, out var item)) return false;
            
            item.Use();
            
            if (item is PotionItem potion && potion.CurrentStack <= 0)
            {
                TryRemoveItem(slotIndex, out _);
            }
            
            OnQuickSlotUsed?.Invoke(slotIndex);
            return true;
        }
        
        public event Action<int> OnQuickSlotUsed;
    }
}