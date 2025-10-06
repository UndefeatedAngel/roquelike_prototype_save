using System;
using NothingAwaits.Cards.Mobs;

namespace NothingAwaits.Cards
{
    public class BenchControlItem: DragAndReturnSelector
    {
        public Action<MobUI> OnBenchMob;
        
        protected override void Take()
        {
            base.Take();
            
            var mob = _selector as MobUI;
            if (mob != null)
            {
                OnBenchMob?.Invoke(mob);
            }
        }
    }
}