using UnityEngine;

namespace BlockoutRoguelike.Core.Inventory.Data.Items
{
    [CreateAssetMenu(fileName = "New Food", menuName = "Roguelike/Items/Food")]
    public class FoodItemData : BaseItemData
    {
        [Header("Food Settings")]
        [SerializeField] private int _hpAdd = 10;
        [SerializeField] private float _duration = 0f; // 0 для мгновенного эффекта

        public int HpAdd => _hpAdd;
        public float Duration => _duration;

        public override string GetDescription()
        {
            string effectType = _duration > 0 ? $"over {_duration}s" : "instantly";
            return base.GetDescription() + $"\nRestores {_hpAdd} HP {effectType}";
        }
    }
}