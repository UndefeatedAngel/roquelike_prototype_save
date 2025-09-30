using TMPro;
using UnityEngine;
using System.Collections;

namespace NothingAwaits.Core.UI
{
    public class AnimateText : MonoBehaviour
    {
        [Header("Text Reference")]
        [SerializeField] private TMP_Text _targetText;

        [Header("Animation Settings")]
        [SerializeField] private float _animationDuration = 5f;
        [SerializeField] private bool _playOnStart = false;
        [SerializeField] private string _finalText = "Hello World!";

        private Coroutine _animationCoroutine;

        private void Start()
        {
            if (_playOnStart)
            {
                StartAnimation(_finalText, _animationDuration);
            }
        }

        public void StartAnimation(string targetText, float duration = -1)
        {
            // Останавливаем предыдущую анимацию если есть
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }

            // Используем переданную длительность или настройку по умолчанию
            float actualDuration = duration >= 0 ? duration : _animationDuration;
            
            _animationCoroutine = StartCoroutine(AnimateTextCoroutine(targetText, actualDuration));
        }

        public void StartAnimationWithDefaultText(float duration = -1)
        {
            StartAnimation(_finalText, duration);
        }

        public void StopAnimation()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
                _animationCoroutine = null;
            }
        }

        public void SetFinalTextInstantly(string text)
        {
            StopAnimation();
            if (_targetText != null)
            {
                _targetText.text = text;
            }
        }

        private IEnumerator AnimateTextCoroutine(string targetText, float duration)
        {
            if (_targetText == null || string.IsNullOrEmpty(targetText))
                yield break;

            _targetText.text = "";
            int totalCharacters = targetText.Length;
            float timePerCharacter = duration / totalCharacters;
            float elapsedTime = 0f;

            for (int i = 0; i <= totalCharacters; i++)
            {
                // Устанавливаем текущий текст
                _targetText.text = targetText.Substring(0, i);
                
                // Ждем время для следующего символа
                elapsedTime += timePerCharacter;
                float waitTime = timePerCharacter;
                
                // Корректируем время ожидания для последней итерации
                if (i == totalCharacters - 1)
                {
                    waitTime = duration - elapsedTime + timePerCharacter;
                }

                yield return new WaitForSeconds(waitTime);
            }

            // Убеждаемся что текст установлен полностью
            _targetText.text = targetText;
            _animationCoroutine = null;
        }

        // Методы для настройки параметров
        public void SetTargetText(TMP_Text textComponent)
        {
            _targetText = textComponent;
        }

        public void SetAnimationDuration(float duration)
        {
            _animationDuration = duration;
        }

        public void SetFinalText(string text)
        {
            _finalText = text;
        }

        private void OnDestroy()
        {
            StopAnimation();
        }
    }
}