using BlockoutRoguelike.Core;
using BlockoutRoguelike.Core.Person.Models;
using BlockoutRoguelike.DataStorage;
using BlockoutRoguelike.Inventory.Services;

namespace BlockoutRoguelike.Adapters
{
    public class CoreToInventoryAdapter : IPlayerDataService
    {
        private InventoryService _inventoryService;
        private InventorySaveService _inventorySaveService;
        
        public CoreToInventoryAdapter(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
            _inventorySaveService = new InventorySaveService();
        }
        
        // Реализация интерфейса IPlayerDataService для совместимости с Core
        public void SavePlayerData(int health, int maxHealth, int coins)
        {
            // Сохраняем данные Core + инвентарь
            _inventorySaveService.SaveInventory(_inventoryService);
            
            // Дополнительно сохраняем базовые данные игрока
            var storage = StorageManager.Storage;
            storage.SaveInt("player_health", health);
            storage.SaveInt("player_maxHealth", maxHealth);
            storage.SaveInt("player_coins", coins);
        }
        
        public (int health, int maxHealth, int coins) LoadPlayerData()
        {
            // Загружаем инвентарь
            _inventorySaveService.LoadInventory(_inventoryService);
            
            // Загружаем базовые данные игрока
            var storage = StorageManager.Storage;
            int health = storage.LoadInt("player_health", 100);
            int maxHealth = storage.LoadInt("player_maxHealth", 100);
            int coins = storage.LoadInt("player_coins", 0);
            
            return (health, maxHealth, coins);
        }
        
        public void DeletePlayerData()
        {
            _inventorySaveService.DeleteSaveData();
            
            // Удаляем базовые данные игрока
            var storage = StorageManager.Storage;
            storage.DeleteKey("player_health");
            storage.DeleteKey("player_maxHealth");
            storage.DeleteKey("player_coins");
        }
        
        // Дополнительные методы для интеграции
        public void InitializeWithPlayerData(PlayerModel playerModel)
        {
            // Инициализация инвентаря на основе данных игрока
            // Например, начальные предметы в зависимости от прогресса
        }
        
        public void UpdateInventoryFromPlayerState(PlayerModel playerModel)
        {
            // Синхронизация состояния инвентаря с состоянием игрока
            // Например, обновление количества монет
        }
    }
}