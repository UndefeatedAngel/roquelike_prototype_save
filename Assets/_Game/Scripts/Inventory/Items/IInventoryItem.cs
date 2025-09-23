namespace BlockoutRoguelike.Inventory.Items
{
    public interface IInventoryItem
    {
        string ItemId { get; }
        string DisplayName { get; }
        int MaxStackSize { get; }
        int CurrentStack { get; }
        bool IsStackable { get; }
        
        bool CanStackWith(IInventoryItem other);
        bool TryStack(ref IInventoryItem other);
        IInventoryItem Clone();
        void Use();
    }
}