using System;
using BlockoutRoguelike.Inventory.Items;

namespace BlockoutRoguelike.Inventory.Containers
{
    public class SacrificeContainer : BaseContainer
    {
        public override string ContainerId => "sacrifice_slots";
        public override string DisplayName => "Слоты жертв";
        public override int SlotCount { get; }
        
        public SacrificeContainer(int slotCount = 3)
        {
            if (slotCount <= 0) throw new ArgumentException("Slot count must be positive");
            SlotCount = slotCount;
            slots = new ContainerSlot[SlotCount];
            for (int i = 0; i < SlotCount; i++)
            {
                slots[i] = new ContainerSlot();
            }
        }
        
        public override bool CanAddItem(IInventoryItem item, int slotIndex = -1)
        {
            return item is SacrificeItem && base.CanAddItem(item, slotIndex);
        }
    }
}