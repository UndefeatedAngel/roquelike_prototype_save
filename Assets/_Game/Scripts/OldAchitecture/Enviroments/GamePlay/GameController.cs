using Modules.SceneLoader;
using NothingAwaits.Core.Player;
using NothingAwaits.Core.UI.Lobby;
using NothingAwaits.Modules;
using NothingAwaits.Quests;
using StarterAssets;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NothingAwaits
{
    public class GameController: MonoBehaviour
    {
        [SerializeField] private RoomEnum[] _testRooms;
        [SerializeField] private TMP_Text _targetText;
        [SerializeField] private Environment[] _enviroments;
        [SerializeField] private Transform _enviromentsContainer;
        [SerializeField] private int _maxSteps = 13;
        [SerializeField] private AtomicStyleIndicator[] _atomicStyleIndicators;
        [Header("Controllers")]
        [SerializeField] private SkinController _skinController;
        [SerializeField] private LobbyController lobbyController;
        [SerializeField] private ThirdPersonController _thirdPersonController;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private QuestController _questController;
        [SerializeField] private PlayerInput _playerInput;
        
        public static int Step = 0;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _playerController.Setup();
            if (Step >= _maxSteps)
            {
                Step = 0;
            }
            Restart();
            SetupSkin();
            GenerateRoomsRandom();
            lobbyController.Init();
            _questController.Setup();
            _questController.OnCurrentQuestTargets += OnCurrentQuestTargets;
            _questController.StartQuest();
            
            _playerInput.EnablePlayerControl(true);
        }

        private void OnDestroy()
        {
            _questController.OnCurrentQuestTargets -= OnCurrentQuestTargets;
        }

        private void Update()
        {
            #if DEV_DEBUG
            if (Input.GetKeyDown(KeyCode.N))
            {
                SceneLoader.Instance.LoadScene(SceneType.GamePlay);
            }
            #endif
        }

        private void Restart()
        {
            _playerController.Reset();
        }

        private void SetupSkin()
        {
            var stepType = EnviromentType.Random;
            if (Step == 0)
            {
                stepType = EnviromentType.First;
            }
            else if (Step == _maxSteps - 1)
            {
                stepType = EnviromentType.Boss;
            }
            
            Debug.Log($"Step: {Step}");
            _skinController.Setup(stepType);
        }

        private int MaxCountRooms()
        {
            return _enviroments.Length;
        }
        
        private void GenerateRoomsRandom()
        {
            var maxBottles = Random.Range(1, 4);
            if (Step < _testRooms.Length)
            {
                var type = _testRooms[Step];
                CreateEnvironment(0, 1, type);
            }
            else
            {
                var countRooms = Random.Range(0, MaxCountRooms());
                for (var i = 0; i < MaxCountRooms(); i++)
                {
                    if (i == countRooms)
                    {
                        CreateEnvironment(i, maxBottles, RoomEnum.Empty);
                    } 
                }
            }
            
            Step++;
        }

        private void CreateEnvironment(int number, int maxBottles, RoomEnum type)
        {
            var env = Instantiate(_enviroments[number], _enviromentsContainer, true);
            env.Setup(type, maxBottles, objects =>
            {
                Debug.Log($"Bottles {objects.Length}");
                
                var post = objects.Length switch
                {
                    1  => "ку",
                    2 or 3 or 4 => "ки",
                    _ => "ок"
                };
                _targetText.text = $"Найди {objects.Length} бутыл{post} чтобы пройти дальше!";
            }, sceneType =>
            {
                _playerInput.DisablePlayerControl();
                SceneLoader.Instance.LoadSceneAsync(sceneType);
            });
        }

        private void OnCurrentQuestTargets(QuestGoal goal, Transform[] objects)
        {
            
            //Clear
            foreach (var indicator in _atomicStyleIndicators)
            {
                indicator.target = null;
            }
            
            if (goal == null || goal.HideMarkers) return;
            
            for (int i = 0; i < objects.Length; i++)
            {
                var indicator = _atomicStyleIndicators[i];
                indicator.target = objects[i].transform;
            }
        }
    }
}