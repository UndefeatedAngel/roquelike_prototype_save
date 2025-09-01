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
            var mobData = _deckStack.Pop();
            var prefab = _prefabsDataBase.FirstOrDefault(x => x.CardType == mobData.CardType);
            if (prefab == null) return;
            
            foreach (var enemySlot in _enemySlots)
            {
                if (enemySlot.ActiveMob == null)
                {
                    var mob = Instantiate(prefab.CardPrefab, enemySlot.transform);
                    enemySlot.ActiveMob = mob.GetComponent<MobUI>();
                    enemySlot.ActiveMob.SetMob("EnemyMob", mobData);
                    break;
                }
            }
        }

    }
}