using NothingAwaits.Core.Cards.Data;
using UnityEngine;

namespace NothingAwaits.Core.Player
{
    [CreateAssetMenu(fileName = "New PlayerConfig", menuName = "NothingAwaits/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Health Settings")]
        [SerializeField] private int _startHealth = 100;
        [SerializeField] private int _maxHealth = 100;
        [Space]
        [Header("Economy Settings")] 
        [SerializeField] private int _startCoins = 0;
        [Space]
        [Header("Base Cards")] 
        [SerializeField] private CardConfig[] _baseCards;

        public int StartHealth => _startHealth;
        public int MaxHealth => _maxHealth;
        public int StartCoins => _startCoins;
        public CardConfig[] BaseCards => _baseCards;
        
    }
}