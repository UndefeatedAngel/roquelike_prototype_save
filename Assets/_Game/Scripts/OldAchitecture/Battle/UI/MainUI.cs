using System;
using DG.Tweening;
using NothingAwaits.Battle.Controllers;
using TMPro;
using UnityEngine;

namespace NothingAwaits.Cards.UI
{
    public class MainUI: MonoBehaviour
    {
        [SerializeField] private RectTransform _coinRectTransform;
        [SerializeField] private TMP_Text _coinsText;
        [SerializeField] private TMP_Text[] _statsText;
        [SerializeField] private TMP_Text[] _bloodText;

        private Tween _coinUpAnimationTween;
        
        public void UpdateCoins(int count)
        {
            _coinsText.text = count.ToString();
        }
        
        public void UpdateStat(PlayerCardType type, int count, int maxCount)
        {
            var number = (int)type - 1;
            _statsText[number].text = $"{count}/{maxCount}";
        }

        public void UpdateBlood(int count, int maxCount)
        {
            foreach (var text in _bloodText)
            {
                text.text = $"{count}/{maxCount}";
            }
        }

        public void FlyCoin(RectTransform rectTransform, Action endFly)
        {
            rectTransform.DOJump(new Vector3(-7.4f, 2.85f, 2f), 0.25f, 1, 0.7f).OnComplete(()=>
            {
                _coinUpAnimationTween?.Kill();
                _coinUpAnimationTween = _coinRectTransform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.05f).OnComplete(() =>
                {
                    _coinRectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.05f);
                });
                endFly?.Invoke();
            });
        }
    }
}