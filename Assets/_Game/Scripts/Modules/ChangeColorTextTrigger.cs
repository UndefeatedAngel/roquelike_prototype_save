using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BlockoutRoguelike.Modules
{
    public class ChangeColorTextTrigger: MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Color _disableColor;
        [SerializeField] private Color _highlightColor;
        
        private Tween _tween;

        private void Start()
        {
            ChangeTextHighlight(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ChangeTextHighlight(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ChangeTextHighlight(false);
            }
        }

        private void ChangeTextHighlight(bool state)
        {
            _tween?.Kill();
            _tween = _text.DOColor(state ? _highlightColor : _disableColor, 0.5f);
        }
    }
}