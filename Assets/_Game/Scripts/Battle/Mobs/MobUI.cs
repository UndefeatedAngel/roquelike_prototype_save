using BlockoutRoguelike.Battle.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlockoutRoguelike.Cards.Mobs
{
    public class MobUI: MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _hpText;
        [SerializeField] private TMP_Text _attackText;
        [SerializeField] private TMP_Text _protectionText;

        private CardData _cardData;
        public void SetMob(string nameText, CardData cardData)
        {
            _cardData = _cardData;
            _nameText.text = nameText;
            _hpText.text = cardData.Health.ToString();
            _attackText.text = cardData.Attack.ToString();
            _protectionText.text = cardData.Protection.ToString();
        }
    }
}