using System;
using BlockoutRoguelike.Battle.Controllers;
using BlockoutRoguelike.Core;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlockoutRoguelike.Battle.Cards
{
    public class CardUI: MonoBehaviour, IPointerDownHandler, IBeginDragHandler
    {
        [SerializeField] private int _numberCard;
        
        [Header("Components")]
        [SerializeField] protected DragAndHoverUI _draggableCard;

        [SerializeField] protected TMP_Text _bloodText;
        [SerializeField] protected TMP_Text _cardAttackText;
        [SerializeField] protected TMP_Text _cardHPText;
        [SerializeField] protected GameObject _deckObject;

        [SerializeField] private bool _isDeck;
        public bool IsDeck => _isDeck;
        public int NumberCard => _numberCard;

        public Action<CardUI> OnClickCard;

        private CardData _cardData;
        public CardData CardData => _cardData;
        private PlayerStats _playerStats;
        public void SetCard(CardData cardData, PlayerStats stats)
        {
            _playerStats = stats;
            _cardData = cardData;
            _cardAttackText.text = cardData.Attack.ToString();
            _cardHPText.text = cardData.Health.ToString();
            _bloodText.text = cardData.Blood.ToString();
        }


        public void SetDeck(bool state)
        {
            _isDeck = state;
            SetDeckObject(state);
            var scale = state ? 0.7f : 1f;
            transform.localScale = new Vector3(scale, scale, scale);
        }

        public void DisableDeckAnimated(float time)
        {
            var halfTime = time / 2f;
            _isDeck = false;
            transform.DOScaleX(0f, halfTime).OnComplete(() =>
            {
                SetDeckObject(false);
                transform.DOScale(new Vector3(1,1,1), halfTime);
            });
        }

        private void SetDeckObject(bool state)
        {
            _deckObject.SetActive(state);
            _draggableCard.enabled = !state;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsDeck) return;
            
            if (eventData.button != PointerEventData.InputButton.Left) return;
            OnClickCard?.Invoke(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _draggableCard.enabled = _cardData.Blood <= _playerStats.BloodCurrent;
            InputController.ActiveCard = this;
        }

        public int BloodCost()
        {
            return _cardData.Blood;
        }
    }
}