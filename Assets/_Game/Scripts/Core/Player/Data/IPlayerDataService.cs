namespace NothingAwaits.Core.Player
{
    public interface IPlayerDataService
    {
        PlayerData Load();
        void Save(PlayerData data);
        void Delete();
    }
}