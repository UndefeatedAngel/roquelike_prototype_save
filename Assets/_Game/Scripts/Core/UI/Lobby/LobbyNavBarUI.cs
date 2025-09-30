using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace NothingAwaits.Core.UI.Lobby
{
    public class LobbyNavBarUI : MonoBehaviour
    {
        [SerializeField] private Button[] _navButtons;
        
        [Header("Button Colors")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _selectedColor = Color.black;
        [SerializeField] private Color _textNormalColor = Color.black;
        [SerializeField] private Color _textSelectedColor = Color.white;

        private LobbyUI _lobbyUI;
        private int _currentSelectedIndex = -1;

        public void Init(LobbyUI lobbyUI)
        {
            _lobbyUI = lobbyUI;

            // Проверяем что количество кнопок соответствует enum
            if (_navButtons.Length != Enum.GetValues(typeof(WindowType)).Length)
            {
                Debug.LogError("Количество кнопок не соответствует количеству окон!");
                return;
            }

            // Назначаем обработчики для всех кнопок
            for (int i = 0; i < _navButtons.Length; i++)
            {
                int buttonIndex = i; // Важно для замыкания
                _navButtons[i].onClick.AddListener(() => OnNavButtonClicked(buttonIndex));
            }

            // Устанавливаем начальное выделение для профиля
            SetSelectedButton((int)WindowType.Profile);
        }

        private void OnNavButtonClicked(int buttonIndex)
        {
            WindowType windowType = (WindowType)buttonIndex;
            _lobbyUI?.ShowWindow(windowType);
            SetSelectedButton(buttonIndex);
        }

        private void SetSelectedButton(int buttonIndex)
        {
            // Проверяем валидность индекса
            if (buttonIndex < 0 || buttonIndex >= _navButtons.Length)
                return;

            // Сбрасываем цвет предыдущей кнопки
            if (_currentSelectedIndex >= 0 && _currentSelectedIndex < _navButtons.Length)
            {
                ResetButtonColor(_currentSelectedIndex);
            }

            // Устанавливаем цвет для новой выбранной кнопки
            _currentSelectedIndex = buttonIndex;
            SetButtonSelectedColor(buttonIndex);
        }

        private void ResetButtonColor(int buttonIndex)
        {
            var image = _navButtons[buttonIndex].GetComponent<Image>();
            if (image != null)
            {
                image.color = _normalColor;
            }

            // Сбрасываем цвет текста
            var text = _navButtons[buttonIndex].GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                text.color = _textNormalColor;
            }
        }

        private void SetButtonSelectedColor(int buttonIndex)
        {
            var image = _navButtons[buttonIndex].GetComponent<Image>();
            if (image != null)
            {
                image.color = _selectedColor;
            }

            // Устанавливаем цвет текста на белый
            var text = _navButtons[buttonIndex].GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                text.color = _textSelectedColor;
            }
        }

        // Метод для внешнего вызова (например, при открытии окна через код)
        public void SetActiveWindow(WindowType windowType)
        {
            SetSelectedButton((int)windowType);
        }
    }
}