using System;
using BlockoutRoguelike.Core.Rune;
using BlockoutRoguelike.Core.UI.Inventory;
using StarterAssets;
using UnityEngine;

namespace BlockoutRoguelike.Core.Inventory
{
    public class InventoryController: MonoBehaviour
    {
        [SerializeField] private GameInventoryView _gameInventoryView;
        
        [SerializeField] private ThirdPersonController _thirdPersonController;
        [SerializeField] private LobbyUI lobbyUI;
        [SerializeField] private RuneSettings _runeSettings;

        private bool _isInventoryShow;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L) && !IsInventoryShow())
            {
                _thirdPersonController.LockCameraPosition = true;
                ShowUI(() =>
                {
                    _thirdPersonController.LockCameraPosition = false;
                });
            }

            if (Input.GetKeyDown(KeyCode.Escape) && IsInventoryShow())
            {
                HideUI();
            }
        }

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