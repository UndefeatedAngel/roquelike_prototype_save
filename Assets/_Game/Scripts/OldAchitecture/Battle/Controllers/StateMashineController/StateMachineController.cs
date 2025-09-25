using System;
using System.Collections;
using System.Collections.Generic;
using BlockoutRoguelike.Cards;
using BlockoutRoguelike.Cards.Mobs;
using BlockoutRoguelike.Cards.Person;
using BlockoutRoguelike.Core.PlayerController;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlockoutRoguelike.Battle.Controllers
{
    public enum TeamType
    {
        Player,
        Enemy
    }
    
    public partial class StateMachineController
    {
        [SerializeField] private PlayerController _playerController;
        
        [Header("Player")]
        [SerializeField] private PlayerBattle _player;
        [SerializeField] private List<Slot> _playerSlots;
        [Header("Enemy")]
        [SerializeField] private EnemyBattle _enemy;
        [SerializeField] private List<Slot> _enemySlots;

        [SerializeField] private Button _movesButton;
        [SerializeField] private TMP_Text _movesButtonText;
        
        [Header("UI")]
        [SerializeField] private GameObject _blockUI;

        private TeamType _currentMoveType = TeamType.Player;

        private Action<TeamType> _nextMoveAction;
        
        public Action OnWin;
        public Action OnLose;

        private bool _isWin;
        private bool _isLose;


        private bool IsEndGame()
        {
            return _isWin || _isLose;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W) && !IsEndGame())
            {
                WinGame();
            }
            if (Input.GetKeyDown(KeyCode.L) && !IsEndGame())
            {
                LoseGame();
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (!_blockUI.gameObject.activeSelf && !IsEndGame())
                {
                    EndMove();
                }
            }
        }

        public void Setup(Action<TeamType> nextMoveAction)
        {
            _nextMoveAction = nextMoveAction;
            _movesButton.onClick.AddListener(EndMove);
            _movesButtonText.text = "В бой!";
        }
        

        public void EndMove()
        {
            StartCoroutine(MoveWithType());
        }

        private IEnumerator MoveWithType()
        {
            var isPlayer = IsPlayerMove();

            if (isPlayer)
            {
                _blockUI.gameObject.SetActive(true);
                _movesButtonText.text = "Ход игрока...";
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
            
            //Магичим
            yield return StartCoroutine(Mage(isPlayer));
            //Лечим
            yield return StartCoroutine(Healths(isPlayer));
            //Ставим защиты
            yield return StartCoroutine(Defends(isPlayer));
            //Атакуем
            yield return StartCoroutine(SlotAttack(isPlayer));

            if (isPlayer)
            {
                if (_enemy.CurrentHp < 1)
                {
                    WinGame();
                    yield break;
                }
            }
            else
            {
                if (_playerController.IsDead())
                {
                    LoseGame();
                    yield break;
                }
            }

            _currentMoveType = isPlayer ? TeamType.Enemy : TeamType.Player;
            _movesButtonText.text = isPlayer? "Ход противника...": "В бой!";
            if (!isPlayer)
            {
                _blockUI.gameObject.SetActive(false);
            }
            _nextMoveAction?.Invoke(_currentMoveType);

        }

        private IEnumerator Mage(bool isEnemy)
        {
            yield break;
        }


        private IEnumerator Defends(bool isPlayer)
        {
            var addProtection = 2;
            var protectionSlots = isPlayer?  _playerSlots : _enemySlots;
            var attackSlots = isPlayer ? _enemySlots : _playerSlots;
            
            var protectionMobs = new List<MobUI>();
            
            foreach (var slot in protectionSlots)
            {
                var activeMob = slot.ActiveMob;
                if (activeMob != null && activeMob.MobType == PlayerCardType.Protection)
                {
                    protectionMobs.Add(activeMob);
                }
            }

            foreach (var _ in protectionMobs)
            {
                var length = protectionSlots.Count;
                var allhs = new int[length];
                var at = new int[length];
                for (int i = 0; i < length; i++)
                {
                    allhs[i] = protectionSlots[i].ActiveMob?.AllHealth() ?? 0;
                    at[i] = attackSlots[i].ActiveMob?.Attack ?? 0;
                }

                var numberSlot = FindBestSlotForProtection(allhs, at, addProtection);
                if (numberSlot == -1) yield break;
                    
                var mob =  protectionSlots[numberSlot].ActiveMob;
                mob.AddProtection(addProtection);
                
                //TODO: Animation
                yield return new WaitForSeconds(2.5f);
            }
        }
        
        private IEnumerator Healths(bool isPlayer)
        {
            var addHealth = 2;
            var healthSlots = isPlayer? _playerSlots : _enemySlots;
            var attackSlots = isPlayer ?  _enemySlots : _playerSlots;
            
            var healthMobs = new List<MobUI>();
            foreach (var slot in healthSlots)
            {
                var activeMob = slot.ActiveMob;
                if (activeMob != null && activeMob.MobType == PlayerCardType.Health)
                {
                    healthMobs.Add(activeMob);
                }
            }

            foreach (var _ in healthMobs)
            {
                var length = healthSlots.Count;
                var allhs = new int[length];
                var curhs = new int[length];
                var maxhs = new int[length];
                var ps = new int[length];
                for (int i = 0; i < length; i++)
                {
                    var activeMob = healthSlots[i].ActiveMob;
                    allhs[i] = activeMob?.AllHealth() ?? 0;
                    curhs[i] = activeMob?.Hp  ?? 0;
                    maxhs[i] = activeMob?.MaxHp ?? 0;
                    ps[i] = attackSlots[i].ActiveMob?.Attack ?? 0;

                }

                var numberSlot = FindBestSlotForHP(allhs, curhs, maxhs, ps, addHealth);
                if (numberSlot == -1) yield break; // не лечим если все на максимуме
                
                var mob =  healthSlots[numberSlot].ActiveMob;
                mob.AddHp(addHealth);
                
                //TODO: Animation
                yield return new WaitForSeconds(2f);
            }
        }
        
        private IEnumerator SlotAttack(bool isPlayer)
        {
            var attackSlots = isPlayer ? _playerSlots : _enemySlots;
            var targetSlots = isPlayer ? _enemySlots : _playerSlots;
            
            for (int i = 0; i < attackSlots.Count; i++)
            {
                var attackSlot = attackSlots[i];
                if (attackSlot.ActiveMob != null && attackSlot.ActiveMob.Attack > 0)
                {
                    var targetSlot = targetSlots[i];
                    if (targetSlot.ActiveMob != null)
                    {
                        AttackSlot(attackSlot, targetSlot);
                        yield return new WaitForSeconds(1.1f);
                    }
                    else
                    {
                        AttackMainType(attackSlot);
                        yield return new WaitForSeconds(1.1f);
                    }
                }
            }
        }

        private void WinGame()
        {
            StartCoroutine(AnimationDance(true, () =>
            {
                OnWin?.Invoke();

            }));
        }

        private void LoseGame()
        {
            StartCoroutine(AnimationDance(false, () =>
            {
                OnLose?.Invoke();
            }));
        }

        private bool IsPlayerMove()
        {
            return _currentMoveType == TeamType.Player;
        }

        private void AttackMainType(Slot slot)
        {
            var mob = slot.ActiveMob;
            var person = IsPlayerMove() ? _enemy : _player as BasePerson;
            if (IsPlayerMove())
            {
                if (person.CurrentHp < 1) return;
            }
            else
            {
                if(_playerController.IsDead()) return;
            }
            
            mob.AttackAnimation();
            var defaultPos = mob.GetMobTransform().position;
            mob.GetMobTransform().DOKill();
            mob.GetMobTransform().DOMove(person.PersonTransform.position, 0.5f).OnComplete(() =>
            {
                if (IsPlayerMove())
                {
                    person.AddHp(-slot.ActiveMob.Attack);
                }
                else
                {
                    _playerController.TakeDamage(slot.ActiveMob.Attack);
                }
                mob.GetMobTransform().DOMove(defaultPos, 0.5f).OnComplete(() =>
                {
                    mob.EndMove();
                });
            }).OnStart(() =>
            {
                mob.StartMove();
            });
        }
        
        private void AttackSlot(Slot slot1, Slot slot2)
        {
            var mob1 = slot1.ActiveMob;
            var mob2 = slot2.ActiveMob;
            var defaultPos = mob1.GetMobTransform().position;
            mob1.AttackAnimation();
            mob1.GetMobTransform().DOKill();
            mob1.GetMobTransform().DOMove(mob2.GetMobTransform().position, 0.5f).OnComplete(() =>
            {
                mob2.GiveDamage(mob1.Attack);
                if (mob2.Hp < 1)
                {
                    slot2.ActiveMob.RemoveMob();
                    slot2.ActiveMob = null;
                }
                mob1.GetMobTransform().DOMove(defaultPos, 0.5f).OnComplete(() =>
                {
                    mob1.EndMove();
                });
            }).OnStart(() =>
            {
                mob1.StartMove();
            });
        }
        
        private IEnumerator AnimationDance(bool isWin, Action end)
        {
            AudioController.Instance.PlayWinSound();
            if (isWin)
            {
                _player.WinDance();
                foreach (var playerSlot in _playerSlots)
                {
                    playerSlot.ActiveMob?.WinDance();
                    yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.25f));
                }
            }
            else
            {
                _enemy.WinDance();
                foreach (var enemySlot in _enemySlots)
                {
                    enemySlot.ActiveMob?.WinDance();
                    yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 1f));
                }
            }

            yield return new WaitForSeconds(7f);

            end?.Invoke();
            AudioController.Instance.HideWinSound();
        }
    }
}