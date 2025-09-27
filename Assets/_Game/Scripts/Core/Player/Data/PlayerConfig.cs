using UnityEngine;

namespace NothingAwaits.Core.Player
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Roguelike/Player Config")]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Health Settings")]
        [SerializeField] private int _startHealth = 100;
        [SerializeField] private int _maxHealth = 100;
        
        [Header("Economy Settings")] 
        [SerializeField] private int _startCoins = 0;

        public int StartHealth => _startHealth;
        public int MaxHealth => _maxHealth;
        public int StartCoins => _startCoins;
    }
}