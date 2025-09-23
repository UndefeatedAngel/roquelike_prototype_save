using BlockoutRoguelike.Core.Person.Models.Items;

namespace BlockoutRoguelike.Core.Person.Models
{
    public class BasePersonModel
    {
        public Health Health { get; private set; }
        protected BasePersonModel(Health health) => Health = health;
    }
}