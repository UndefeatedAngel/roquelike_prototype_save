using NothingAwaits.Core.Cards.Data;
using UnityEngine;
using System.Collections.Generic;

namespace NothingAwaits.Core.Cards
{
    [CreateAssetMenu(fileName = "New CardsContainer", menuName = "NothingAwaits/CardsContainer")]
    public class CardsContainer : ScriptableObject
    {
        [SerializeField] private CardConfig[] _allCards;

        public CardConfig[] AllCards => _allCards;

        public CardConfig GetCardById(string cardId)
        {
            foreach (var card in _allCards)
            {
                if (card != null && card.name == cardId)
                {
                    return card;
                }
            }
            return null;
        }

        public Dictionary<string, CardConfig> GetCardDictionary()
        {
            var dictionary = new Dictionary<string, CardConfig>();
            foreach (var card in _allCards)
            {
                if (card != null)
                {
                    dictionary[card.name] = card;
                }
            }
            return dictionary;
        }
    }
}