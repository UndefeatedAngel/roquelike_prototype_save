using NothingAwaits.Core.Actor.Models.Items;

namespace NothingAwaits.Core.Actor.Models
{
    public class PlayerModel : BaseActorModel
    {
        public Coins Coins { get; private set; }
        public PlayerModel(Health health, Coins coins) : base(health) => Coins = coins;
    }
}