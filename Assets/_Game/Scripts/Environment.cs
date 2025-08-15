using System;
using System.Linq;
using BlockoutRoguelike.Modules;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BlockoutRoguelike
{
    public class Environment: MonoBehaviour
    {
        [SerializeField] private GameObject _bottlePrefab;
        [SerializeField] private Transform[] _bottlePoints;
        [SerializeField] private RoomObject[] _roomEnds;
        public void Setup(bool isStart, bool isEnd, int maxBottles)
        {
            gameObject.SetActive(true);
            GenerateBottle(maxBottles);
            
            if (isStart)
            {
                foreach (var room in _roomEnds)
                {
                    room.Setup(RoomEnum.Enemy, maxBottles);
                }
            }
            else if (isEnd)
            {
                foreach (var room in _roomEnds)
                {
                    room.Setup(RoomEnum.Boss, maxBottles);
                }
            }
            else
            {
                foreach (var room in _roomEnds)
                {
                    var maxCount = Enum.GetValues(typeof(RoomEnum)).Length - 1;
                    var type = Random.Range(0, maxCount);
                    room.Setup((RoomEnum)type, maxBottles);
                }
            }
        }

        private void GenerateBottle(int maxBottles)
        {
            var bottlesToSpawn = Mathf.Min(maxBottles, _bottlePoints.Length);
            var rng = new System.Random();
            var shuffledPoints = _bottlePoints.OrderBy(x => rng.Next()).ToArray();
            for (var i = 0; i < bottlesToSpawn; i++)
            {
                Instantiate(_bottlePrefab, shuffledPoints[i]);
            }
        }
    }
}