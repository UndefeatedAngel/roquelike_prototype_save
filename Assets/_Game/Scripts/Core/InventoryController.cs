using System;
using BlockoutRoguelike.Core.Rune;
using UnityEngine;

namespace BlockoutRoguelike.Core
{
    public class InventoryController: MonoBehaviour
    {
        [SerializeField] private InventoryUI _inventoryUI;
        [SerializeField] private RuneSettings _runeSettings;

        private bool _isInventoryShow;
        public void Init()
        {
            _inventoryUI.Init(_runeSettings);
        }

        public bool IsInventoryShow()
        {
            return _isInventoryShow;
        }
        
        public void ShowUI(Action onHide)
        {
            _isInventoryShow = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _inventoryUI.Show();
            _inventoryUI.OnHide = () =>
            {
                HideUI();
                onHide?.Invoke();
            };
        }

        private void HideUI()
        {
            _isInventoryShow = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}