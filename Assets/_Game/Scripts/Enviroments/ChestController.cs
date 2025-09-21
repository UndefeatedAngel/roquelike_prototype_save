using _Game.Scripts.Modules;
using BlockoutRoguelike.Core.Inventory;
using UnityEngine;

namespace _Game.Scripts.Enviroments
{
    public class ChestController: MonoBehaviour
    {
        [SerializeField] private TriggerObject _exitSceneTrigger;
        [SerializeField] private InventoryController _inventoryController;
        
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _inventoryController.Init();
            _exitSceneTrigger.OnEnter = ExitScene;
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