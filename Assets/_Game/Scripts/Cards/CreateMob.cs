using UnityEngine;

namespace BlockoutRoguelike.Cards
{
    public class CreateMob: MonoBehaviour
    {
        [SerializeField] private GameObject[] _mobPrefabs;

        public void MobCreate(int number)
        {
            Instantiate(_mobPrefabs[number], transform);
        }
    }
}