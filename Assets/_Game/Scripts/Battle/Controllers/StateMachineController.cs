using System;
using System.Collections;
using System.Collections.Generic;
using BlockoutRoguelike.Cards;
using BlockoutRoguelike.Cards.Person;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlockoutRoguelike.Battle.Controllers
{
    public class StateMachineController: MonoBehaviour
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

        private int _currentMoveType = 0;

        private Action<int> _nextMoveAction;

        public void Setup(Action<int> nextMoveAction)
        {
            _nextMoveAction = nextMoveAction;
            _movesButton.onClick.AddListener(EndMove);
            _movesButtonText.text = "Player move";
        }
        

        public void EndMove()
        {
            if (_currentMoveType == 0)
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
            _movesButtonText.text = "Player calculate...";
            for (int i = 0; i < _playerSlots.Count; i++)
            {
                var playerSlot = _playerSlots[i];
                if (playerSlot.ActiveMob != null && playerSlot.ActiveMob.Attack>0)
                {
                    var enemySlot = _enemySlots[i];
                    if (enemySlot.ActiveMob != null)
                    {
                        AttackSlot(playerSlot, enemySlot);
                        yield return new WaitForSeconds(1.1f);
                    }
                    else
                    {
                        AttackEnemy(playerSlot);
                        yield return new WaitForSeconds(1.1f);
                    }
                }
            }

            if (_enemy.CurrentHp < 1) yield break;
            
            _currentMoveType = 1;
            _movesButtonText.text = "Enemy move";
            _nextMoveAction?.Invoke(_currentMoveType);
        }

        private void AttackSlot(Slot slot1, Slot slot2)
        {
            var mob1 = slot1.ActiveMob;
            var mob2 = slot2.ActiveMob;
            var defaultPos = mob1.transform.position;
            mob1.AttackAnimation();
            mob1.transform.DOMove(mob2.transform.position, 0.5f).OnComplete(() =>
            {
                mob2.AddHp(-mob1.Attack);
                if (mob2.Hp < 1)
                {
                    slot2.ActiveMob.RemoveMob();
                    slot2.ActiveMob = null;
                }
                mob1.transform.DOMove(defaultPos, 0.5f);
            });
        }
        
        private void AttackEnemy(Slot playerSlot)
        {
            var mob = playerSlot.ActiveMob;
            if (_enemy.CurrentHp < 1) return;
            mob.AttackAnimation();
            var defaultPos = mob.transform.position;
            mob.transform.DOMove(_enemy.PersonTransform.position, 0.5f).OnComplete(() =>
            {
                _enemy.AddHp(-playerSlot.ActiveMob.Attack);
                mob.transform.DOMove(defaultPos, 0.5f);
            });
        }
        
        
        private void AttackPlayer(Slot enemySlot)
        {
            var mob = enemySlot.ActiveMob;
            if (_player.CurrentHp < 1) return;
            mob.AttackAnimation();
            var defaultPos = mob.transform.position;
            mob.transform.DOMove(_player.PersonTransform.position, 0.5f).OnComplete(() =>
            {
                _player.AddHp(-enemySlot.ActiveMob.Attack);
                mob.transform.DOMove(defaultPos, 0.5f);
            });
        }

        private IEnumerator EnemyMove()
        {
            yield return new WaitForSeconds(0.5f);
            _movesButtonText.text = "Enemy calculate...";
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
                        AttackPlayer(enemySlot);
                        yield return new WaitForSeconds(1.1f);
                    }
                }
            }

            _currentMoveType = 0;
            _movesButtonText.text = "Player move";
            _blockUI.gameObject.SetActive(false);
            _nextMoveAction?.Invoke(_currentMoveType);
        }
    }
}