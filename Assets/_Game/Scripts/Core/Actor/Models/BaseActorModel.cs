using NothingAwaits.Core.Actor.Models.Items;

namespace NothingAwaits.Core.Actor.Models
{
    public class BaseActorModel
    {
        public Health Health { get; private set; }
        protected BaseActorModel(Health health) => Health = health;
    }
}