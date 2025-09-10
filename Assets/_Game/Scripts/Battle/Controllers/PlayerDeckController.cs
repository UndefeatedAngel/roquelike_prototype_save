using System;
using System.Collections.Generic;
using System.Linq;
using BlockoutRoguelike.Battle.Cards;
using BlockoutRoguelike.Core;
using UnityEngine;

namespace BlockoutRoguelike.Battle.Controllers
{
    public enum PlayerCardType
    {
        Empty,
        Attack,
        Protection, 
        Health,
        Mage
    }

    [Serializable]
    public class CardData
    {
        public PlayerCardType CardType;
        public int Attack;
        public int Protection;
        public int Health;
        public int Blood;

        public string GetName()
        {
            return CardType switch
            {
                PlayerCardType.Attack => "Attacker",
                PlayerCardType.Protection => "Defender",
                PlayerCardType.Health => "Hiller",
                PlayerCardType.Mage => "Mage",
                _ => ""
            };
        }
    }
    
    [Serializable]
    public class PrefabData
    {
        public PlayerCardType CardType;
        public GameObject CardPrefab;
    }
    
    public class PlayerDeckController: MonoBehaviour
    {
        [Header("Setup")] 
        [SerializeField] private List<CardData> _deck;
        [Header("Deck")]
        [SerializeField] private Transform _deckContainer;
        [Space]
        [Header("Prefabs")]
        [SerializeField] private List<PrefabData> _prefabsDataBase;

        private Stack<CardUI> _cardsInDeck = new();

        public Action<CardUI> OnCardClick;
        public void Init(PlayerStats playerStats)
        {
            InitDeck(playerStats);
        }

        private void InitDeck(PlayerStats playerStats)
        {
            var rng = new System.Random();
            var sortDeck = _deck.OrderBy(x => rng.Next()).ToList();
            var number = 0;
            foreach (var playerCard in sortDeck)
            {
                var prefabs = _prefabsDataBase.FirstOrDefault(x => x.CardType == playerCard.CardType);
                if (prefabs != null)
                {
                    var card = Instantiate(prefabs.CardPrefab, _deckContainer);
                    var ui = card.GetComponent<CardUI>();
                    ui.SetCard(playerCard, playerStats);
                    ui.SetDeck(true);
                    ui.GetComponent<RectTransform>().anchoredPosition = new Vector2(number, number);
                    ui.OnClickCard += cardUI =>
                    {
                        OnCardClick?.Invoke(cardUI);
                    };
                    number+=2;
                    _cardsInDeck.Push(ui);
                }
            }
        }

        public CardUI GetCard()
        {
            if (_cardsInDeck.Count < 1) return null;
            
            var card = _cardsInDeck.Pop();
            return card.GetComponent<CardUI>();
        }
    }
}