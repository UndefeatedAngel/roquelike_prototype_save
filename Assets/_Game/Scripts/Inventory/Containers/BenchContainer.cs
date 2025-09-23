using System;
using System.Collections.Generic;
using BlockoutRoguelike.Inventory.Items;

namespace BlockoutRoguelike.Inventory.Containers
{
    public class BenchContainer : BaseContainer
    {
        public override string ContainerId => "bench";
        public override string DisplayName => "Скамья запасных";
        public override int SlotCount { get; }
        
        public float HealTimePerHealthPoint { get; set; } = 0.2f;
        
        private Dictionary<int, float> _healingProgress = new Dictionary<int, float>();
        private Dictionary<int, float> _healingTargets = new Dictionary<int, float>();
        
        public BenchContainer(int slotCount = 6)
        {
            if (slotCount <= 0) throw new ArgumentException("Slot count must be positive");
            SlotCount = slotCount;
            slots = new ContainerSlot[SlotCount];
            for (int i = 0; i < SlotCount; i++)
            {
                slots[i] = new ContainerSlot();
            }
        }
        
        public void UpdateHealing(float deltaTime)
        {
            for (int i = 0; i < SlotCount; i++)
            {
                if (TryGetItem(i, out var item) && item is SacrificeItem sacrifice && sacrifice.IsInjured)
                {
                    if (!_healingProgress.ContainsKey(i))
                    {
                        _healingProgress[i] = 0f;
                        _healingTargets[i] = 100f - sacrifice.Health; // Сколько здоровья нужно восстановить
                    }
                    
                    float healthToHeal = _healingTargets[i];
                    float totalHealTime = healthToHeal * HealTimePerHealthPoint;
                    
                    _healingProgress[i] += deltaTime;
                    
                    if (totalHealTime > 0)
                    {
                        float healProgress = _healingProgress[i] / totalHealTime;
                        float targetHealth = 100f - healthToHeal + (healthToHeal * Math.Clamp(healProgress, 0f, 1f));
                        
                        float actualHealAmount = targetHealth - sacrifice.Health;
                        if (actualHealAmount > 0)
                        {
                            sacrifice.Heal(actualHealAmount);
                            InvokeSlotChanged(i);
                        }
                        
                        if (healProgress >= 1f)
                        {
                            _healingProgress.Remove(i);
                            _healingTargets.Remove(i);
                        }
                    }
                }
                else
                {
                    _healingProgress.Remove(i);
                    _healingTargets.Remove(i);
                }
            }
        }
        
        public float GetHealingProgress(int slotIndex)
        {
            if (!_healingProgress.ContainsKey(slotIndex) || !_healingTargets.ContainsKey(slotIndex))
                return 0f;
                
            float totalHealTime = _healingTargets[slotIndex] * HealTimePerHealthPoint;
            if (totalHealTime <= 0) return 1f;
            
            return Math.Clamp(_healingProgress[slotIndex] / totalHealTime, 0f, 1f);
        }
        
        public float GetRemainingHealingTime(int slotIndex)
        {
            if (!_healingProgress.ContainsKey(slotIndex) || !_healingTargets.ContainsKey(slotIndex))
                return 0f;
                
            float totalHealTime = _healingTargets[slotIndex] * HealTimePerHealthPoint;
            return Math.Max(0f, totalHealTime - _healingProgress[slotIndex]);
        }
        
        public bool HasInjuredSacrifices()
        {
            for (int i = 0; i < SlotCount; i++)
            {
                if (TryGetItem(i, out var item) && item is SacrificeItem sacrifice && sacrifice.IsInjured)
                    return true;
            }
            return false;
        }
        
        public IEnumerable<SacrificeItem> GetInjuredSacrifices()
        {
            for (int i = 0; i < SlotCount; i++)
            {
                if (TryGetItem(i, out var item) && item is SacrificeItem sacrifice && sacrifice.IsInjured)
                    yield return sacrifice;
            }
        }
        
        public override bool CanAddItem(IInventoryItem item, int slotIndex = -1)
        {
            return item is SacrificeItem && base.CanAddItem(item, slotIndex);
        }
    }
}