using System;

namespace NothingAwaits.Core.Health
{
    public class HealthModel
    {
        public int Current { get; private set; }
        public int Max { get; private set; }
        public bool IsDead => Current <= 0;
        
        public event Action<int> OnChanged;

        public HealthModel(int current, int max)
        {
            SetHealth(current, max);
        }

        public void SetHealth(int current, int max)
        {
            Current = Math.Clamp(current, 0, max);
            Max = Math.Max(1, max);
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0 || IsDead) return;
            
            Current = Math.Max(0, Current - damage);
            OnChanged?.Invoke(Current);
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || IsDead) return;
            
            Current = Math.Min(Max, Current + amount);
            OnChanged?.Invoke(Current);
        }

        public void FullHeal()
        {
            Current = Max;
            OnChanged?.Invoke(Current);
        }

        public void HealMax(int additionalMax)
        {
            Max += Math.Max(0, additionalMax);
            Current = Math.Min(Max, Current + additionalMax);
            OnChanged?.Invoke(Current);
        }

        public override string ToString() => $"{Current}/{Max}";
    }
}