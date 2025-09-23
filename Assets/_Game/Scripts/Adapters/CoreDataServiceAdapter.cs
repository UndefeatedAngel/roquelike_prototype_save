using BlockoutRoguelike.Core;
using BlockoutRoguelike.DataStorage;

namespace BlockoutRoguelike.Adapters
{
    public class CoreDataServiceAdapter : IPlayerDataService
    {
        private const string SaveKey = "roguelike.player";
        
        private const string PlayerHealth = "health";
        private const string PlayerMaxHealth = "maxHealth";
        private const string PlayerCoins = "coins";
        
        public void SavePlayerData(int health, int maxHealth, int coins)
        {
            var storage = StorageManager.Storage;
            storage.SaveInt($"{SaveKey}_{PlayerHealth}", health);
            storage.SaveInt($"{SaveKey}_{PlayerMaxHealth}", maxHealth);
            storage.SaveInt($"{SaveKey}_{PlayerCoins}", coins);
        }
        
        public (int health, int maxHealth, int coins) LoadPlayerData()
        {
            var storage = StorageManager.Storage;
            
            if (storage.HasKey($"{SaveKey}_health"))
            {
                int health = storage.LoadInt($"{SaveKey}_{PlayerHealth}", 100);
                int maxHealth = storage.LoadInt($"{SaveKey}_{PlayerMaxHealth}", 100);
                int coins = storage.LoadInt($"{SaveKey}_{PlayerCoins}", 0);
                return (health, maxHealth, coins);
            }
            
            return (100, 100, 0);
        }
        
        public void DeletePlayerData()
        {
            var storage = StorageManager.Storage;
            storage.DeleteKey($"{SaveKey}_{PlayerHealth}");
            storage.DeleteKey($"{SaveKey}_{PlayerMaxHealth}");
            storage.DeleteKey($"{SaveKey}_{PlayerCoins}");
        }
    }
}