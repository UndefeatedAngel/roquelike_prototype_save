using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;
using NothingAwaits.Core.Actor.Models.Items;
using NothingAwaits.Core.Actor.UI.Presenters;

namespace NothingAwaits.Core.Actor.UI.Views
{
    public class HealthView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image _healthFillImage;
        [SerializeField] private TMP_Text _healthText;
        
        [Header("Animation")]
        [SerializeField] private float _animationSpeed = 2f;
        [SerializeField] private bool _useAnimation = true;
        
        private HealthPresenter _presenter;
        private CancellationTokenSource _animationCts;
        private int _currentDisplayHealth;
        private int _currentDisplayMaxHealth;

        public void Initialize(Health healthModel, bool skipInitialAnimation = true)
        {
            _presenter?.Disable();
            _presenter = new HealthPresenter(healthModel, this);
            _presenter.Enable();
            
            if (skipInitialAnimation)
            {
                UpdateHealthDisplay(healthModel.Current, healthModel.Max);
            }
        }

        public void UpdateHealthDisplay(int current, int max)
        {
            _animationCts?.Cancel();
            _animationCts?.Dispose();
            _animationCts = new CancellationTokenSource();

            if (!_useAnimation || _animationSpeed <= 0)
            {
                ApplyDisplay(current, max);
            }
            else
            {
                AnimateToValueAsync(current, max, _animationCts.Token).Forget();
            }
        }

        private void ApplyDisplay(int current, int max)
        {
            _currentDisplayHealth = current;
            _currentDisplayMaxHealth = max;
            
            var fillAmount = max > 0 ? (float)current / max : 0f;
            var displayText = $"{current}/{max}";
            
            UpdateUIComponents(fillAmount, displayText);
        }

        private void UpdateUIComponents(float fillAmount, string text)
        {
            if (_healthFillImage != null)
                _healthFillImage.fillAmount = fillAmount;
                
            if (_healthText != null)
                _healthText.text = text;
        }

        private async UniTaskVoid AnimateToValueAsync(int targetCurrent, int targetMax, CancellationToken token)
        {
            var startHealth = _currentDisplayHealth;
            var startMax = _currentDisplayMaxHealth;
            var startFill = _healthFillImage != null ? _healthFillImage.fillAmount : 0f;
            var targetFill = targetMax > 0 ? (float)targetCurrent / targetMax : 0f;
            
            var duration = Mathf.Abs(targetFill - startFill) / _animationSpeed;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                if (token.IsCancellationRequested)
                    return;

                elapsed += Time.deltaTime;
                var progress = Mathf.Clamp01(elapsed / duration);
                var smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

                _currentDisplayHealth = (int)Mathf.Lerp(startHealth, targetCurrent, smoothProgress);
                _currentDisplayMaxHealth = (int)Mathf.Lerp(startMax, targetMax, smoothProgress);
                
                var currentFill = Mathf.Lerp(startFill, targetFill, smoothProgress);
                var displayText = $"{_currentDisplayHealth}/{_currentDisplayMaxHealth}";
                
                UpdateUIComponents(currentFill, displayText);

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            ApplyDisplay(targetCurrent, targetMax);
        }

        private void OnEnable() => _presenter?.Enable();
        private void OnDisable() => Cleanup();
        private void OnDestroy() => Cleanup();

        private void Cleanup()
        {
            _presenter?.Disable();
            _animationCts?.Cancel();
            _animationCts?.Dispose();
            _animationCts = null;
        }
    }
}