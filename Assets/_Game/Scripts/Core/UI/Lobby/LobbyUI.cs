using System;
using NothingAwaits.Core.Player;
using UnityEngine;
using UnityEngine.UI;

namespace NothingAwaits.Core.UI.Lobby
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private LobbyNavBarUI _navBarUI;
        
        [Header("Windows")]
        [SerializeField] private Windows.BaseLobbyWindow[] _windows;

        [Header("Specific Windows")]
        [SerializeField] private Windows.CardsWindowUI _cardsWindowUI;

        public Action OnHide;

        private PlayerController _playerController;

        public void Init(PlayerController playerController)
        {
            _playerController = playerController;
            _closeButton.onClick.AddListener(OnCloseButtonClicked);
            _navBarUI.Init(this);
            
            HideAllWindows();
        }

        public void Show(WindowType type = WindowType.Profile)
        {
            gameObject.SetActive(true);
            ShowWindow(type);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            OnHide?.Invoke();
        }

        private void OnCloseButtonClicked()
        {
            Hide();
        }

        public void ShowWindow(WindowType windowType)
        {
            HideAllWindows();

            int windowIndex = (int)windowType;
            if (windowIndex >= 0 && windowIndex < _windows.Length)
            {
                _windows[windowIndex].OpenWindow();
                
                if (windowType == WindowType.Cards && _cardsWindowUI != null && _playerController != null)
                {
                    _cardsWindowUI.SetPlayerCards(_playerController.GetPlayerCards());
                }
            }
            else
            {
                Debug.LogError($"Окно с индексом {windowIndex} не найдено!");
            }

            _navBarUI.SetActiveWindow(windowType);
        }

        private void HideAllWindows()
        {
            foreach (var window in _windows)
            {
                if (window != null)
                {
                    window.CloseWindow();
                }
            }
        }
    }
}