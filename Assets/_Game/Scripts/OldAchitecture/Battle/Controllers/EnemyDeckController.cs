using System;
using System.Collections.Generic;
using System.Linq;
using NothingAwaits.Cards;
using NothingAwaits.Cards.Mobs;
using NothingAwaits.Core.Cards;
using NothingAwaits.Core.Cards.Data;
using UnityEngine;

namespace NothingAwaits.Battle.Controllers
{
    public class EnemyDeckController: MonoBehaviour
    {
        [Header("Setup")] 
        [SerializeField] private List<CardData> _deck;
        [SerializeField] private List<Slot> _playerSlots;
        [SerializeField] private List<Slot> _enemySlots;
        [Header("Deck")]
        [Space]
        [Header("Prefabs")]
        [SerializeField] private List<PrefabData> _prefabsDataBase;
        
        private Stack<CardData> _deckStack = new();

        public Action<CardType> OnDeath;
        public Action<RectTransform> OnDropCoin;


        private int _nextSlot;
        public void Init()
        {
            InitDeck();
        }
        private void InitDeck()
        {
            var rng = new System.Random();
            var sortDeck = _deck.OrderBy(x => rng.Next()).ToList();
            foreach (var deck in sortDeck)
            {
                _deckStack.Push(deck);
            }
        }

        public void Move()
        {
            if (_deckStack.Count < 1) return;
            if(_nextSlot<0) return;
            var enemySlot = _enemySlots[_nextSlot];
            
            var mobData = _deckStack.Pop();
            var prefab = _prefabsDataBase.FirstOrDefault(x => x.CardType == mobData.CardType);
            if (prefab == null) return;
            
            var mob = Instantiate(prefab.CardPrefab, enemySlot.transform);
            var mobUI = mob.GetComponent<EnemyMobUI>();
            enemySlot.ActiveMob = mobUI;
            mobUI.SetMob(TeamType.Enemy,$"EU: {mobData.GetName()}", mobData, null);
            mobUI.OnDeath = type => OnDeath?.Invoke(type);
            mobUI.OnCoinDrop = rectTransform => OnDropCoin?.Invoke(rectTransform);
        }

        public int GetMaxStat(CardType type)
        {
            return _deck.Count(x=>x.CardType == type);
        }

        public int GetNextSlot()
        {
            _nextSlot = -1;
            if (_deckStack.Count < 1) return 0;

            var slots = new List<int>();
            for (var i = 0; i < _enemySlots.Count; i++)
            {
                var slot = _enemySlots[i];
                if (slot.ActiveMob == null)
                {
                    slots.Add(i);
                }
            }

            if (slots.Count <= 0) return 0;

            var saveSlot = -1;
            var saveAttack = 0;
            for (int i = 0; i < _playerSlots.Count; i++)
            {
                if(!slots.Contains(i)) continue;
                var mob = _playerSlots[i].ActiveMob;
                if (mob != null && mob.Attack > 0)
                {
                    if (mob.Attack > saveAttack)
                    {
                        saveSlot = i;
                        saveAttack = mob.Attack;
                    }
                }
            }

            if (saveSlot >= 0)
            {
                _nextSlot = saveSlot;
                return saveSlot;
            }
            ;
            
            var random = UnityEngine.Random.Range(0, slots.Count);
            _nextSlot = slots[random];
            return slots[random];

        }


        public string GetNextEnemyName()
        {
            return _deckStack.Count < 1 ? "" : _deckStack.Peek().GetName();
        }

        public CardType GetNextEnemyType()
        {
            return _deckStack.Count < 1 ? CardType.Empty : _deckStack.Peek().CardType;
        }
        
        public CardData GetNextEnemyData()
        {
            return _deckStack.Count < 1 ? new CardData(): _deckStack.Peek();
        }
    }
}