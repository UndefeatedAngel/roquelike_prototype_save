using BlockoutRoguelike.Inventory.Items;

namespace BlockoutRoguelike.Inventory.Containers
{
    public class ContainerSlot
    {
        public IInventoryItem Item { get; private set; }
        public bool IsEmpty => Item == null;
        
        public bool CanAcceptItem(IInventoryItem item)
        {
            if (item == null) return true;
            if (IsEmpty) return true;
            
            return Item.CanStackWith(item);
        }
        
        public bool TrySetItem(IInventoryItem item)
        {
            if (!CanAcceptItem(item)) return false;
            
            if (IsEmpty)
            {
                Item = item;
                return true;
            }
            
            return Item.TryStack(ref item);
        }
        
        public bool TryClear()
        {
            if (IsEmpty) return false;
            Item = null;
            return true;
        }
        
        public IInventoryItem GetItemAndClear()
        {
            var item = Item;
            Item = null;
            return item;
        }
    }
}