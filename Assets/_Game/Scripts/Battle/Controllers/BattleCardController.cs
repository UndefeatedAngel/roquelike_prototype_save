using System;
using System.Collections;
using System.Collections.Generic;
using BlockoutRoguelike.Battle.Cards;
using BlockoutRoguelike.Cards;
using BlockoutRoguelike.Cards.Mobs;
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

        private float _timeAnimation = 0.5f;
        //Player
        //Boss
        //PlayerSlots
        //BossSlots

        private IEnumerator Start()
        {
            _playerDeckController.Init();
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
        }
        
        private void FlyCardToPlayer()
        {
            var card = _playerDeckController.GetCard();
            card.DisableDeckAnimated(_timeAnimation);
            card.transform.DOJump(_cardPlayerHorizontalLayoutGroup.transform.position, 50, 1 , _timeAnimation).OnComplete(() =>
            {
                card.transform.parent = _cardPlayerHorizontalLayoutGroup.transform;
                card.GetComponent<DragAndHoverUI>().Setup();
            });
        }

        private void SetupSlots()
        {
            _inputController.OnCardToSlot = AddCardToSlot;
        }

        private void AddCardToSlot(CardUI cardUI, Slot slot)
        {
            if (slot.Team != 0) return;

            var prefabMob = _playerMobPrefabs[cardUI.NumberCard];
            var mob = Instantiate(prefabMob, slot.transform);
            mob.GetComponent<MobUI>().SetMob("Bot", cardUI.CardData);
            DestroyImmediate(cardUI.GetComponentInParent<RectTransform>().gameObject);
        }
    }
}