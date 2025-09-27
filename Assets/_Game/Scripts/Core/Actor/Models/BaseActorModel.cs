using NothingAwaits.Core.Health;

namespace NothingAwaits.Core.Actor.Models
{
    public class BaseActorModel
    {
        public HealthModel Health { get; private set; }
        protected BaseActorModel(HealthModel health) => Health = health;
    }
}