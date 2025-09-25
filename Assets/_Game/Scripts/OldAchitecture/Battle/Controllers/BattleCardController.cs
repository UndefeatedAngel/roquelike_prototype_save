using System;
using System.Collections;
using System.Collections.Generic;
using BlockoutRoguelike.Battle.Cards;
using BlockoutRoguelike.Cards;
using BlockoutRoguelike.Cards.Mobs;
using BlockoutRoguelike.Core;
using BlockoutRoguelike.Core.PlayerController;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Math = System.Math;
using Random = UnityEngine.Random;

namespace BlockoutRoguelike.Battle.Controllers
{
    public class BattleCardController: MonoBehaviour
    {
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private Transform _pointStartMob;
        [SerializeField] private HorizontalLayoutGroup _cardPlayerHorizontalLayoutGroup;
        [Header("Player")]
        [SerializeField] private GameObject[] _playerMobPrefabs;
        [SerializeField] private List<Slot> _playerSlots;
        [Header("Enemy")]
        [SerializeField] private GameObject[] _enemyMobPrefabs;
        [SerializeField] private List<Slot> _enemySlots;
        [Header("Sacrifices")]
        [SerializeField] private Sacrifice[] _sacrifices;
        
        [Header("UI")]
        [SerializeField] private CanvasGroup _noAmountBloodGroup;
        
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
        
        private Stats _stats;
        
        private Tweener _noAmountBloodTween;
        private int _maxGames = 2;


