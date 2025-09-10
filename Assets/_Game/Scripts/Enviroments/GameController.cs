using _Game.Scripts;
using BlockoutRoguelike.Static;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace BlockoutRoguelike
{
    public class GameController: MonoBehaviour
    {
        [SerializeField] private TMP_Text _targetText;
        [SerializeField] private Environment[] _enviroments;
        [SerializeField] private Transform _enviromentsContainer;
        [SerializeField] private int _maxSteps = 13;
        [Header("Controllers")]
        [SerializeField] private SkinController _skinController;
        
        private static int _step = 0;
        private int _maxGames = 2;

        private void Start()
        {
            if (_step >= _maxSteps)
            {
                _step = 0;
            }
            Restart();
            SetupSkin();
            GenerateRoomsRandom();
        }

        private void Update()
        {
            #if DEV_DEBUG
            if (Input.GetKeyDown(KeyCode.N))
            {
                SceneLoadManager.LoadScene(SceneName.GamePlay);
            }
            #endif
        }

        private void Restart()
        {
            if (PlayerPrefs.GetInt("Restart") == _maxGames)
            {
                PlayerPrefs.SetInt("Restart", 0);
                _step = 0;
            }
        }

        private void SetupSkin()
        {
            var stepType = EnviromentType.Random;
            if (_step == 0)
            {
                stepType = EnviromentType.First;
            }
            else if (_step == _maxSteps - 1)
            {
                stepType = EnviromentType.Boss;
            }
            
            Debug.Log($"Step: {_step}");
            _skinController.Setup(stepType);
        }

        private int MaxCountRooms()
        {
            return _enviroments.Length;
        }
        
        private void GenerateRoomsRandom()
        {
            var maxBottles = Random.Range(1, 4);
            
            if (_step == 0)
            {
                CreateEnvironment(0,true, false, maxBottles);
            }
            else if (_step == _maxSteps-1)
            {
                CreateEnvironment(0,false, true, maxBottles);
            }
            else
            {
                var countRooms = Random.Range(0, MaxCountRooms());
                for (var i = 0; i < MaxCountRooms(); i++)
                {
                    if (i == countRooms)
                    {
                        CreateEnvironment(i,false, false, maxBottles);
                    } 
                }
            }

            _step++;
        }

        private void CreateEnvironment(int number, bool isStart, bool isEnd, int maxBottles)
        {
            var env = Instantiate(_enviroments[number], _enviromentsContainer, true);
            env.Setup(isStart, isEnd, maxBottles);
            var post = maxBottles switch
            {
                1  => "ку",
                2 or 3 or 4 => "ки",
                _ => "ок"
            };
            _targetText.text = $"Собери {maxBottles} бутыл{post} чтобы пройти дальше!";
        }
    }
}