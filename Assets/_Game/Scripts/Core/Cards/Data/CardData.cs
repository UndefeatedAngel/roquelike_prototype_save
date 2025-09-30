using System;

namespace NothingAwaits.Core.Cards.Data
{
    [Serializable]
    public class CardData
    {
        public CardType CardType;
        public int Attack;
        public int Protection;
        public int Health;
        public int Blood;

        public string GetName()
        {
            return CardType switch
            {
                CardType.Attack => "Attacker",
                CardType.Protection => "Defender",
                CardType.Health => "Hiller",
                CardType.Mage => "Mage",
                _ => ""
            };
        }
    }
}