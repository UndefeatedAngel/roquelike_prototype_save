using BlockoutRoguelike.Core.Actor.Models;
using BlockoutRoguelike.DataStorage;

namespace BlockoutRoguelike.Core.PlayerController
{
    public class PlayerDataManager : IPlayerDataService
    {
        private const string SaveKey = "roguelike.player";
        private const string HealthKey = "health";
        private const string MaxHealthKey = "maxHealth";
        private const string CoinsKey = "coins";

        private readonly PlayerModel _playerModel;
        private readonly IDataStorage _storage;

        public PlayerDataManager(PlayerModel playerModel)
        {
            _playerModel = playerModel;
            _storage = StorageManager.Storage;
        }

        public PlayerData Load()
        {
            var data = new PlayerData
            {
                health = _storage.LoadInt($"{SaveKey}_{HealthKey}", -1),    // ← Исправлено на LoadInt
                maxHealth = _storage.LoadInt($"{SaveKey}_{MaxHealthKey}", -1), // ← Исправлено на LoadInt
                coins = _storage.LoadInt($"{SaveKey}_{CoinsKey}", -1)          // ← Исправлено на LoadInt
            };

            if (data.health != -1)
            {
                int healthDifference = data.health - _playerModel.Health.Current;
                if (healthDifference > 0)
                    _playerModel.Health.Heal(healthDifference);
                else if (healthDifference < 0)
                    _playerModel.Health.TakeDamage(-healthDifference);
            }

            if (data.coins != -1)
            {
                int coinsDifference = data.coins - _playerModel.Coins.Amount;
                if (coinsDifference > 0)
                    _playerModel.Coins.Add(coinsDifference);
            }

            return data;
        }

        public void Save(PlayerData data)
        {
            _storage.SaveInt($"{SaveKey}_{HealthKey}", data.health);        // ← Исправлено на SaveInt
            _storage.SaveInt($"{SaveKey}_{MaxHealthKey}", data.maxHealth);   // ← Исправлено на SaveInt
            _storage.SaveInt($"{SaveKey}_{CoinsKey}", data.coins);           // ← Исправлено на SaveInt
        }

        public void Save()
        {
            Save(new PlayerData
            {
                health = _playerModel.Health.Current,
                maxHealth = _playerModel.Health.Max,
                coins = _playerModel.Coins.Amount
            });
        }

        public void Delete()
        {
            _storage.DeleteKey($"{SaveKey}_{HealthKey}");
            _storage.DeleteKey($"{SaveKey}_{MaxHealthKey}");
            _storage.DeleteKey($"{SaveKey}_{CoinsKey}");
        }
    }
}