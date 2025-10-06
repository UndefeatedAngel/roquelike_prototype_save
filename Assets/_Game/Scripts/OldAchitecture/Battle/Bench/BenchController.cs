using NothingAwaits.Cards.Mobs;
using UnityEngine;

namespace NothingAwaits.Cards
{
    public class BenchController: MonoBehaviour
    {
        [SerializeField] private Transform[] _benchPoints;
        [SerializeField] private BenchItemUI[] _benchItems;
        [SerializeField] private BenchDraggableItem[] _benchDraggableItems;
        [SerializeField] private BenchControlItem _benchControlItem;
        public void Init()
        {
            _benchControlItem.OnBenchMob = OnBenchMob;
            for (int i = 0; i < _benchPoints.Length; i++)
            {
                var index = i;
                var draggableItem = _benchDraggableItems[index];
                draggableItem.OnSelectSlot += slot =>
                {
                    OnSlotSelected(slot, index);
                };
            }

            foreach (var benchItem in _benchItems)
            {
                benchItem.Set(null, "");
            }
        }

        private bool TryGetPoint(out Transform point, out int index)
        {
            for (var i = 0; i < _benchPoints.Length; i++)
            {
                var benchPoint = _benchPoints[i];
                if (benchPoint.childCount != 0) continue;
                
                point = benchPoint;
                index = i;
                return true;
            }
            point = null;
            index = -1;
            return false;
        }

        private void OnBenchMob(MobUI mob)
        {
            if (TryGetPoint(out Transform point, out int index))
            {
                if (!mob.IsBenchMob())
                {
                    _benchItems[index].Set(mob.GetIcon(), mob.Hp.ToString());
                    mob.Bench(point);
                }
            }
            else
            {
                //TODO: Hint недостаточно места
            }
        }

        private void OnSlotSelected(Slot slot, int index)
        {
            if (slot.Team == 1) return;
            
            var mob = _benchPoints[index].GetChild(0).GetComponent<MobUI>();
            mob.ReturnSlot(slot, slot.transform);
            _benchItems[index].Set(null, "");
        }
    }
}