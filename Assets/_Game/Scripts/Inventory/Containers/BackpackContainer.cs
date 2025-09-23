using System;

namespace BlockoutRoguelike.Inventory.Containers
{
    public class BackpackContainer : BaseContainer
    {
        private int _capacity;
        
        public override string ContainerId => "backpack";
        public override string DisplayName => "Рюкзак";
        public override int SlotCount => _capacity;
        
        public BackpackContainer(int capacity = 20)
        {
            if (capacity <= 0) throw new ArgumentException("Capacity must be positive");
            
            _capacity = capacity;
            slots = new ContainerSlot[SlotCount];
            for (int i = 0; i < SlotCount; i++)
            {
                slots[i] = new ContainerSlot();
            }
        }
        
        public void Expand(int additionalSlots)
        {
            if (additionalSlots <= 0) return;
            
            var newSlots = new ContainerSlot[SlotCount + additionalSlots];
            slots.CopyTo(newSlots, 0);
            
            for (int i = SlotCount; i < newSlots.Length; i++)
            {
                newSlots[i] = new ContainerSlot();
            }
            
            slots = newSlots;
            _capacity = slots.Length;
            
            InvokeContainerChanged();
        }
    }
}