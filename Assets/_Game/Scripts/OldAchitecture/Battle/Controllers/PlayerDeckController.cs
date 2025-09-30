using System;
using System.Collections.Generic;
using System.Linq;
using NothingAwaits.Battle.Cards;
using NothingAwaits.Core;
using NothingAwaits.Core.Cards;
using NothingAwaits.Core.Cards.Data;
using UnityEngine;

namespace NothingAwaits.Battle.Controllers
{
    [Serializable]
    public class PrefabData
    {
        public CardType CardType;
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
        public Action<CardUI> OnCardSelect;
        public Action<CardUI> OnCardDeckClick;

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
                        OnCardSelect?.Invoke(cardUI);
                    };
                    ui.OnClickCardDeck += cardUI =>
                    {
                        OnCardDeckClick?.Invoke(cardUI);
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