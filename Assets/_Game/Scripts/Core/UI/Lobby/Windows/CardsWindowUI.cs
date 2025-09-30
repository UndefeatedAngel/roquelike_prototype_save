using UnityEngine;
using NothingAwaits.Core.Cards;
using System.Collections.Generic;
using _Game.Scripts.OldAchitecture.Modules;
using TMPro;

namespace NothingAwaits.Core.UI.Lobby.Windows
{
    public class CardsWindowUI : BaseLobbyWindow
    {
        [Header("Cards Setup")] 
        [SerializeField] private ModelRotatorActivate _modelRotatorActivate;
        [SerializeField] private CardView _cardViewActive;
        [SerializeField] private TMP_Text _nameMobText;
        [SerializeField] private TMP_Text _descriptionMobText;
        [SerializeField] private AnimateText _descriptionText;
        [SerializeField] private Transform _cardsContainer;
        [SerializeField] private CardView _cardViewPrefab;

        private List<CardView> _spawnedCards = new();
        public System.Action<CardModel> OnCardSelected;

        private List<CardModel> _currentCards = new();

        protected override void OnWindowOpened()
        {
            base.OnWindowOpened();
            RefreshCards();
        }

        protected override void OnWindowClosed()
        {
            base.OnWindowClosed();
            ClearCards();
        }
        
        public void SetPlayerCards(List<CardModel> playerCards)
        {
            _currentCards = playerCards ?? new List<CardModel>();
            if (gameObject.activeInHierarchy)
            {
                RefreshCards();
            }
        }

        public void RefreshCards()
        {
            ClearCards();

            foreach (var cardModel in _currentCards)
            {
                if (cardModel?.Config != null)
                {
                    CreateCardView(cardModel);
                }
            }

            if (_currentCards.Count > 0)
            {
                SetCurrentActiveCard(_currentCards[0]);
            }
            
            Debug.Log($"Refreshed cards window with {_currentCards.Count} cards");
        }

        private void CreateCardView(CardModel cardModel)
        {
            var cardView = Instantiate(_cardViewPrefab, _cardsContainer);
            cardView.SetCard(cardModel);
            cardView.OnCardClicked += HandleCardClick;
            _spawnedCards.Add(cardView);
        }

        private void HandleCardClick(CardModel cardModel)
        {
            OnCardSelected?.Invoke(cardModel);
            Debug.Log($"Card selected: {cardModel.Name}");
            SetCurrentActiveCard(cardModel);
        }

        private void SetCurrentActiveCard(CardModel cardModel)
        {
            _modelRotatorActivate.Activate(cardModel.Config.CardData.CardType);
            _cardViewActive.SetCard(cardModel);
            _nameMobText.text = cardModel.Config.NameCard;
            _descriptionMobText.text = cardModel.Config.CardData.GetName();
            _descriptionText.StartAnimation(cardModel.Config.DescriptionCard, 5f);
        }

        private void ClearCards()
        {
            foreach (var card in _spawnedCards)
            {
                if (card != null)
                {
                    card.OnCardClicked -= HandleCardClick;
                    Destroy(card.gameObject);
                }
            }
            _spawnedCards.Clear();
        }
    }
}