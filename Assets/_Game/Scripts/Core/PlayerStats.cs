using System.Collections.Generic;
using BlockoutRoguelike.Battle.Controllers;

namespace BlockoutRoguelike.Core
{
    public class PlayerStats
    {
        public int Coins;
        private Dictionary<PlayerCardType, int> _stats = new();
        private int _bloodCurrent = 3;
        private int _bloodMax = 3;
        public int BloodCurrent => _bloodCurrent;
        public int BloodMax => _bloodMax;

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

        public bool IsNeedBlood()
        {
            return BloodCurrent < BloodMax;
        }

        public void AddBlood(int count)
        {
            _bloodCurrent += count;
            if (_bloodCurrent < 0)
            {
                _bloodCurrent = 0;
            }
        }
    }
}