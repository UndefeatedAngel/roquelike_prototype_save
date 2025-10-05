 using _Game.Scripts.Modules;
using Modules.SceneLoader;
using NothingAwaits.Core.Player;
using NothingAwaits.Modules;
using NothingAwaits.Quests;
using UnityEngine;

namespace _Game.Scripts.OldAchitecture.Enviroments
{
    public class RestController: MonoBehaviour
    {
        [SerializeField] private RestView _restView;
        [SerializeField] private AtomicStyleIndicator[] _atomicStyleIndicators;
        [SerializeField] private TriggerObject _exitSceneTrigger;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private QuestController _questController;
        
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _restView.SetView(OnBuff);
            _exitSceneTrigger.OnEnter = ExitScene;
            _questController.Setup();
            _questController.OnCurrentQuestTargets += OnCurrentQuestTargets;
            _questController.StartQuest();
        }
        
        private void OnBuff()
        {
            _playerController.FullHeal();
        }

        private void ExitScene()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneType.GamePlay, () =>
            {
                _playerInput.DisablePlayerControl();
                _playerController.SaveData();
                Debug.Log("Загружен!");
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