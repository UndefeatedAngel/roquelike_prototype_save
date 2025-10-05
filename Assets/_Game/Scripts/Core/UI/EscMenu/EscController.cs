using System;
using Modules.SceneLoader;
using NothingAwaits.Core.Player;
using UnityEngine;

namespace NothingAwaits.Core.UI.EscMenu
{
    public enum EscMenuType
    {
        Continue,
        Settings, 
        ExitMain,
        ExitGame
    }
    public class EscController: MonoBehaviour
    {
        [SerializeField] private EscUI _escUI;
        
        private PlayerInput _playerInput;
        
        private bool _isShow;

        private void Awake()
        {
            _escUI = Instantiate(_escUI, transform);
            _escUI.Init();
            _escUI.Hide();
            _escUI.onExitMenuClicked = ExitMenuClicked;
        }

        private void Start()
        {
            _playerInput = FindObjectOfType<PlayerInput>();
        }

        private void ExitMenuClicked(EscMenuType type)
        {
            switch (type)
            {
                case EscMenuType.Continue:
                    Hide();
                    break;
                case EscMenuType.ExitMain:
                    Hide();
                    if (_playerInput != null)
                    {
                        _playerInput.DisablePlayerControl();
                    }
                    SceneLoader.Instance.LoadSceneAsync(SceneType.Main, null, true);
                    break;
                case EscMenuType.ExitGame:
                    Application.Quit();
                    break;
            }
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!Input.GetKeyDown(KeyCode.P)) return;
#else
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
#endif
            if (!_isShow)
            {
                if (_playerInput != null)
                {
                    _playerInput.DisablePlayerControl();
                }
                _escUI.Show();
                _isShow = true;
            }
            else
            {
                Hide();
            }
        }

        private void Hide()
        {
            _escUI.Hide();
            if (_playerInput != null)
            {
                _playerInput.EnablePlayerControl();
            }

            _isShow = false;
        }
    }
}