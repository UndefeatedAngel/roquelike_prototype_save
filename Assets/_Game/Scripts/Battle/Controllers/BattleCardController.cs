using System;
using System.Collections;
using System.Collections.Generic;
using BlockoutRoguelike.Battle.Cards;
using BlockoutRoguelike.Cards;
using BlockoutRoguelike.Cards.Mobs;
using BlockoutRoguelike.Cards.Person;
using BlockoutRoguelike.Cards.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BlockoutRoguelike.Battle.Controllers
{
    public class BattleCardController: MonoBehaviour
    {
        [SerializeField] private HorizontalLayoutGroup _cardPlayerHorizontalLayoutGroup;
        [Header("Player")]
        [SerializeField] private GameObject[] _playerMobPrefabs;
        [SerializeField] private List<Slot> _playerSlots;
        [Header("Enemy")]
        [SerializeField] private GameObject[] _enemyMobPrefabs;
        [SerializeField] private List<Slot> _enemySlots;
        
        [Header("Controllers")]
        [SerializeField] private PlayerDeckController _playerDeckController;
        [SerializeField] private EnemyDeckController _enemyDeckController;
        [SerializeField] private InputController _inputController;
        [SerializeField] private StateMachineController _stateMachineController;
        [Header("UI")] 
        [SerializeField] private MainUI _mainUI;

        private float _timeAnimation = 0.5f;
        
        private PlayerStats _playerStats = new();

        private IEnumerator Start()
        {
            InitPlayerStats();
            _playerDeckController.Init();
            _enemyDeckController.Init();
            _enemyDeckController.OnDeath = type =>
            {
                _playerStats.AddStat(type, 1);
                _mainUI.UpdateStat(type, _playerStats.GetStat(type));
            };
            _enemyDeckController.OnDropCoin = rectTransform =>
            {
                _mainUI.FlyCoin(rectTransform, () =>
                {
                    _playerStats.Coins += 1;
                    _mainUI.UpdateCoins(_playerStats.Coins);
                    DestroyImmediate(rectTransform.GetComponentInParent<RectTransform>().gameObject);
                });
            };
            
            yield return new WaitForSeconds(0.5f);
            FlyCardToPlayer();
            yield return new WaitForSeconds(_timeAnimation);
            FlyCardToPlayer();
            yield return new WaitForSeconds(_timeAnimation);
            FlyCardToPlayer();
            yield return new WaitForSeconds(_timeAnimation);
            _playerDeckController.OnCardClick += cardUI =>
            {
                FlyCardToPlayer();
            };

            SetupSlots();
            _stateMachineController.Setup(moveType =>
            {
                if (moveType == 1)
                {
                    StartCoroutine(EnemyDeckMove());
                }
            });
        }

        private IEnumerator EnemyDeckMove()
        {
            yield return new WaitForSeconds(1f);
            _enemyDeckController.Move();
            _stateMachineController.EndMove();
        }
        
        private void FlyCardToPlayer()
        {
            var card = _playerDeckController.GetCard();
            card.DisableDeckAnimated(_timeAnimation);
            card.GetComponent<DragAndHoverUI>().enabled = false;
            card.transform.DOJump(_cardPlayerHorizontalLayoutGroup.transform.position, 50, 1 , _timeAnimation).OnComplete(() =>
            {
                card.transform.parent = _cardPlayerHorizontalLayoutGroup.transform;
                card.GetComponent<DragAndHoverUI>().enabled = true;
                card.GetComponent<DragAndHoverUI>().Setup();
                card.transform.localScale = Vector3.one;
            });
        }

        private void SetupSlots()
        {
            _inputController.OnCardToSlot = AddCardToSlot;
        }

        private void AddCardToSlot(CardUI cardUI, Slot slot)
        {
            if (slot.Team != 0) return;
            if (slot.ActiveMob != null) return;

            var prefabMob = _playerMobPrefabs[cardUI.NumberCard];
            var mob = Instantiate(prefabMob, slot.transform);
            var mobUI = mob.GetComponent<MobUI>();
            mobUI.SetMob("PlayerMob", cardUI.CardData);
            slot.ActiveMob = mobUI;
            DestroyImmediate(cardUI.GetComponentInParent<RectTransform>().gameObject);
            StartCoroutine(EnableLayoutGroup());
        }
        
        private IEnumerator EnableLayoutGroup()
        {
            yield return new WaitForEndOfFrame();
            if (_cardPlayerHorizontalLayoutGroup != null)
            {
                _cardPlayerHorizontalLayoutGroup.enabled = true;
            }
        }

        private void InitPlayerStats()
        {
            _mainUI.UpdateCoins(_playerStats.Coins);
            _mainUI.UpdateBlood(3,3);
            var stats = new[]
            {
                PlayerCardType.Attack, PlayerCardType.Protection, PlayerCardType.Health, PlayerCardType.Mage
            };
            foreach (var stat in stats)
            {
                _mainUI.UpdateStat(stat, _playerStats.GetStat(stat));
            }
        }
    }
}