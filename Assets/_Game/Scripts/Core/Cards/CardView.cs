using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NothingAwaits.Core.Cards
{
    public class CardView : MonoBehaviour
    {
        [Header("Setup")] 
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _cardImage;
        [SerializeField] private TMP_Text _bloodText;
        [SerializeField] private TMP_Text _cardAttackText;
        [SerializeField] private TMP_Text _cardHPText;
        [SerializeField] private Button _cardButton;

        private CardModel _cardModel;
        public Action<CardModel> OnCardClicked;

        private void Awake()
        {
            if (_cardButton != null)
            {
                _cardButton.onClick.AddListener(OnCardClick);
            }
        }

        public void SetCard(CardModel cardModel)
        {
            _cardModel = cardModel;
            
            if (_cardModel?.Config != null)
            {
                _nameText.text = _cardModel.Config.NameCard;
                _cardImage.sprite = _cardModel.Sprite;
                _cardAttackText.text = _cardModel.Data.Attack.ToString();
                _cardHPText.text = _cardModel.Data.Health.ToString();
                _bloodText.text = _cardModel.Data.Blood.ToString();
                _backgroundImage.color = _cardModel.Color;
            }
        }

        private void OnCardClick()
        {
            OnCardClicked?.Invoke(_cardModel);
        }

        private void OnDestroy()
        {
            if (_cardButton != null)
            {
                _cardButton.onClick.RemoveListener(OnCardClick);
            }
        }
    }
}