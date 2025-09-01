using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BlockoutRoguelike.Cards.Mobs
{
    public class LayoutDamage: MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _text;
        
        public void AnimateDamage(int amount)
        {
            _text.text = amount.ToString();
            gameObject.SetActive(true);
            _canvasGroup.DOFade(0f, 2f);
            transform.GetComponent<RectTransform>().DOAnchorPosY(1f, 1f).OnComplete(() =>
            {
                _canvasGroup.DOKill();
                DestroyImmediate(gameObject);
            });
        }
    }
}