using System;

namespace BlockoutRoguelike.Core.Actor.Models.Items
{
    public class Coins
    {
        public int Amount { get; private set; }
        
        public event Action<int> OnChanged;

        public Coins(int initialAmount = 0)
        {
            Amount = Math.Max(0, initialAmount);
        }

        public void Add(int amount)
        {
            if (amount <= 0) return;
            
            Amount += amount;
            OnChanged?.Invoke(Amount);
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0 || amount > Amount) 
                return false;
            
            Amount -= amount;
            OnChanged?.Invoke(Amount);
            return true;
        }

        public bool CanSpend(int amount) => amount > 0 && amount <= Amount;
    }
}