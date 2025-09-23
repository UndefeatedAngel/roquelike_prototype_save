using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using BlockoutRoguelike.Core.Person.Models.Items;
using BlockoutRoguelike.Core.Person.UI.Presenters;

namespace BlockoutRoguelike.Core.Person.UI.Views
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField] private Image _healthFillImage;
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private bool _useAnimation = true;
        
        private HealthPresenter _presenter;
        private float _targetFillAmount;
        private int _currentDisplayHealth;
        private int _targetHealth;
        private int _targetMaxHealth;
        private Coroutine _animationCoroutine;
        
        private Health _healthModel;
        private bool _skipNextAnimation;

        public void Initialize(Health healthModel, bool skipInitialAnimation = true)
        {
            _healthModel = healthModel;
            _skipNextAnimation = skipInitialAnimation;
            
            InitializePresenter();
        }

        private void InitializePresenter()
        {
            _presenter?.Disable();
            _presenter = new HealthPresenter(_healthModel, this);
            _presenter.Enable();
        }

        public void UpdateHealthDisplay(int current, int max)
        {
            _targetHealth = current;
            _targetMaxHealth = max;
            _targetFillAmount = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;
            
            if (!_useAnimation || _skipNextAnimation)
            {
                _skipNextAnimation = false;
                _currentDisplayHealth = current;
                if (_healthFillImage != null)
                {
                    _healthFillImage.fillAmount = _targetFillAmount;
                }
                UpdateTextImmediately();
            }
            else
            {
                if (_animationCoroutine != null)
                    StopCoroutine(_animationCoroutine);
                
                _animationCoroutine = StartCoroutine(AnimateHealthChange());
            }
        }

        private IEnumerator AnimateHealthChange()
        {
            int startHealth = _currentDisplayHealth;
            float startFill = _healthFillImage != null ? _healthFillImage.fillAmount : 0f;
            float elapsed = 0f;
            
            while (elapsed < _animationDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / _animationDuration);
                
                float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
                
                _currentDisplayHealth = (int)Mathf.Lerp(startHealth, _targetHealth, easedProgress);
                
                if (_healthFillImage != null)
                {
                    _healthFillImage.fillAmount = Mathf.Lerp(startFill, _targetFillAmount, easedProgress);
                }
                
                UpdateTextImmediately();
                yield return null;
            }
            
            _currentDisplayHealth = _targetHealth;
            if (_healthFillImage != null)
            {
                _healthFillImage.fillAmount = _targetFillAmount;
            }
            UpdateTextImmediately();
            
            _animationCoroutine = null;
        }

        private void UpdateTextImmediately()
        {
            if (_healthText != null)
            {
                _healthText.text = $"{_currentDisplayHealth}/{_targetMaxHealth}";
            }
        }

        private void OnEnable()
        {
            if (_healthModel != null)
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