using System;
using UnityEngine;
using UnityEngine.UI;

namespace NothingAwaits.Core.UI.EscMenu
{
    public class EscUI: MonoBehaviour
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitMainButton;
        [SerializeField] private Button _exitGameButton;

        public Action<EscMenuType> onExitMenuClicked;

        public void Init()
        {
            _continueButton.onClick.AddListener(() =>
            {
                onExitMenuClicked?.Invoke(EscMenuType.Continue);
            });
            _settingsButton.onClick.AddListener(() =>
            {
                onExitMenuClicked?.Invoke(EscMenuType.Settings);
            });
            _exitMainButton.onClick.AddListener(() =>
            {
                onExitMenuClicked?.Invoke(EscMenuType.ExitMain);
            });
            _exitGameButton.onClick.AddListener(() =>
            {
                onExitMenuClicked?.Invoke(EscMenuType.ExitGame);
            });
        }

        private void OnEnable()
        {
            ResetAllButtons();
        }

        private void ResetAllButtons()
        {
            var buttons = GetComponentsInChildren<Button>(true);
            foreach (var button in buttons)
            {
                button.OnDeselect(null);
                if (button.gameObject.activeInHierarchy)
                {
                    button.OnPointerExit(new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current));
                }
            }
            if (UnityEngine.EventSystems.EventSystem.current != null)
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            }
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}