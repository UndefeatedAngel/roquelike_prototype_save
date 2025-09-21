using System;
using System.Collections;
using System.Collections.Generic;
using BlockoutRoguelike.Cards;
using BlockoutRoguelike.Cards.Mobs;
using BlockoutRoguelike.Cards.Person;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlockoutRoguelike.Battle.Controllers
{
    public enum QueueMove
    {
        Player,
        Enemy
    }
    
    public partial class StateMachineController: MonoBehaviour
    {
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

        private QueueMove _currentMoveType = QueueMove.Player;

        private Action<QueueMove> _nextMoveAction;
        
        public Action OnWin;
        public Action OnLose;

        public void Setup(Action<QueueMove> nextMoveAction)
        {
            _nextMoveAction = nextMoveAction;
            _movesButton.onClick.AddListener(EndMove);
            _movesButtonText.text = "В бой!";
        }
        

        public void EndMove()
        {
            if (_currentMoveType == QueueMove.Player)
            {
                StartCoroutine(PlayerMove());
            }
            else
            {
                StartCoroutine(EnemyMove());
            }
        }

        private IEnumerator PlayerMove()
        {
            _blockUI.gameObject.SetActive(true);
            _movesButtonText.text = "Ход игрока...";
            
            //Магичим
            //Лечим
            yield return StartCoroutine(PlayerHealths());
            //Ставим защиты
            yield return StartCoroutine(PlayerDefends());
            //Атакуем
            yield return StartCoroutine(PlayerAttack());
            
            
            

            if (_enemy.CurrentHp < 1)
            {
                OnWin?.Invoke();
                yield break;
            }
            
            _currentMoveType = QueueMove.Enemy;
            _movesButtonText.text = "Ход противника...";
            _nextMoveAction?.Invoke(_currentMoveType);
        }

        private IEnumerator PlayerAttack()
        {
            for (int i = 0; i < _playerSlots.Count; i++)
            {
                var playerSlot = _playerSlots[i];
                if (playerSlot.ActiveMob != null && playerSlot.ActiveMob.Attack > 0)
                {
                    var enemySlot = _enemySlots[i];
                    if (enemySlot.ActiveMob != null)
                    {
                        AttackSlot(playerSlot, enemySlot);
                        yield return new WaitForSeconds(1.1f);
                    }
                    else
                    {
                        AttackMainType(playerSlot);
                        yield return new WaitForSeconds(1.1f);
                    }
                }
            }
        }
        
        private IEnumerator PlayerDefends()
        {
            var protectionMobs = new List<MobUI>();
            foreach (var slot in _playerSlots)
            {
                var activeMob = slot.ActiveMob;
                if (activeMob != null && activeMob.MobType == PlayerCardType.Protection)
                {
                    protectionMobs.Add(activeMob);
                }
            }

            foreach (var _ in protectionMobs)
            {
                var addProtection = 2;
                var length = _playerSlots.Count;
                var allhs = new int[length];
                var ps = new int[length];
                for (int i = 0; i < length; i++)
                {
                    allhs[i] = _playerSlots[i].ActiveMob?.AllHealth() ?? 0;
                    ps[i] = _enemySlots[i].ActiveMob?.AllHealth() ?? 0;
                }

                var numberSlot = FindBestSlotForProtection(allhs, ps, addProtection);
                if (numberSlot == -1) yield break;
                    
                var mob =  _playerSlots[numberSlot].ActiveMob;
                mob.AddProtection(addProtection);
                
                //TODO: Animation
                yield return new WaitForSeconds(1f);
            }
        }
        
        private IEnumerator PlayerHealths()
        {
            var healthMobs = new List<MobUI>();
            foreach (var slot in _playerSlots)
            {
                var activeMob = slot.ActiveMob;
                if (activeMob != null && activeMob.MobType == PlayerCardType.Health)
                {
                    healthMobs.Add(activeMob);
                }
            }

            foreach (var _ in healthMobs)
            {
                var addHealth = 2;
                var length = _playerSlots.Count;
                var allhs = new int[length];
                var curhs = new int[length];
                var maxhs = new int[length];
                var ps = new int[length];
                for (int i = 0; i < length; i++)
                {
                    var activeMob = _playerSlots[i].ActiveMob;
                    allhs[i] = activeMob?.AllHealth() ?? 0;
                    curhs[i] = activeMob?.Hp  ?? 0;
                    maxhs[i] = activeMob?.MaxHp ?? 0;
                        ps[i] = _enemySlots[i].ActiveMob?.AllHealth() ?? 0;

                }

                var numberSlot = FindBestSlotForHP(allhs, curhs, maxhs, ps, addHealth);
                if (numberSlot == -1) yield break; // не лечим если все на максимуме
                
                var mob =  _playerSlots[numberSlot].ActiveMob;
                mob.AddHp(addHealth);
                
                //TODO: Animation
                yield return new WaitForSeconds(2f);
            }
        }

        private IEnumerator EnemyMove()
        {
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < _enemySlots.Count; i++)
            {
                var enemySlot = _enemySlots[i];
                if (enemySlot.ActiveMob != null && enemySlot.ActiveMob.Attack>0)
                {
                    var playerSlot = _playerSlots[i];
                    if (playerSlot.ActiveMob != null)
                    {
                        AttackSlot(enemySlot, playerSlot);
                        yield return new WaitForSeconds(1.1f);
                    }
                    else
                    {
                        AttackMainType(enemySlot);
                        yield return new WaitForSeconds(1.1f);
                    }
                }
            }

            if (_player.CurrentHp < 1)
            {
                OnLose?.Invoke();
                yield break;
            }

            _currentMoveType = QueueMove.Player;
            _movesButtonText.text = "В бой!";
            _blockUI.gameObject.SetActive(false);
            _nextMoveAction?.Invoke(_currentMoveType);
        }

        private IEnumerator MoveWithType()
        {
            _blockUI.gameObject.SetActive(IsPlayerMove());
            _movesButtonText.text = IsPlayerMove() ? "Ход игрока..." : "Ход противника...";
            yield return new WaitForSeconds(0.5f);
            var slots1 = IsPlayerMove() ? _playerSlots : _enemySlots;
            var slots2 = IsPlayerMove() ? _enemySlots : _playerSlots;
            for (int i = 0; i < slots1.Count; i++)
            {
                var slot1 = slots1[i];
                if (slot1.ActiveMob != null && slot1.ActiveMob.Attack>0)
                {
                    var slot2 = slots2[i];
                    if (slot2.ActiveMob != null)
                    {
                        AttackSlot(slot1, slot2);
                        yield return new WaitForSeconds(1.1f);
                    }
                    else
                    {
                        AttackMainType(slot1);
                        yield return new WaitForSeconds(1.1f);
                    }
                }
            }
            
            _currentMoveType = QueueMove.Player;
            _movesButtonText.text = "В бой!";
            _blockUI.gameObject.SetActive(false);
            _nextMoveAction?.Invoke(_currentMoveType);
        }

        private bool IsPlayerMove()
        {
            return _currentMoveType == QueueMove.Player;
        }

        private void AttackMainType(Slot slot)
        {
            var mob = slot.ActiveMob;
            var person = IsPlayerMove() ? _enemy : _player as BasePerson;
            if (person.CurrentHp < 1) return;
            
            mob.AttackAnimation();
            var defaultPos = mob.transform.position;
            mob.transform.DOKill();
            mob.transform.DOMove(person.PersonTransform.position, 0.5f).OnComplete(() =>
            {
                person.AddHp(-slot.ActiveMob.Attack);
                mob.transform.DOMove(defaultPos, 0.5f);
            });
        }
        
        private void AttackSlot(Slot slot1, Slot slot2)
        {
            var mob1 = slot1.ActiveMob;
            var mob2 = slot2.ActiveMob;
            var defaultPos = mob1.transform.position;
            mob1.AttackAnimation();
            mob1.transform.DOKill();
            mob1.transform.DOMove(mob2.transform.position, 0.5f).OnComplete(() =>
            {
                mob2.GiveDamage(mob1.Attack);
                if (mob2.Hp < 1)
                {
                    slot2.ActiveMob.RemoveMob();
                    slot2.ActiveMob = null;
                }
                mob1.transform.DOMove(defaultPos, 0.5f);
            });
        }
    }
}