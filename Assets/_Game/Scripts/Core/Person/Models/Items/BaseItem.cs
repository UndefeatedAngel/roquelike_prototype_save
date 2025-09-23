using UnityEngine;
using System;

namespace BlockoutRoguelike.Core.Person.Models.Items
{
    public abstract class BaseItem : IItem
    {
        public abstract ItemType ItemType { get; }
        public int Current { get; protected set; }
        public event Action<int> OnChanged;

        protected BaseItem(int current) => Current = current;

        public virtual void Increase(int amount)
        {
            Current += amount;
            ApplyLimits();
            OnChanged?.Invoke(Current);
        }

        public virtual void Decrease(int amount)
        {
            Current -= amount;
            ApplyLimits();
            OnChanged?.Invoke(Current);
        }

        protected virtual void ApplyLimits() => Current = Mathf.Max(Current, 0);
    }
}