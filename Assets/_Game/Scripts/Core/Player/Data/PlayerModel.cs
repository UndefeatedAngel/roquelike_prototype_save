using NothingAwaits.Core.Actor.Models;
using NothingAwaits.Core.Coins;
using NothingAwaits.Core.Health;
using NothingAwaits.Core.Cards.Data;
using NothingAwaits.Core.Cards;
using System.Collections.Generic;

namespace NothingAwaits.Core.Player
{
    public class PlayerModel : BaseActorModel
    {
        public CoinsModel Coins { get; private set; }
        public List<CardModel> Cards { get; private set; }

        public PlayerModel(HealthModel health, CoinsModel coins, List<CardConfig> baseCards) : base(health)
        {
            Coins = coins;
            Cards = new List<CardModel>();
            InitializeBaseCards(baseCards);
        }

        private void InitializeBaseCards(List<CardConfig> baseCards)
        {
            foreach (var config in baseCards)
            {
                if (config != null)
                {
                    // Убираем проверку на дубликаты - добавляем все карты
                    Cards.Add(new CardModel(config));
                }
            }
        }

        public void AddCard(CardConfig cardConfig)
        {
            if (cardConfig != null)
            {
                // Добавляем даже дубликаты
                Cards.Add(new CardModel(cardConfig));
            }
        }

        public void AddCard(CardModel cardModel)
        {
            if (cardModel != null && cardModel.Config != null)
            {
                // Добавляем даже дубликаты
                Cards.Add(cardModel);
            }
        }

        public void RemoveCard(CardConfig cardConfig)
        {
            Cards.RemoveAll(card => card.Config == cardConfig);
        }

        public void RemoveCard(CardModel cardItem)
        {
            Cards.Remove(cardItem);
        }
    }
}