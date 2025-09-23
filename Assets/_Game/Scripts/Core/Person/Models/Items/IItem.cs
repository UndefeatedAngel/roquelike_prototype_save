namespace BlockoutRoguelike.Core.Person.Models.Items
{
    public interface IItem
    {
        ItemType ItemType { get; }
        int Current { get; }
        void Increase(int amount);
        void Decrease(int amount);
    }
}