using UnityEngine;
using Random = UnityEngine.Random;

namespace BlockoutRoguelike
{
    public class GameController: MonoBehaviour
    {
        [SerializeField] private Environment[] _enviroments;
        [SerializeField] private int _maxSteps = 13;
        private static int _step = 0;
        private int _maxGames = 2;

        private void Start()
        {
            if (PlayerPrefs.GetInt("Restart") == _maxGames)
            {
                PlayerPrefs.SetInt("Restart", 0);
                _step = 0;
            }
            GenerateRoomsRandom();
        }

        private int MaxCountRooms()
        {
            return _enviroments.Length;
        }
        
        private void GenerateRoomsRandom()
        {
            var maxBottles = Random.Range(1, 4);
            
            if (_step >= _maxSteps)
            {
                _step = 0;
            }
            
            if (_step == 0)
            {
                _enviroments[0].Setup(true, false, maxBottles);
            }
            else if (_step == _maxSteps-1)
            {
                _enviroments[0].Setup(false, true, maxBottles);
            }
            else
            {
                var countRooms = Random.Range(0, MaxCountRooms());
                for (var i = 0; i < MaxCountRooms(); i++)
                {
                    if (i == countRooms)
                    {
                        _enviroments[i].Setup(false, false, maxBottles);
                    }
                }
            }

            _step++;
        }
    }
}