using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.Modules.SceneLoader
{
    public class UILoader: MonoBehaviour
    {
        [SerializeField] private GraphicRaycaster _graphicRaycaster;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Slider _slider;
        [SerializeField] private float sliderSpeed;
        [SerializeField] private float fadeSpeed;

        private float _startProgress = 0f;
        private float _targetProgress = 0f;

        public void ClearProgress()
        {
            _startProgress = 0f;
            _slider.value = 0f;
        }
        
        public IEnumerator UpdateSlider(float progress)
        {
            _targetProgress = progress;
            while (_startProgress < _targetProgress - 0.01f)
            {
                _startProgress = Mathf.Lerp(_startProgress, _targetProgress, Time.deltaTime * sliderSpeed);
                UpdateUIElements();
                yield return new WaitForEndOfFrame();
            }
        }
    
        private void UpdateUIElements()
        {
            if (_slider != null) _slider.value = _startProgress;
        }
        
        public IEnumerator ShowLoadingScreen()
        {
            _canvasGroup.gameObject.SetActive(true);
            _canvasGroup.alpha = 0f;
        
            while (_canvasGroup.alpha < 1f)
            {
                _canvasGroup.alpha += fadeSpeed * Time.deltaTime;
                yield return null;
            }
        
            _canvasGroup.alpha = 1f;
        }
    
        public IEnumerator HideLoadingScreen()
        {
            {
                _canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
                yield return null;
            }
            
            _canvasGroup.gameObject.SetActive(false);
        }
    }
}