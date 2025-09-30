using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.SceneLoader
{
    public class UILoader: MonoBehaviour
    {
        [SerializeField] private CanvasGroup _spaceCanvas;
        [SerializeField] private GraphicRaycaster _graphicRaycaster;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Slider _slider;
        [SerializeField] private float sliderSpeed;
        [SerializeField] private float fadeSpeed;

        private float _startProgress;
        private float _targetProgress;

        private bool _isActiveSpace;
        private bool _isSpaceClick;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_isActiveSpace && Input.GetKeyDown(KeyCode.Space))
            {
                _isActiveSpace = false;
                _isSpaceClick = true;
            }
        }

        public void ActivateSpace()
        {
            _spaceCanvas.DOFade(1f, 0.5f).OnComplete(() =>
            {
                _isActiveSpace = true;
            });
        }

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
        
        public IEnumerator FullSlider()
        {
            _startProgress = _targetProgress;
            UpdateUIElements();
            yield return new WaitForEndOfFrame();
        }
    
        private void UpdateUIElements()
        {
            if (_slider != null) _slider.value = _startProgress;
        }
        
        public IEnumerator ShowLoadingScreen()
        {
            _canvasGroup.gameObject.SetActive(true);
            _canvasGroup.alpha = 0f;
            _spaceCanvas.alpha = 0f;
            _isActiveSpace = false;
            _isSpaceClick = false;
        
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

        public IEnumerator SpaceCondition()
        {
            yield return new WaitUntil(() => _isSpaceClick);
        }
    }
}