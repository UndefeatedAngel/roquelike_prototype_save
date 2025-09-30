using UnityEngine;
using System;

namespace NothingAwaits.Core.UI.Lobby.Windows
{
    public class BaseLobbyWindow : MonoBehaviour
    {
        [SerializeField] private bool _isOpenOnStart = false;
        
        public Action OnWindowOpen;
        public Action OnWindowClose;

        protected virtual void Start()
        {
            if (_isOpenOnStart)
            {
                OpenWindow();
            }
            else
            {
                CloseWindow();
            }
        }

        public virtual void OpenWindow()
        {
            gameObject.SetActive(true);
            OnWindowOpen?.Invoke();
            OnWindowOpened();
        }

        public virtual void CloseWindow()
        {
            gameObject.SetActive(false);
            OnWindowClose?.Invoke();
            OnWindowClosed();
        }

        public virtual void ToggleWindow()
        {
            if (gameObject.activeSelf)
            {
                CloseWindow();
            }
            else
            {
                OpenWindow();
            }
        }

        protected virtual void OnWindowOpened()
        {
        }

        protected virtual void OnWindowClosed()
        {
        }
    }
}