namespace BlockoutRoguelike.Core.Person.Models.Items
{
    public class Coins : BaseItem
    {
        public override ItemType ItemType => ItemType.Coins;
    
        public Coins(int initialAmount = 0) : base(int.MaxValue) => Current = initialAmount;

        protected override void ApplyLimits() { if (Current < 0) Current = 0; }

        public bool CanSpend(int amount) => Current >= amount;

        public bool TrySpend(int amount)
        {
            if (CanSpend(amount))
            {
                Decrease(amount);
                return true;
            }
            return false;
        }
    }
}