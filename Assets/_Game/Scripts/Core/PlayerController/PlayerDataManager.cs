using BlockoutRoguelike.Core.Person.Models;

namespace BlockoutRoguelike.Core.Person.Controllers.PlayerController
{
    public class PlayerDataManager
    {
        private readonly PlayerModel _playerModel;
        private readonly IPlayerDataService _playerDataService;
        
        public PlayerDataManager(PlayerModel playerModel, IPlayerDataService playerDataService)
        {
            _playerModel = playerModel;
            _playerDataService = playerDataService;
        }
        
        public void SaveData()
        {
            _playerDataService.SavePlayerData(_playerModel.Health.Current, _playerModel.Health.Max, _playerModel.Coins.Current);
        }
        
        public void LoadData()
        {
            var (health, maxHealth, coins) = _playerDataService.LoadPlayerData();
            
            _playerModel.Health.UpdateMax(maxHealth);
            _playerModel.Health.Increase(health - _playerModel.Health.Current);
            _playerModel.Coins.Increase(coins - _playerModel.Coins.Current);
        }
    }
}