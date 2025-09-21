using BlockoutRoguelike.Battle.Cards;
using BlockoutRoguelike.Core.Rune;
using TMPro;
using UnityEngine;

namespace BlockoutRoguelike.Core.UI.Inventory
{
    public class ProfileWindowUI: MonoBehaviour
    {
        [Header("Info Player")]
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;
        [Header("Parametrs")] 
        [SerializeField] private TMP_Text _goldText;
        [SerializeField] private TMP_Text _hpText;
        [Header("Cards")]
        [SerializeField] private GameObject _cardPrefab;
        [SerializeField] private Transform _cardsParent;
        [Header("Rune")] 
        [SerializeField] private RuneUI _runePrefab;
        [SerializeField] private Transform _runeParent;
        [Header("Rewards")]
        [SerializeField] private GameObject _rewardPrefab;
        [SerializeField] private Transform _rewardParent;
        [Header("Stats")]
        [SerializeField] private TMP_Text _allFightsText;
        [SerializeField] private TMP_Text _enemiesDeathText;
        [SerializeField] private TMP_Text _eliteEnemiesDeathText;
        [SerializeField] private TMP_Text _mobAttackDeathText;
        [SerializeField] private TMP_Text _mobProtectionDeathText;
        [SerializeField] private TMP_Text _mobHealthDeathText;
        [SerializeField] private TMP_Text _mobMageDeathText;
        [Header("Boss")]
        [SerializeField] private GameObject _bossPrefab;
        [SerializeField] private Transform _bossParent;

        public void OnEnable()
        {
            UpdateNameAndDescription();
            UpdateHp();
            UpdateStats();
        }

        private void UpdateNameAndDescription()
        {
            _nameText.text = "Потерянный странник";
            _descriptionText.text = "Единственный выживший в этом темном месте...";
        }

        private void UpdateHp()
        {
            _hpText.text = $"{Player.HP}/{Player.MaxHp}";
        }

        private void UpdateStats()
        {
            var stats = new Stats();
            stats.LoadStats();
            _allFightsText.text = stats.AllFights.ToString();
            _enemiesDeathText.text = stats.EnemiesDeath.ToString();
            _eliteEnemiesDeathText.text = stats.EliteEnemiesDeath.ToString();
            _mobAttackDeathText.text = stats.MobAttackDeath.ToString();
            _mobProtectionDeathText.text = stats.MobProtectionDeath.ToString();
            _mobHealthDeathText.text = stats.MobHealthDeath.ToString();
            _mobMageDeathText.text = stats.MobMageDeath.ToString();
        }
    }
}