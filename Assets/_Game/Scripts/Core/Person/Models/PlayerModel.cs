using BlockoutRoguelike.Core.Person.Models.Items;

namespace BlockoutRoguelike.Core.Person.Models
{
    public class PlayerModel : BasePersonModel
    {
        public Coins Coins { get; private set; }
        public PlayerModel(Health health, Coins coins) : base(health) => Coins = coins;
    }
}