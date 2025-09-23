using UnityEngine;
using TMPro;
using System.Collections;
using BlockoutRoguelike.Core.Person.Models.Items;
using BlockoutRoguelike.Core.Person.UI.Presenters;

namespace BlockoutRoguelike.Core.Person.UI.Views
{
    public class CoinsUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _coinsText;
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private bool _useAnimation = true;
        
        private CoinsPresenter _presenter;
        private int _currentDisplayCoins;
        private int _targetCoins;
        private Coroutine _animationCoroutine;
        
        private Coins _coinsModel;
        private bool _skipNextAnimation;

        public void Initialize(Coins coinsModel, bool skipInitialAnimation = true)
        {
            _coinsModel = coinsModel;
            _skipNextAnimation = skipInitialAnimation;
            InitializePresenter();
        }

        private void InitializePresenter()
        {
            _presenter?.Disable();
            _presenter = new CoinsPresenter(_coinsModel, this);
            _presenter.Enable();
        }

        public void UpdateCoinsDisplay(int current)
        {
            _targetCoins = current;
            
            if (!_useAnimation || _skipNextAnimation)
            {
                _skipNextAnimation = false;
                _currentDisplayCoins = current;
                UpdateTextImmediately();
            }
            else
            {
                if (_animationCoroutine != null)
                    StopCoroutine(_animationCoroutine);
                
                _animationCoroutine = StartCoroutine(AnimateCoinsChange());
            }
        }

        private IEnumerator AnimateCoinsChange()
        {
            int startCoins = _currentDisplayCoins;
            float elapsed = 0f;
            
            while (elapsed < _animationDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / _animationDuration);
                float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
                
                _currentDisplayCoins = (int)Mathf.Lerp(startCoins, _targetCoins, easedProgress);
                UpdateTextImmediately();
                yield return null;
            }
            
            _currentDisplayCoins = _targetCoins;
            UpdateTextImmediately();
            _animationCoroutine = null;
        }

        private void UpdateTextImmediately()
        {
            if (_coinsText != null)
            {
                _coinsText.text = _currentDisplayCoins.ToString();
            }
        }

        private void OnEnable()
        {
            if (_coinsModel != null)
            {
                InitializePresenter();
            }
        }

        private void OnDisable()
        {
            _presenter?.Disable();
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);
        }

        private void OnDestroy()
        {
            _presenter?.Disable();
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);
        }
    }
}