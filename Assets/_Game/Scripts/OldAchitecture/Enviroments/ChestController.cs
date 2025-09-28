using _Game.Scripts.Modules;
using NothingAwaits.Core.Inventory;
using NothingAwaits.Core.Inventory.InventorySystem.UI;
using NothingAwaits.Core.Player;
using NothingAwaits.Modules;
using NothingAwaits.Quests;
using Modules.SceneLoader;
using UnityEngine;

namespace _Game.Scripts.Enviroments
{
    public class ChestController: MonoBehaviour
    {
        [SerializeField] private AtomicStyleIndicator[] _atomicStyleIndicators;
        [SerializeField] private TriggerObject _exitSceneTrigger;
        [SerializeField] private SimpleChest _simpleChest;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private QuestController _questController;
        
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _exitSceneTrigger.OnEnter = ExitScene;

            _simpleChest.OnCoinEject = () =>
            {
                _playerController.AddCoins(100);
            };
            
            _questController.Setup();
            _questController.OnCurrentQuestTargets += OnCurrentQuestTargets;
            _questController.StartQuest();
        }

        private void ExitScene()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneType.GamePlay, () =>
            {
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