using System.Collections.Generic;
using BlockoutRoguelike.Battle.Controllers;

namespace BlockoutRoguelike.Cards.Person
{
    public class PlayerStats
    {
        public int Coins;
        private Dictionary<PlayerCardType, int> _stats = new();

        public void AddStat(PlayerCardType type, int count)
        {
            if (!_stats.TryAdd(type, count))
            {
                _stats[type] += count;
            }
        }

        public int GetStat(PlayerCardType type)
        {
            return _stats.GetValueOrDefault(type, 0);
        }
    }
}