using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NothingAwaits.Cards
{
    public class BenchItemUI: MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _text;
        
        private Tween _fadeTween;

        public void Set(Sprite sprite, string text)
        {
            _image.sprite = sprite;
            _text.text = text;
            _fadeTween?.Kill();
            var shouldShow = sprite != null;
            
            if (shouldShow)
            {
                _canvasGroup.alpha = 1f;
                _image.enabled = true;
                _text.enabled = true;
            }
            else
            {
                _fadeTween = _canvasGroup.DOFade(0f, 0.25f)
                    .OnComplete(() => 
                    {
                        _image.enabled = false;
                        _text.enabled = false;
                    });
            }
        }
    }
}