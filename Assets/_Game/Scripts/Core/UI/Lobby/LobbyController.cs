using NothingAwaits.Core.Player;
using UnityEngine;

namespace NothingAwaits.Core.UI.Lobby
{
    public class LobbyController : MonoBehaviour
    {
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private LobbyUI _lobbyUI;
        [SerializeField] private PlayerController _playerController;

        private bool _isShow;
        
        public void Init()
        {
            _lobbyUI.Init(_playerController); // Передаем PlayerController
            _lobbyUI.OnHide = OnLobbyHide;
        }

        private void Update()
        {
            if (!IsInventoryShow())
            {
                if (Input.GetKeyDown(KeyCode.L))
                {
                    ShowUI();
                }
                else if (Input.GetKeyDown(KeyCode.I))
                {
                    ShowUI(WindowType.Inventory);
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    ShowUI(WindowType.Runes);
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) && IsInventoryShow())
            {
                _lobbyUI.Hide();
            }
        }

        public bool IsInventoryShow()
        {
            return _isShow;
        }

        private void ShowUI(WindowType type = WindowType.Profile)
        {
            _isShow = true;
            _lobbyUI.Show(type);
            
            if (_playerInput != null)
            {
                _playerInput.DisablePlayerControl();
            }
        }

        private void OnLobbyHide()
        {
            _isShow = false;
            
            if (_playerInput != null)
            {
                _playerInput.EnablePlayerControl();
            }
        }
    }
}