using System;
using BlockoutRoguelike.Core.Rune;
using BlockoutRoguelike.Core.UI.Inventory;
using UnityEngine;

namespace BlockoutRoguelike.Core
{
    public class InventoryController: MonoBehaviour
    {
        [SerializeField] private LobbyUI lobbyUI;
        [SerializeField] private RuneSettings _runeSettings;

        private bool _isInventoryShow;
        public void Init()
        {
            lobbyUI.Init(_runeSettings);
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
            lobbyUI.Show();
            lobbyUI.OnHide = () =>
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