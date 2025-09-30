using NothingAwaits.Core.Cards.Data;
using UnityEngine;

namespace NothingAwaits.Core.Cards
{
    [System.Serializable]
    public class CardModel
    {
        [SerializeField] private string _cardId;
        [SerializeField] private CardConfig _config;

        public string CardId => _cardId;
        public CardConfig Config => _config;
        public string Name => _config != null ? _config.name : "";
        public Sprite Sprite => _config != null ? _config.CardSprite : null;
        public Color Color => _config != null ? _config.BackgroundColor : Color.white;
        public CardData Data => _config != null ? _config.CardData : null;

        public CardModel(CardConfig config)
        {
            _config = config;
            _cardId = config.CardId;
        }

        public CardModel(string cardId, CardConfig config)
        {
            _cardId = cardId;
            _config = config;
        }
    }
}