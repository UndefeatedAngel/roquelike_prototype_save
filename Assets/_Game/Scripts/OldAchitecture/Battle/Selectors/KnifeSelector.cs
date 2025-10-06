using NothingAwaits.Cards.Mobs;
using NothingAwaits.Core.Actor.Views;

namespace NothingAwaits.Cards
{
    public class KnifeSelector: DragAndReturnSelector
    {
        protected override void Take()
        {
            base.Take();

            var mob = _selector as MobUI;
            if (mob != null)
            {
                mob.Blood();
            }

            var sacrifice = _selector as SacrificeBattle;
            if (sacrifice != null)
            {
                sacrifice.Attack();
            }
        }
    }
}