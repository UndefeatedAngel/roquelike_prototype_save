using BlockoutRoguelike.Battle.Controllers;
using BlockoutRoguelike.Cards.Mobs;
using UnityEngine;

namespace BlockoutRoguelike.Cards
{
    public class KnifeController: ObjectMoveAndReturnController
    {
        protected override void TakeMob()
        {
            base.TakeMob();
            highlightedMob.Blood();
        }

        protected override void Action3D(RaycastHit hit)
        {
            base.Action3D(hit);
            
            var sacrifice = hit.transform.GetComponent<Sacrifice>();
            if (sacrifice!=null && !sacrifice.Equals(highlightedObject))
            {
                RemoveHighlight();
                
                highlightedObject = sacrifice;
                HighlightObject(highlightedObject);
            }

            var mob = hit.transform.GetComponent<MobUI>();
            if (mob!=null&& !mob.Equals(highlightedMob) && mob.Team == TeamType.Player)
            {
                RemoveHighlight();
                highlightedMob = mob;
                HighlightObject(mob);
            }
        }
    }
}