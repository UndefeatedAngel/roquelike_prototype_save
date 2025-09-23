using System;

namespace BlockoutRoguelike.Inventory.Items
{
    public class SacrificeItem : BaseInventoryItem
    {
        public override string ItemId => "sacrifice";
        public override string DisplayName => "Жертва";
        public override int MaxStackSize => 1;
        
        public float Health { get; private set; }
        public bool IsInjured => Health < 100f;
        
        public SacrificeItem(float initialHealth = 100f) : base(1)
        {
            Health = Math.Clamp(initialHealth, 0f, 100f);
        }

        public void Heal(float amount)
        {
            Health = Math.Clamp(Health + amount, 0f, 100f);
        }

        public void TakeDamage(float damage)
        {
            Health = Math.Clamp(Health - damage, 0f, 100f);
        }

        public override IInventoryItem Clone()
        {
            return new SacrificeItem(Health);
        }

        public override void Use()
        {
            OnItemUsed?.Invoke(this);
        }
        
        public event Action<SacrificeItem> OnItemUsed;
    }
}