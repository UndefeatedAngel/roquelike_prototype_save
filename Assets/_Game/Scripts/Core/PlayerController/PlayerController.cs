using UnityEngine;
using BlockoutRoguelike.Core.Person.Models;
using BlockoutRoguelike.Core.UI;
using BlockoutRoguelike.Adapters;
using BlockoutRoguelike.Core.Person.Controllers.PlayerController;
using BlockoutRoguelike.Core.Person.Models.Items;

namespace BlockoutRoguelike.Core.PlayerController
{
    public partial class PlayerController : MonoBehaviour
    {
        [SerializeField] private HUDController _hudController;
        [SerializeField] private bool _skipInitialHealthAnimation = true;
        [SerializeField] private bool _autoSave = true;
        
        #if UNITY_EDITOR
        [SerializeField] private bool _showTests = false;
        #endif
        
        private PlayerModel _playerModel;
        private PlayerDataManager _dataManager;

        #if UNITY_EDITOR
        public PlayerModel PlayerModel => _playerModel;
        #endif

        private void Start()
        {
            var health = new Health(100, 100);
            var coins = new Coins(0);
            _playerModel = new PlayerModel(health, coins);
            
            _dataManager = new PlayerDataManager(_playerModel, new CoreDataServiceAdapter());
            _dataManager.LoadData();
            
            _hudController.Initialize(_playerModel, _skipInitialHealthAnimation);
            
            if (_autoSave)
            {
                _playerModel.Health.OnChanged += OnHealthChanged;
                _playerModel.Coins.OnChanged += OnCoinsChanged;
            }
        }
        
        private void OnHealthChanged(int health) => SaveData();
        private void OnCoinsChanged(int coins) => SaveData();
        
        public void SaveData() => _dataManager?.SaveData();
        
        public void TakeDamage(int damage) => _playerModel.Health.Decrease(damage);
        public void Heal(int amount) => _playerModel.Health.Increase(amount);

        public bool IsDead() => _playerModel.Health.IsDead;
        
        public void AddCoins(int amount) => _playerModel.Coins.Increase(amount);
        
        public void SpendCoins(int amount) => _playerModel.Coins.Decrease(amount);
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) SaveData();
        }
        
        private void OnApplicationQuit()
        {
            SaveData();
        }
    }
}