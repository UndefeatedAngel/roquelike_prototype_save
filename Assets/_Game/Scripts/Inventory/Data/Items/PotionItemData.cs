using UnityEngine;

namespace BlockoutRoguelike.Core.Inventory.Data.Items
{
    [CreateAssetMenu(fileName = "New Potion", menuName = "Roguelike/Items/Potion")]
    public class PotionItemData : BaseItemData
    {
        [Header("Potion Settings")]
        [SerializeField] private int _healAmount = 20;
        [SerializeField] private bool _isHealthPotion = true;

        public int HealAmount => _healAmount;
        public bool IsHealthPotion => _isHealthPotion;

        public override string GetDescription()
        {
            return base.GetDescription() + $"\nRestores {_healAmount} health";
        }
    }
}