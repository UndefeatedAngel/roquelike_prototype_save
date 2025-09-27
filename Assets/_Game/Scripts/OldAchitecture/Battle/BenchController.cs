using NothingAwaits.Cards.Mobs;
using UnityEngine;

namespace NothingAwaits.Cards
{
    public class BenchController: ObjectMoveAndReturnController
    {
        protected override void TakeMob()
        {
            base.TakeMob();
            highlightedMob.Bench();
        }

        protected override void Action3D(RaycastHit hit)
        {
            base.Action3D(hit);
            var mob = hit.transform.GetComponent<MobUI>();
            if (mob!=null&& !mob.Equals(highlightedMob))
            {
                RemoveHighlight();
                highlightedMob = mob;
                HighlightObject(mob);
            }
        }
    }
}