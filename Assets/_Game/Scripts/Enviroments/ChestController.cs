using _Game.Scripts.Modules;
using BlockoutRoguelike.Core.Inventory;
using BlockoutRoguelike.Core.Inventory.InventorySystem.UI;
using BlockoutRoguelike.Core.PlayerController;
using UnityEngine;

namespace _Game.Scripts.Enviroments
{
    public class ChestController: MonoBehaviour
    {
        [SerializeField] private TriggerObject _exitSceneTrigger;
        [SerializeField] private InventoryController _inventoryController;
        [SerializeField] private SimpleChest _simpleChest;
        [SerializeField] private PlayerController _playerController;
        
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
        }

        private void ExitScene()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneType.GamePlay, () =>
            {
                Debug.Log("Загружен!");
            });
        }
    }
}