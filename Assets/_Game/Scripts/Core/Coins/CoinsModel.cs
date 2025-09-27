using System;

namespace NothingAwaits.Core.Coins
{
    public class CoinsModel
    {
        public int Amount { get; private set; }
        
        public event Action<int> OnChanged;

        public CoinsModel(int initialAmount = 0)
        {
            SetAmount(initialAmount);
        }

        public void SetAmount(int amount)
        {
            Amount = Math.Max(0, amount);
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
    }
}