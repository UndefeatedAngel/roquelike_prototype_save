using System.Collections.Generic;

namespace NothingAwaits.Core.Player
{
    [System.Serializable]
    public class PlayerData
    {
        public int health;
        public int maxHealth;
        public int coins;
        public List<string> cardIds;
    }
}