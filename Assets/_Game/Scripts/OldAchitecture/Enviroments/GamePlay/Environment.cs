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
        public void Setup(bool isStart, RoomEnum[] testRooms, bool isEnd, int maxBottles, Action<GameObject[]> OnBottles)
        {
            gameObject.SetActive(true);
            var currentBottles = maxBottles;
            foreach (var room in _roomEnds)
            {
                RoomEnum type;
                if (isStart)
                {
                    type = RoomEnum.Enemy;
                    currentBottles = 1;
                }
                else if (isEnd)
                {
                    type = RoomEnum.Boss;
                    currentBottles = 3;
                }
                else
                {
                    var maxCount = Enum.GetValues(typeof(RoomEnum)).Length - 1;
                    type = (RoomEnum)Random.Range(0, maxCount);
                }
                
                //TEST
                if (GameController.Step<testRooms.Length)
                {
                    type = testRooms[GameController.Step];
                    currentBottles = 1;
                }
                
                var bottles = GenerateBottle(currentBottles);
                OnBottles?.Invoke(bottles.ToArray());
                room.Setup(type, currentBottles);
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