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
            if (Input.GetKeyDown(KeyCode.L) && !IsInventoryShow())
            {
                ShowUI();
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

        private void ShowUI()
        {
            _isShow = true;
            _lobbyUI.Show();
            
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