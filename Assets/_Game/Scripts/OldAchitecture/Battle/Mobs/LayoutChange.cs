using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace NothingAwaits.Cards.Mobs
{
    public enum TypeChange
    {
        Hp,
        Protection,
        Attack
    }
    
    public class LayoutChange: MonoBehaviour
    {
        [SerializeField] private GameObject[] _icons;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _text;
        
        private float _defaultPosY;
        public float DefaultPosY => _defaultPosY;
        private float _offsetAnimation;
        
        public void Init(float offsetAnimation)
        {
            _offsetAnimation = offsetAnimation;
            _defaultPosY = GetComponent<RectTransform>().anchoredPosition.y;
        }
        public void AnimateChange(TypeChange type, int amount, Action endAction)
        {
            _text.text = amount>0 ? $"+{amount}" : amount.ToString();
            gameObject.SetActive(true);
            for (int i = 0; i < _icons.Length; i++)
            {
                _icons[i].SetActive(i == (int)type);
            }
            _canvasGroup.DOFade(0f, 2f);
            transform.GetComponent<RectTransform>().DOAnchorPosY(_defaultPosY+_offsetAnimation, 1f).OnComplete(() =>
            {
                endAction?.Invoke();
                _canvasGroup.DOKill();
                DestroyImmediate(gameObject);
            });
        }
    }
}