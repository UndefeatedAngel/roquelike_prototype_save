using System;
using UnityEngine;
using BlockoutRoguelike.Core.Person.Models;
using BlockoutRoguelike.Core.UI;
using BlockoutRoguelike.Adapters;
using BlockoutRoguelike.Core.Person.Controllers.PlayerController;
using BlockoutRoguelike.Core.Person.Models.Items;
using BlockoutRoguelike.Inventory.Items;
using BlockoutRoguelike.Inventory.Services;

namespace BlockoutRoguelike.Core.PlayerController
{
    public partial class PlayerController : MonoBehaviour
    {
        private static int _attack = 3;
        public static int Attack => _attack;
        
        [SerializeField] private HUDController _hudController;
        [SerializeField] private bool _playOnAwake = true;
        [SerializeField] private bool _skipInitialHealthAnimation = true;
        [SerializeField] private bool _autoSave = true;
        
        #if UNITY_EDITOR
        [SerializeField] private bool _showTests = false;
        #endif
        
        private PlayerModel _playerModel;
        private PlayerDataManager _dataManager;
        private InventoryService _inventoryService;
        private InventoryToCoreAdapter _inventoryAdapter;

        #if UNITY_EDITOR
        public PlayerModel PlayerModel => _playerModel;
        #endif

        private void Start()
        {
            if (_playOnAwake)
            {
                Setup();
            }
        }

        public void Setup()
        {
            var health = new Health(30, 30);
            var coins = new Coins(0);
            _playerModel = new PlayerModel(health, coins);
            
            // Инициализация инвентаря
            _inventoryService = new InventoryService(
                quickSlotsCount: 3,
                backpackSlotsCount: 20,
                sacrificeSlotsCount: 3,
                benchSlotsCount: 6
            );
            _inventoryAdapter = new InventoryToCoreAdapter(_inventoryService);
            
            // Замена адаптера данных на новый с поддержкой инвентаря
            _dataManager = new PlayerDataManager(_playerModel, new CoreToInventoryAdapter(_inventoryService));
            _dataManager.LoadData();
            
            _hudController.Initialize(_playerModel, _skipInitialHealthAnimation);
            _hudController.InitializeInventory(_inventoryService);
            
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
        
        // Методы для работы с инвентарем
        public bool TryUseInventoryItem(int quickSlotIndex, bool useKeysContainer = true)
        {
            return _inventoryService.TryUseQuickSlot(quickSlotIndex, useKeysContainer);
        }
        
        public bool AddItemToInventory(IInventoryItem item)
        {
            return _inventoryService.TryAddItemToAnyContainer(item);
        }
        
        public InventoryService GetInventoryService() => _inventoryService;
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) SaveData();
        }
        
        private void OnApplicationQuit()
        {
            SaveData();
        }

        public void Restart()
        {
            var healAmount = PlayerModel.Health.Max - PlayerModel.Health.Current;
            Heal(healAmount);
        }
    }
}