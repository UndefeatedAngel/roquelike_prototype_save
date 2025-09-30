using _Game.Scripts.OldAchitecture.Modules;
using NothingAwaits.Core.Cards.Data;
using NothingAwaits.Core.Rune;
using TMPro;
using UnityEngine;

namespace NothingAwaits.Core.UI.Lobby.Windows
{
    public class ProfileWindowUI: BaseLobbyWindow
    {
        [Header("References")]
        [SerializeField] private ModelRotatorActivate _modelRotatorActivate;
        [Space]
        [Header("Info Player")]
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;
        [Space]
        [Header("Cards")]
        [SerializeField] private GameObject _cardPrefab;
        [SerializeField] private Transform _cardsParent;
        [Space]
        [Header("Rune")] 
        [SerializeField] private RuneUI _runePrefab;
        [SerializeField] private Transform _runeParent;
        [Space]
        [Header("Rewards")]
        [SerializeField] private GameObject _rewardPrefab;
        [SerializeField] private Transform _rewardParent;
        [Space]
        [Header("Stats")]
        [SerializeField] private TMP_Text _allFightsText;
        [SerializeField] private TMP_Text _enemiesDeathText;
        [SerializeField] private TMP_Text _eliteEnemiesDeathText;
        [SerializeField] private TMP_Text _mobAttackDeathText;
        [SerializeField] private TMP_Text _mobProtectionDeathText;
        [SerializeField] private TMP_Text _mobHealthDeathText;
        [SerializeField] private TMP_Text _mobMageDeathText;
        [Space]
        [Header("Boss")]
        [SerializeField] private GameObject _bossPrefab;
        [SerializeField] private Transform _bossParent;

        public void OnEnable()
        {
            _modelRotatorActivate.Activate(CardType.Empty);
            UpdateNameAndDescription();
            UpdateStats();
        }

        private void UpdateNameAndDescription()
        {
            _nameText.text = "Потерянный странник";
            _descriptionText.text = "Единственный выживший в этом темном месте...";
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