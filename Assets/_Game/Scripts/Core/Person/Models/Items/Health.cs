using UnityEngine;

namespace BlockoutRoguelike.Core.Person.Models.Items
{
    public class Health : BaseItem
    {
        public override ItemType ItemType => ItemType.Health;
        public int Max { get; private set; }

        public Health(int currentHp, int maxHp) : base(currentHp) => UpdateMax(maxHp);

        public void UpdateMax(int newMax)
        {
            Max = newMax;
            ApplyLimits();
        }
        
        protected override void ApplyLimits() => Current = Mathf.Clamp(Current, 0, Max);

        public bool IsDead => Current <= 0;
        public override string ToString() => $"{Current}/{Max}";
    }
}