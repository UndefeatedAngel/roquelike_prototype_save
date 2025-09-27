using System;
using System.Collections.Generic;
using System.Linq;
using NothingAwaits.Modules;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NothingAwaits
{
    public class Environment: MonoBehaviour
    {
        [SerializeField] private GameObject _bottlePrefab;
        [SerializeField] private Transform[] _bottlePoints;
        [SerializeField] private RoomObject[] _roomEnds;
        public void Setup(bool isStart, RoomEnum testRoom, bool isEnd, int maxBottles, Action<GameObject[]> OnBottles)
        {
            gameObject.SetActive(true);
            var bottles = GenerateBottle(maxBottles);
            OnBottles?.Invoke(bottles.ToArray());
            
            if (isStart)
            {
                foreach (var room in _roomEnds)
                {
                    room.Setup(RoomEnum.Enemy, maxBottles);
                }
            }
            else if (GameController.Step == 1 && testRoom != RoomEnum.Unknown)
            {
                foreach (var room in _roomEnds)
                {
                    room.Setup(testRoom, maxBottles);
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

        private List<GameObject> GenerateBottle(int maxBottles)
        {
            var bottles = new List<GameObject>();
            var bottlesToSpawn = Mathf.Min(maxBottles, _bottlePoints.Length);
            var rng = new System.Random();
            var shuffledPoints = _bottlePoints.OrderBy(x => rng.Next()).ToArray();
            for (var i = 0; i < bottlesToSpawn; i++)
            {
                var bottle= Instantiate(_bottlePrefab, shuffledPoints[i]);
                bottles.Add(bottle);
            }

            return bottles;
        }
    }
}