using System;
using NothingAwaits.Battle.Controllers;
using UnityEngine;

namespace NothingAwaits.Core
{
    [Serializable]
    public class Stats
    {
        private string _keySaveStats = "roguelike.stats";
        public int AllFights;
        public int EnemiesDeath;
        public int EliteEnemiesDeath;
        public int MobAttackDeath;
        public int MobProtectionDeath;
        public int MobHealthDeath;
        public int MobMageDeath;

        public void LoadStats()
        {
            var json = PlayerPrefs.GetString(_keySaveStats);
            if (json == null || string.IsNullOrEmpty(json))
            {
                SaveStats();
                json = PlayerPrefs.GetString(_keySaveStats);
            }
            
            var stats = JsonUtility.FromJson<Stats>(json);
            AllFights = stats.AllFights;
            EnemiesDeath = stats.EnemiesDeath;
            EliteEnemiesDeath = stats.EliteEnemiesDeath;
            MobAttackDeath = stats.MobAttackDeath;
            MobProtectionDeath = stats.MobProtectionDeath;
            MobHealthDeath = stats.MobHealthDeath;
            MobMageDeath = stats.MobMageDeath;
        }

        public void SaveStats()
        {
            var json = JsonUtility.ToJson(this);
            PlayerPrefs.SetString(_keySaveStats, json);
            PlayerPrefs.Save();
        }

        public void ClearStats()
        {
            PlayerPrefs.SetString(_keySaveStats, JsonUtility.ToJson(new Stats()));
        }

        public void AddFight()
        {
            AllFights++;
            SaveStats();
        }

        public void AddEnemiesDeath()
        {
            EnemiesDeath++;
            SaveStats();
        }

        public void AddEliteEnemiesDeath()
        {
            EliteEnemiesDeath++;
            SaveStats();
        }

        public void MobEnemyDeath(PlayerCardType type)
        {
            _ = type switch
            {
                PlayerCardType.Attack => MobAttackDeath++,
                PlayerCardType.Protection => MobProtectionDeath++,
                PlayerCardType.Health => MobHealthDeath++,
                PlayerCardType.Mage => MobMageDeath++,
                _ => throw new ArgumentException($"Unknown card type: {type}")
            };

            SaveStats();
        }
    }
}