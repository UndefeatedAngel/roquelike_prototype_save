using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace NothingAwaits.Core.Coins
{
    public class CoinsView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text _coinsText;
        
        [Header("Animation")]
        [SerializeField] private float _animationSpeed = 2f;
        [SerializeField] private bool _useAnimation = true;
        
        private CoinsPresenter _presenter;
        private CancellationTokenSource _animationCts;
        private int _currentDisplayCoins;

        public void Initialize(CoinsModel coinsModel, bool skipInitialAnimation = true)
        {
            _presenter?.Disable();
            _presenter = new CoinsPresenter(coinsModel, this);
            _presenter.Enable();
            
            if (skipInitialAnimation)
            {
                UpdateCoinsDisplay(coinsModel.Amount);
            }
        }

        public void UpdateCoinsDisplay(int amount)
        {
            _animationCts?.Cancel();
            _animationCts?.Dispose();
            _animationCts = new CancellationTokenSource();

            if (!_useAnimation || _animationSpeed <= 0)
            {
                ApplyDisplay(amount);
            }
            else
            {
                AnimateToValueAsync(amount, _animationCts.Token).Forget();
            }
        }

        private void ApplyDisplay(int amount)
        {
            _currentDisplayCoins = amount;
            UpdateUIText(amount.ToString());
        }

        private void UpdateUIText(string text)
        {
            if (_coinsText != null)
                _coinsText.text = text;
        }

        private async UniTaskVoid AnimateToValueAsync(int targetAmount, CancellationToken token)
        {
            var startAmount = _currentDisplayCoins;
            var duration = Mathf.Abs(targetAmount - startAmount) / _animationSpeed;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                if (token.IsCancellationRequested)
                    return;

                elapsed += Time.deltaTime;
                var progress = Mathf.Clamp01(elapsed / duration);
                var smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

                _currentDisplayCoins = (int)Mathf.Lerp(startAmount, targetAmount, smoothProgress);
                UpdateUIText(_currentDisplayCoins.ToString());

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            ApplyDisplay(targetAmount);
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