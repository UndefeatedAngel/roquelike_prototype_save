using NothingAwaits.Storage;
using System.Collections.Generic;
using NothingAwaits.Core.Cards;
using NothingAwaits.Core.Cards.Data;
using UnityEngine;

namespace NothingAwaits.Core.Player
{
    public class PlayerDataManager : IPlayerDataService
    {
        private const string SaveKey = "nothingawaits.player";
        private const string HealthKey = "health";
        private const string MaxHealthKey = "maxHealth";
        private const string CoinsKey = "coins";
        private const string CardsKey = "cards";

        private readonly PlayerModel _playerModel;
        private readonly IStorage _storage;
        private readonly CardConfig[] _baseCards;
        private readonly CardsContainer _cardsContainer;

        public PlayerDataManager(PlayerModel playerModel, CardConfig[] baseCards, CardsContainer cardsContainer)
        {
            _playerModel = playerModel;
            _storage = StorageManager.Storage;
            _baseCards = baseCards;
            _cardsContainer = cardsContainer;
        }

        public PlayerData Load()
        {
            var data = new PlayerData
            {
                health = _storage.LoadInt($"{SaveKey}_{HealthKey}", -1),
                maxHealth = _storage.LoadInt($"{SaveKey}_{MaxHealthKey}", -1),
                coins = _storage.LoadInt($"{SaveKey}_{CoinsKey}", -1),
                cardIds = LoadCards()
            };

            if (data.health != -1 && data.maxHealth != -1)
            {
                _playerModel.Health.SetHealth(data.health, data.maxHealth);
            }

            if (data.coins != -1)
            {
                _playerModel.Coins.SetAmount(data.coins);
            }

            LoadCardsToModel(data.cardIds);

            return data;
        }

        public void Save(PlayerData data)
        {
            _storage.SaveInt($"{SaveKey}_{HealthKey}", data.health);
            _storage.SaveInt($"{SaveKey}_{MaxHealthKey}", data.maxHealth);
            _storage.SaveInt($"{SaveKey}_{CoinsKey}", data.coins);
        }

        public void Save()
        {
            Save(new PlayerData
            {
                health = _playerModel.Health.Current,
                maxHealth = _playerModel.Health.Max,
                coins = _playerModel.Coins.Amount,
                cardIds = GetCardIds()
            });
        }

        public void Delete()
        {
            _storage.DeleteKey($"{SaveKey}_{HealthKey}");
            _storage.DeleteKey($"{SaveKey}_{MaxHealthKey}");
            _storage.DeleteKey($"{SaveKey}_{CoinsKey}");
            _storage.DeleteKey($"{SaveKey}_{CardsKey}");
        }

        private List<string> LoadCards()
        {
            var cardsJson = _storage.LoadString($"{SaveKey}_{CardsKey}", "");
            if (string.IsNullOrEmpty(cardsJson)) return new List<string>();

            return JsonUtility.FromJson<CardListWrapper>(cardsJson)?.cardIds ?? new List<string>();
        }

        private void SaveCards(List<string> cardIds)
        {
            var wrapper = new CardListWrapper { cardIds = cardIds };
            _storage.SaveString($"{SaveKey}_{CardsKey}", JsonUtility.ToJson(wrapper));
        }

        private List<string> GetCardIds()
        {
            var cardIds = new List<string>();
            foreach (var cardModel in _playerModel.Cards)
            {
                if (cardModel != null && !string.IsNullOrEmpty(cardModel.CardId))
                {
                    cardIds.Add(cardModel.CardId);
                }
            }
            return cardIds;
        }

        private void LoadCardsToModel(List<string> cardIds)
        {
            _playerModel.Cards.Clear();

            if (cardIds == null || cardIds.Count == 0)
            {
                foreach (var baseCard in _baseCards)
                {
                    if (baseCard != null) _playerModel.AddCard(baseCard);
                }
                return;
            }

            var cardDictionary = _cardsContainer.GetCardDictionary();

            foreach (var cardId in cardIds)
            {
                if (cardDictionary.TryGetValue(cardId, out var cardConfig))
                {
                    _playerModel.AddCard(new CardModel(cardId, cardConfig));
                }
            }
        }

        [System.Serializable]
        private class CardListWrapper
        {
            public List<string> cardIds;
        }
    }
}