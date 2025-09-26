using _Game.Scripts.Modules;
using BlockoutRoguelike.Core.Inventory;
using BlockoutRoguelike.Core.Inventory.InventorySystem.UI;
using BlockoutRoguelike.Core.PlayerController;
using BlockoutRoguelike.Modules;
using BlockoutRoguelike.Quests;
using Modules.SceneLoader;
using UnityEngine;

namespace _Game.Scripts.Enviroments
{
    public class ChestController: MonoBehaviour
    {
        [SerializeField] private AtomicStyleIndicator[] _atomicStyleIndicators;
        [SerializeField] private TriggerObject _exitSceneTrigger;
        [SerializeField] private InventoryController _inventoryController;
        [SerializeField] private SimpleChest _simpleChest;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private QuestController _questController;
        
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _inventoryController.Init();
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
        
        private void OnCurrentQuestTargets(int goalNumber, Transform[] objects)
        {
            if (goalNumber != 3) return;
            //Clear
            foreach (var indicator in _atomicStyleIndicators)
            {
                indicator.target = null;
            }
            
            for (int i = 0; i < objects.Length; i++)
            {
                var indicator = _atomicStyleIndicators[i];
                indicator.target = objects[i].transform;
            }
        }
    }
}