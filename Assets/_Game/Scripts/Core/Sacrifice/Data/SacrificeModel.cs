using NothingAwaits.Core.Health;

namespace NothingAwaits.Core.Actor.Models
{
    public class SacrificeModel: BaseActorModel
    {
        public HealthModel Blood { get; private set; }
        public SacrificeModel(HealthModel health, HealthModel blood) : base(health) => Blood = blood;
    }
}