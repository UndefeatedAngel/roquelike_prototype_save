using System;

namespace NothingAwaits.Cards
{
    public class BenchDraggableItem : DragAndReturnSelector
    {
        public Action<Slot> OnSelectSlot;
        
        protected override void Take()
        {
            base.Take();

            var slot = _selector as Slot;
            if (slot != null)
            {
                OnSelectSlot?.Invoke(slot);
            }
        }
    }
}