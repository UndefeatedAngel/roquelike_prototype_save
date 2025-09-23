using UnityEngine;

namespace BlockoutRoguelike.Core.PlayerController
{
    public partial class PlayerController
    {
#if UNITY_EDITOR
        [SerializeField] private int _testDamageAmount = 10;
        [SerializeField] private int _testHealAmount = 5;

        public void TestTakeDamage()
        {
            if (_playerModel == null) return;
            Debug.Log($"Before damage: {_playerModel.Health.Current}/{_playerModel.Health.Max}");
            TakeDamage(_testDamageAmount);
            Debug.Log($"After damage: {_playerModel.Health.Current}/{_playerModel.Health.Max}");
        }

        public void TestHeal()
        {
            if (_playerModel == null) return;
            Debug.Log($"Before heal: {_playerModel.Health.Current}/{_playerModel.Health.Max}");
            Heal(_testHealAmount);
            Debug.Log($"After heal: {_playerModel.Health.Current}/{_playerModel.Health.Max}");
        }
#endif
    }
}