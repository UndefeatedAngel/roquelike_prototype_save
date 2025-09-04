using System;
using System.Collections.Generic;
using System.Linq;
using BlockoutRoguelike.Cards;
using BlockoutRoguelike.Cards.Mobs;
using UnityEngine;

namespace BlockoutRoguelike.Battle.Controllers
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

        public Action<PlayerCardType> OnDeath;
        public Action<RectTransform> OnDropCoin;
         
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
            
            foreach (var enemySlot in _enemySlots)
            {
                if (enemySlot.ActiveMob == null)
                {
                    var mobData = _deckStack.Pop();
                    var prefab = _prefabsDataBase.FirstOrDefault(x => x.CardType == mobData.CardType);
                    if (prefab == null) return;
                    
                    var mob = Instantiate(prefab.CardPrefab, enemySlot.transform);
                    var mobUI = mob.GetComponent<EnemyMobUI>();
                    enemySlot.ActiveMob = mobUI;
                    mobUI.SetMob($"EU: {mobData.GetName()}", mobData);
                    mobUI.OnDeath = type => OnDeath?.Invoke(type);
                    mobUI.OnCoinDrop = rectTransform => OnDropCoin?.Invoke(rectTransform);
                    break;
                }
            }
        }

        public int GetMaxStat(PlayerCardType type)
        {
            return _deck.Count(x=>x.CardType == type);
        }
    }
}