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
        public void Setup(RoomEnum typeRoom, int maxBottles, Action<GameObject[]> OnBottles)
        {
            gameObject.SetActive(true);
            foreach (var room in _roomEnds)
            {
                var maxCount = Enum.GetValues(typeof(RoomEnum)).Length - 1;
                var type = typeRoom == RoomEnum.Empty ? (RoomEnum)Random.Range(0, maxCount) : typeRoom;
                var bottles = GenerateBottle(maxBottles);
                OnBottles?.Invoke(bottles.ToArray());
                room.Setup(type, maxBottles);
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