        private IEnumerator Start()
        {
            InitStats();
            InitPlayerStats();
            InitBloods();
            _playerDeckController.Init(_playerStats);
            _enemyDeckController.Init();
            _enemyDeckController.OnDeath = type =>
            {
                _stats.MobEnemyDeath(type);
                _playerStats.AddStat(type, 1);
                _uiController.UpdateStat(type, _playerStats.GetStat(type), _enemyDeckController.GetMaxStat(type));
                //NextMoveEnemy();
            };
            _enemyDeckController.OnDropCoin = rectTransform =>
            {
                _uiController.FlyCoin(rectTransform, () =>
                {
                    _playerController.AddCoins(1);
                    DestroyImmediate(rectTransform.GetComponentInParent<RectTransform>().gameObject);
                });
            };
            
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(AddCardsToDeck());
            _playerDeckController.OnCardDeckClick += cardUI =>
            {
                if (_currentCardAdd < _maxCardAdd)
                {
                    _currentCardAdd++;
                    FlyCardToPlayer();
                }
            };

            _playerDeckController.OnCardSelect += cardUI =>
            {
                if (cardUI.BloodCost() > _playerStats.BloodCurrent)
                {
                    _noAmountBloodTween?.Kill();
                    _noAmountBloodGroup.alpha = 0;
                    _noAmountBloodTween = _noAmountBloodGroup.DOFade(1f, 1f).SetEase(Ease.OutExpo).OnComplete(() =>
                    {
                        _noAmountBloodTween = _noAmountBloodGroup.DOFade(0f, 1f).OnComplete(() =>
                        {

                        });
                    });
                }
            }; 

            SetupSlots();
            _stateMachineController.OnWin = WinGame;
            _stateMachineController.OnLose = LoseGame;
            _stateMachineController.Setup(moveType =>
            {
                if (moveType == TeamType.Enemy)
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
            var nextEnemy = _enemyDeckController.GetNextEnemyData();
            if (nextEnemy.CardType != PlayerCardType.Empty)
            {
                _uiController.ShowMoveEnemy(nextSlot-1, nextEnemy);
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
        
        private void FlyCardToPlayer(CardUI cardUI = null)
        {
            var card = cardUI != null ? cardUI : _playerDeckController.GetCard();
            card.DisableDeckAnimated(_timeAnimation);
            card.GetComponent<DragAndHoverUI>().enabled = false;
            card.GetComponent<DragAndHoverUI>().UpdateInput(false);
            card.transform.DOJump(_cardPlayerHorizontalLayoutGroup.transform.position, 50, 1 , _timeAnimation).OnComplete(() =>
            {
                card.transform.SetParent(_cardPlayerHorizontalLayoutGroup.transform);
                card.GetComponent<DragAndHoverUI>().Setup();
                card.transform.localScale = Vector3.one;
                _cardsInDeck.Add(card);
                UpdateDragState();
            });
        }

        private void SetupSlots()
        {
            _inputController.OnCardToSlot = AddCardToSlot;
        }

        private void AddCardToSlot(CardUI cardUI, Slot slot)
        {
            if (cardUI.CardData.Blood > _playerStats.BloodCurrent)
            {
                return;
            }
            if (slot.Team != 0) return;
            if (slot.ActiveMob != null) return;

            var prefabMob = _playerMobPrefabs[cardUI.NumberCard];
            var mob = Instantiate(prefabMob, slot.transform);
            var mobUI = mob.GetComponent<MobUI>();
            mobUI.SetMob(TeamType.Player,$"PU: {cardUI.CardData.GetName()}", cardUI.CardData, _pointStartMob, cardUI.GetCardImage());
            mobUI.OnGetBlood = AddBlood;
            slot.ActiveMob = mobUI;
            _cardsInDeck.RemoveAll(x=>x.Guid == cardUI.Guid);
            RemoveBlood(cardUI);
            DestroyImmediate(cardUI.GetComponentInParent<RectTransform>().gameObject);

            NextMoveEnemy();
            //StartCoroutine(EnableLayoutGroup());
        }

        private void RemoveBlood(CardUI cardUI)
        {
            _playerStats.AddBlood(-cardUI.BloodCost());
            _uiController.UpdateBlood(_playerStats.BloodCurrent, _playerStats.BloodMax);
            UpdateDragState();
        }

        private IEnumerator EnableLayoutGroup()
        {
            yield return new WaitForEndOfFrame();
            if (_cardPlayerHorizontalLayoutGroup != null)
            {
                _cardPlayerHorizontalLayoutGroup.enabled = true;
            }
        }

        private void InitStats()
        {
            _stats = new Stats();
            _stats.LoadStats();
            _stats.AddFight();
        }

        private void InitPlayerStats()
        {
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
                    AddBlood(1);
                };
            }
        }

        private void AddBlood(int count)
        {
            var add = Math.Min(count, _playerStats.BloodMax-_playerStats.BloodCurrent);
            _playerStats.AddBlood(add);
            UpdateDragState();
            _uiController.UpdateBlood(_playerStats.BloodCurrent, _playerStats.BloodMax);
        }

        private void UpdateDragState()
        {
            foreach (var card in _cardsInDeck)
            {
                var drag = card.GetComponent<DragAndHoverUI>();
                drag.UpdateInput(card.BloodCost()<=_playerStats.BloodCurrent);
                drag.ForceReturnToPosition();
            }
        }

        private void WinGame()
        {
           _stats.AddEnemiesDeath();

           _uiController.ShowWin(() =>
           {
               SceneLoader.Instance.LoadSceneAsync(SceneType.GamePlay, () =>
               {
                
               });
           });
        }

        private void LoseGame()
        {
            _uiController.ShowLose(() =>
            {
                var currentGame = PlayerPrefs.GetInt("Restart");
                currentGame++;
                PlayerPrefs.SetInt("Restart", currentGame);
                PlayerPrefs.Save();
                if (currentGame == _maxGames)
                {
                    PlayerPrefs.DeleteAll();
                    GameController.Step = 0;
                    _playerController.Reset();
                    SceneLoader.Instance.LoadSceneAsync(SceneType.Main, () => { });
                    return;
                }
                SceneLoader.Instance.LoadSceneAsync(SceneType.GamePlay, () => { });
            });
        }
    }
}