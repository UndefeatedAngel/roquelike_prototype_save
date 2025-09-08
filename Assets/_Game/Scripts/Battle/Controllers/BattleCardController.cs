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
using UnityEngine.ProBuilder;
using UnityEngine.SceneManagement;
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
        [Header("Sacrifices")]
        [SerializeField] private Sacrifice[] _sacrifices;
        
        [Header("Controllers")]
        [SerializeField] private PlayerDeckController _playerDeckController;
        [SerializeField] private EnemyDeckController _enemyDeckController;
        [SerializeField] private InputController _inputController;
        [SerializeField] private StateMachineController _stateMachineController;
        [SerializeField] private UIController _uiController;
        

        private float _timeAnimation = 0.5f;
        
        private PlayerStats _playerStats = new();
        
        private List<CardUI> _cardsInDeck = new();

        private int _currentCardAdd;
        private int _maxCardAdd = 1;

        private IEnumerator Start()
        {
            InitPlayerStats();
            InitBloods();
            _playerDeckController.Init(_playerStats);
            _enemyDeckController.Init();
            _enemyDeckController.OnDeath = type =>
            {
                _playerStats.AddStat(type, 1);
                _uiController.UpdateStat(type, _playerStats.GetStat(type), _enemyDeckController.GetMaxStat(type));
                NextMoveEnemy();
            };
            _enemyDeckController.OnDropCoin = rectTransform =>
            {
                _uiController.FlyCoin(rectTransform, () =>
                {
                    _playerStats.Coins += 1;
                    _uiController.UpdateCoins(_playerStats.Coins);
                    DestroyImmediate(rectTransform.GetComponentInParent<RectTransform>().gameObject);
                });
            };
            
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(AddCardsToDeck());
            _playerDeckController.OnCardClick += cardUI =>
            {
                if (_currentCardAdd < _maxCardAdd)
                {
                    _currentCardAdd++;
                    FlyCardToPlayer();
                }
            };

            SetupSlots();
            _stateMachineController.OnWin = WinGame;
            _stateMachineController.OnLose = LoseGame;
            _stateMachineController.Setup(moveType =>
            {
                if (moveType == QueueMove.Enemy)
                {
                    _uiController.HideMoveEnemy();
                    StartCoroutine(EnemyDeckMove());
                }
                else
                {
                    NextMoveEnemy();
                    StartCoroutine(AddCardsToDeck());
                }
            });

            NextMoveEnemy();
        }

        private void NextMoveEnemy()
        {
            var nextSlot = _enemyDeckController.GetNextSlot()+1;
            var nextEnemy = _enemyDeckController.GetNextEnemy();
            if (nextEnemy != PlayerCardType.Empty)
            {
                _uiController.ShowMoveEnemy($"S{nextSlot}-{nextEnemy.ToString()}");
            }
        }

        private IEnumerator AddCardsToDeck()
        {
            if (_cardsInDeck.Count < 3)
            {
                var count = 3-_cardsInDeck.Count;
                for (int i = 0; i < count; i++)
                {
                    FlyCardToPlayer();
                    yield return new WaitForSeconds(_timeAnimation);
                }
            }
            _currentCardAdd = 0;
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
            card.GetComponent<DragAndHoverUI>().UpdateInput(false);
            card.transform.DOJump(_cardPlayerHorizontalLayoutGroup.transform.position, 50, 1 , _timeAnimation).OnComplete(() =>
            {
                card.transform.parent = _cardPlayerHorizontalLayoutGroup.transform;
                card.GetComponent<DragAndHoverUI>().enabled = true;
                card.GetComponent<DragAndHoverUI>().Setup();
                card.GetComponent<DragAndHoverUI>().UpdateInput(card.BloodCost()<=_playerStats.BloodCurrent);
                card.transform.localScale = Vector3.one;
                _cardsInDeck.Add(card);
                foreach (var cardDeck in _cardsInDeck)
                {
                    cardDeck.GetComponent<DragAndHoverUI>().UpdateInput(card.BloodCost()<=_playerStats.BloodCurrent);
                }
            });
        }

        private void SetupSlots()
        {
            _inputController.OnCardToSlot = AddCardToSlot;
        }

        private void AddCardToSlot(CardUI cardUI, Slot slot)
        {
            if (cardUI.CardData.Blood > _playerStats.BloodCurrent) return;
            if (slot.Team != 0) return;
            if (slot.ActiveMob != null) return;

            var prefabMob = _playerMobPrefabs[cardUI.NumberCard];
            var mob = Instantiate(prefabMob, slot.transform);
            var mobUI = mob.GetComponent<MobUI>();
            mobUI.SetMob($"PU: {cardUI.CardData.GetName()}", cardUI.CardData);
            slot.ActiveMob = mobUI;
            _cardsInDeck.Remove(cardUI);
            RemoveBlood(cardUI);
            DestroyImmediate(cardUI.GetComponentInParent<RectTransform>().gameObject);
            StartCoroutine(EnableLayoutGroup());
        }

        private void RemoveBlood(CardUI cardUI)
        {
            _playerStats.AddBlood(-cardUI.BloodCost());
            _uiController.UpdateBlood(_playerStats.BloodCurrent, _playerStats.BloodMax);
            foreach (var card in _cardsInDeck)
            {
                card.GetComponent<DragAndHoverUI>().UpdateInput(card.BloodCost()<=_playerStats.BloodCurrent);
            }
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
            _uiController.UpdateCoins(_playerStats.Coins);
            var stats = new[]
            {
                PlayerCardType.Attack, PlayerCardType.Protection, PlayerCardType.Health, PlayerCardType.Mage
            };
            foreach (var stat in stats)
            {
                _uiController.UpdateStat(stat, _playerStats.GetStat(stat), _enemyDeckController.GetMaxStat(stat));
            }
        }

        private void InitBloods()
        {
            _uiController.UpdateBlood(_playerStats.BloodCurrent, _playerStats.BloodMax);
            foreach (var sacrifice in _sacrifices)
            {
                sacrifice.Set(_playerStats);
                sacrifice.OnGetBlood = () =>
                {
                    _playerStats.AddBlood(1);
                    foreach (var card in _cardsInDeck)
                    {
                        card.GetComponent<DragAndHoverUI>().UpdateInput(card.BloodCost()<=_playerStats.BloodCurrent);
                    }
                    _uiController.UpdateBlood(_playerStats.BloodCurrent, _playerStats.BloodMax);
                };
            }
        }

        private void WinGame()
        {
            _uiController.ShowWin(() =>
            {
                SceneManager.LoadScene("GamePlay");
            });
        }

        private void LoseGame()
        {
            _uiController.ShowLose(() =>
            {
                PlayerPrefs.SetInt("Restart", 1);
                PlayerPrefs.Save();
                SceneManager.LoadScene("GamePlay");
            });
        }
    }
}