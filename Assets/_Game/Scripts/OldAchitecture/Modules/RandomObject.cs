using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BlockoutRoguelike
{
    public class RandomObject: MonoBehaviour
    {
        [SerializeField] private Transform _parent;
        [SerializeField] private RandomObjectStruct[] _objects;

        private void Start()
        {
            for (var i = _parent.childCount - 1; i >= 0; i--)
            {
                Destroy(_parent.GetChild(i).gameObject);
            }
            
            if (_objects == null || _objects.Length == 0)
            {
                Debug.LogError("No objects assigned!");
                return;
            }
            var totalWeight = _objects.Sum(x => x.Weight);
            var randomValue = Random.Range(0, totalWeight);
            var accumulatedWeight = 0;
            GameObject selectedObject = null;
            foreach (var obj in _objects)
            {
                accumulatedWeight += obj.Weight;
                if (randomValue >= accumulatedWeight) continue;
                
                selectedObject = obj.Prefab;
                break;
            }

            if (selectedObject != null)
            {
                Instantiate(selectedObject, _parent);
            }
            else
            {
                Debug.LogError("Failed to select random object!");
            }
        }
    }
}