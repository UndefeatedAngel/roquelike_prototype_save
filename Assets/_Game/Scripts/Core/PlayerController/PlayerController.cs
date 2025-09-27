using UnityEngine;
using NothingAwaits.Core.Actor.Models;
using NothingAwaits.Core.Actor.Models.Items;
using NothingAwaits.Core.UI;

namespace NothingAwaits.Core.PlayerController
{
    public partial class PlayerController : MonoBehaviour
    {
        [SerializeField] private HUDController _hudController;
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private bool _playOnStart = true;
        [SerializeField] private bool _skipInitialHealthAnimation = true;
        [SerializeField] private bool _autoSave = true;
        
        #if UNITY_EDITOR
        [SerializeField] private bool _showTests = false;
        public PlayerModel PlayerModel => _playerModel;
        #endif

        private PlayerModel _playerModel;
        private PlayerDataManager _dataManager;

        private void Start()
        {
            if (_playOnStart) Setup();
        }

        public void Setup()
        {
            var health = new Health(_playerConfig.StartHealth, _playerConfig.MaxHealth);
            var coins = new Coins(_playerConfig.StartCoins);
            _playerModel = new PlayerModel(health, coins);
            _dataManager = new PlayerDataManager(_playerModel);
            _dataManager.Load();
            
            _hudController.Initialize(_playerModel, _skipInitialHealthAnimation);
            
            if (_autoSave)
            {
                _playerModel.Health.OnChanged += OnHealthChanged;
                _playerModel.Coins.OnChanged += OnCoinsChanged;
            }
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

        public void Reset()
        {
            //Setup();
        }
    }
}