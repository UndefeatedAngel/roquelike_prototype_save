using BlockoutRoguelike.Core.Inventory;
using UnityEngine;

namespace _Game.Scripts.Enviroments
{
    public class ChestController: MonoBehaviour
    {
        [SerializeField] private InventoryController _inventoryController;
        
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _inventoryController.Init();
        }
    }
}