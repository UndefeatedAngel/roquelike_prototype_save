namespace BlockoutRoguelike.Core.PlayerController
{
    public interface IPlayerDataService
    {
        PlayerData Load();
        void Save(PlayerData data);
        void Delete();
    }

    public struct PlayerData
    {
        public int health;
        public int maxHealth;
        public int coins;
    }
}