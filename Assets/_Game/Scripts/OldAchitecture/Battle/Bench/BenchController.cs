using NothingAwaits.Cards.Mobs;
using UnityEngine;

namespace NothingAwaits.Cards
{
    public class BenchController: ObjectMoveAndReturnController
    {
        private static BenchController _instance;
        public static BenchController Instance => _instance;

        protected override void Start()
        {
            base.Start();
            _instance = this;
        }
        
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

        public void Bench(Sprite icon, int hp, MobUI mob)
        {
            
        }
    }
}