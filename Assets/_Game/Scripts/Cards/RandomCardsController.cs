using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace BlockoutRoguelike.Cards
{
    [System.Serializable]
    public class CardData
    {
        public GameObject CardPrefab;
        public int Weight = 1;
    }
    
    public class RandomCardsController: MonoBehaviour
    {
        [Header("Card Database")]
        [SerializeField] private List<CardData> _cardDatabase;
        [SerializeField] private Transform _parentCards;
        [SerializeField] private int _countCreate;
        [SerializeField] private HorizontalLayoutGroup _horizontalLayoutGroup;

        private void Start()
        {
            for (var i = 0; i < _countCreate; i++)
            {
                AddRandomCard();
            }

            StartCoroutine(DisableLayoutGroup());
        }

        private IEnumerator DisableLayoutGroup()
        {
            yield return new WaitForEndOfFrame();
            _horizontalLayoutGroup.enabled = false;
        }

        public void AddRandomCard()
        {
            var selectedCard = GetRandomWeightedCard();
            if (selectedCard == null) return;

            var newCard = Instantiate(selectedCard.CardPrefab, _parentCards);
        }
        
        private CardData GetRandomWeightedCard()
        {
            int totalWeight = _cardDatabase.Sum(card => card.Weight);
            int randomValue = Random.Range(0, totalWeight);
            int currentWeight = 0;

            foreach (var card in _cardDatabase)
            {
                currentWeight += card.Weight;
                if (randomValue < currentWeight)
                {
                    return card;
                }
            }

            return _cardDatabase[0];
        }
    }
}