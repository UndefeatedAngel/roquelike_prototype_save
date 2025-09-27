using NothingAwaits.Core.Coins;
using NothingAwaits.Core.Health;

namespace NothingAwaits.Core.Actor.Models
{
    public class PlayerModel : BaseActorModel
    {
        public CoinsModel Coins { get; private set; }
        public PlayerModel(HealthModel health, CoinsModel coins) : base(health) => Coins = coins;
    }
}