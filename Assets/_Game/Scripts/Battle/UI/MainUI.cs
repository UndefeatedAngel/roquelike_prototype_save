using System;
using BlockoutRoguelike.Battle.Controllers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace BlockoutRoguelike.Cards.UI
{
    public class MainUI: MonoBehaviour
    {
        [SerializeField] private RectTransform _coinRectTransform;
        [SerializeField] private TMP_Text _coinsText;
        [SerializeField] private TMP_Text[] _statsText;
        [SerializeField] private TMP_Text _bloodText;

        private Tween _coinUpAnimationTween;
        
        public void UpdateCoins(int count)
        {
            _coinsText.text = count.ToString();
        }
        
        public void UpdateStat(PlayerCardType type, int count)
        {
            var number = (int)type - 1;
            _statsText[number].text = count.ToString();
        }

        public void UpdateBlood(int count, int maxCount)
        {
            _bloodText.text = $"{count}/{maxCount}";
        }

        public void FlyCoin(RectTransform rectTransform, Action endFly)
        {
            rectTransform.DOJump(new Vector3(-8.5f, 1f, 5.5f), 0.5f, 1, 0.7f).OnComplete(()=>
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