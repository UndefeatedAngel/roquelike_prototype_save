using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;

namespace Modules.SceneLoader
{
    public class TipLoader : MonoBehaviour
    {
        [Header("UI Components")] 
        [SerializeField] private TextMeshProUGUI titleTextUI;
        [SerializeField] private TextMeshProUGUI descriptionTextUI;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image _tipImage;
        [Header("Tips Data")] 
        [SerializeField] private List<string> tipsList;
        [SerializeField] private List<Sprite> tipsListSprites;
        [SerializeField] private string fixedTitle = "Совет Боевого Алхимика";
        [Header("Animation Settings")] 
        [SerializeField] private float tipChangeInterval = 4f;
        [SerializeField] private float fadeDuration = 2f;

        private int currentTipIndex = -1;
        private CancellationTokenSource _tipsCts;

        private void Start()
        {
            titleTextUI.text = fixedTitle;
            StartTipsCycle().Forget();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ForceNextTip().Forget();
            }
        }

        private void OnDestroy()
        {
            _tipsCts?.Cancel();
            _tipsCts?.Dispose();
        }

        private async UniTaskVoid StartTipsCycle()
        {
            _tipsCts = new CancellationTokenSource();
            var token = _tipsCts.Token;

            await ShowTipImmediate(token);

            while (!token.IsCancellationRequested)
            {
                await UniTask.WaitForSeconds(tipChangeInterval, cancellationToken: token);

                await SwitchTipSmooth(token);
            }
        }

        private async UniTask ShowTipImmediate(CancellationToken token)
        {
            SetRandomTip();
            await FadeTip(1f, token);
        }

        private async UniTask SwitchTipSmooth(CancellationToken token)
        {
            await FadeTip(0f, token);

            SetRandomTip();

            await FadeTip(1f, token);
        }

        private void SetRandomTip()
        {
            if (tipsList == null || tipsList.Count == 0)
            {
                Debug.LogWarning("Tips list is empty!");
                return;
            }

            int newTipIndex;
            do
            {
                newTipIndex = Random.Range(0, tipsList.Count);
            } while (newTipIndex == currentTipIndex && tipsList.Count > 1);

            currentTipIndex = newTipIndex;
            descriptionTextUI.text = tipsList[currentTipIndex];
            _tipImage.sprite = tipsListSprites[currentTipIndex];
        }

        private async UniTask FadeTip(float targetAlpha, CancellationToken token)
        {
            var startAlpha = canvasGroup.alpha;
            var time = 0f;

            while (time < fadeDuration && !token.IsCancellationRequested)
            {
                time += Time.deltaTime;
                var t = Mathf.SmoothStep(0f, 1f, time / fadeDuration);
                var alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                canvasGroup.alpha = alpha;
                _tipImage.color = new Color(1f, 1f, 1f, alpha);
                await UniTask.Yield(token);
            }

            if (!token.IsCancellationRequested)
            {
                canvasGroup.alpha = targetAlpha;
                _tipImage.color = new Color(1f, 1f, 1f, targetAlpha);
            }
        }

        private async UniTaskVoid ForceNextTip()
        {
            if (_tipsCts == null || _tipsCts.Token.IsCancellationRequested)
                return;

            _tipsCts.Cancel();
            _tipsCts.Dispose();

            _tipsCts = new CancellationTokenSource();
            var token = _tipsCts.Token;
            canvasGroup.alpha = 0f;
            _tipImage.color = new Color(1f, 1f, 1f, 0f);
            SetRandomTip();
            await FadeTip(1f, token);
            await FadeTip(0f, token);
            StartTipsCycle().Forget();
        }
    }
}