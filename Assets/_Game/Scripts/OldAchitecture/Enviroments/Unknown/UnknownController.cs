using _Game.Scripts.Modules;
using Modules.SceneLoader;
using NothingAwaits.Core.Player;
using NothingAwaits.Modules;
using NothingAwaits.Quests;
using UnityEngine;

namespace _Game.Scripts.OldAchitecture.Enviroments
{
    public class UnknownController: MonoBehaviour
    {
        [SerializeField] private UnknownView[] _unknownViews;
        [SerializeField] private AtomicStyleIndicator[] _atomicStyleIndicators;
        [SerializeField] private TriggerObject _exitSceneTrigger;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private QuestController _questController;
        
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _exitSceneTrigger.OnEnter = ExitScene;

            var array = GetShuffledKeysArray();
            for (var i = 0; i < array.Length; i++)
            {
                _unknownViews[i].SetView(array[i], OnBuff, OnDebuff);
            }
            
            _questController.Setup();
            _questController.OnCurrentQuestTargets += OnCurrentQuestTargets;
            _questController.StartQuest();
        }
        
        private void OnBuff()
        {
            _playerController.FullHeal();
            _playerController.AddCoins(100);
        }

        private void OnDebuff()
        {
            _playerController.TakeDamage(_playerController.PlayerModel.Health.Current/2);
            if (!_playerController.TrySpendCoins(100))
            {
                _playerController.TrySpendCoins(_playerController.PlayerModel.Coins.Amount);
            }
        }

        public UnknownType[] GetShuffledKeysArray()
        {
            var array = (UnknownType[])System.Enum.GetValues(typeof(UnknownType));
            var rng = new System.Random();
    
            for (var i = array.Length - 1; i > 0; i--)
            {
                var randomIndex = rng.Next(i + 1);
                (array[i], array[randomIndex]) = (array[randomIndex], array[i]);
            }
    
            return array;
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