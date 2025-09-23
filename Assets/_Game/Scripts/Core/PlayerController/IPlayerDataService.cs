namespace BlockoutRoguelike.Core
{
    public interface IPlayerDataService
    {
        void SavePlayerData(int health, int maxHealth, int coins);
        (int health, int maxHealth, int coins) LoadPlayerData();
        void DeletePlayerData();
    }
}