using System.Collections.Generic;
using NothingAwaits.Core.Cards;
using NothingAwaits.Core.Cards.Data;
using UnityEngine;
using NothingAwaits.Core.Coins;
using NothingAwaits.Core.Health;
using NothingAwaits.Core.UI;

namespace NothingAwaits.Core.Player
{
    public partial class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HUDController _hudController;
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private CardsContainer _cardsContainer;
        [Space]
        [Header("Setup")]
        [SerializeField] private bool _playOnStart = true;
        [SerializeField] private bool _skipInitialHealthAnimation = true;
        [SerializeField] private bool _autoSave = true;
        
        #if UNITY_EDITOR
        [SerializeField] private bool _showTests = false;
        public PlayerModel PlayerModel => _playerModel;
        #endif

        private PlayerModel _playerModel;
        private PlayerDataManager _dataManager;

        public IReadOnlyList<CardModel> GetCards() => _playerModel.Cards;
        
        private void Start()
        {
            if (_playOnStart) Setup();
        }

        public void Setup()
        {
            var health = new HealthModel(_playerConfig.StartHealth, _playerConfig.MaxHealth);
            var coins = new CoinsModel(_playerConfig.StartCoins);
            
            _playerModel = new PlayerModel(health, coins, new List<CardConfig>(_playerConfig.BaseCards));
            _dataManager = new PlayerDataManager(_playerModel, _playerConfig.BaseCards, _cardsContainer);
            _dataManager.Load();
            
            _hudController.Initialize(_playerModel, _skipInitialHealthAnimation);

            if (!_autoSave) return;
            
            _playerModel.Health.OnChanged += OnHealthChanged;
            _playerModel.Coins.OnChanged += OnCoinsChanged;
        }

        private void OnHealthChanged(int health) => SaveData();
        private void OnCoinsChanged(int coins) => SaveData();
        
        public void SaveData() => _dataManager?.Save();
        
        public void TakeDamage(int damage) => _playerModel.Health.TakeDamage(damage);
        public void Heal(int amount) => _playerModel.Health.Heal(amount);
        public void FullHeal() => _playerModel.Health.FullHeal();
        public bool IsDead() => _playerModel.Health.IsDead;
        
        public void AddCoins(int amount) => _playerModel.Coins.Add(amount);
        public bool TrySpendCoins(int amount) => _playerModel.Coins.TrySpend(amount);

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) SaveData();
        }
        
        private void OnApplicationQuit() => SaveData();

        #region Cards
        public void AddCard(CardConfig cardConfig)
        {
            _playerModel.AddCard(cardConfig);
            if (_autoSave) SaveData();
        }

        public void AddCard(CardModel cardModel)
        {
            _playerModel.AddCard(cardModel);
            if (_autoSave) SaveData();
        }

        public void RemoveCard(CardConfig cardConfig)
        {
            _playerModel.RemoveCard(cardConfig);
            if (_autoSave) SaveData();
        }

        public void RemoveCard(CardModel cardModel)
        {
            _playerModel.RemoveCard(cardModel);
            if (_autoSave) SaveData();
        }
        
        public List<CardModel> GetPlayerCards()
        {
            return _playerModel?.Cards ?? new List<CardModel>();
        }
        #endregion

        public void Reset()
        {
            
        }
    }
}