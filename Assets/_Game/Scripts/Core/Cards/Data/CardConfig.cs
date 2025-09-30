using NothingAwaits.Cards.Mobs;
using UnityEngine;

namespace NothingAwaits.Core.Cards.Data
{
    [CreateAssetMenu(fileName = "New CardConfig", menuName = "NothingAwaits/CardConfig")]
    public class CardConfig: ScriptableObject
    {
        [SerializeField] private string _cardId;
        [SerializeField] private string _nameCard;
        [TextArea(3, 10)]
        [SerializeField] private string _descriptionCard;
        [SerializeField] private Sprite _cardSprite;
        [SerializeField] private Color _colorBackground;
        [SerializeField] private CardData _cardData;
        [SerializeField] private MobUI _mobUI;

        public string NameCard => _nameCard;
        public string DescriptionCard => _descriptionCard;
        public string CardId => _cardId;
        public Sprite CardSprite => _cardSprite;
        public Color BackgroundColor => _colorBackground;
        public CardData CardData => _cardData;
    }
}