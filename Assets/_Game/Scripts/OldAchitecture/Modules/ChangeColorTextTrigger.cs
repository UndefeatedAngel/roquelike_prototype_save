using BlockoutRoguelike.Enviroments;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BlockoutRoguelike.Modules
{
    public class ChangeColorTextTrigger: BaseInteractObject
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Color _disableColor;
        [SerializeField] private Color _highlightColor;
        
        private Tween _tween;

        private void Start()
        {
            ChangeTextHighlight(false);
        }

        protected override void TriggerEnter()
        {
            base.TriggerEnter();
            ChangeTextHighlight(true);
        }

        protected override void Interact()
        {
            
        }

        protected override void ClearInteract()
        {
            base.ClearInteract();
            ChangeTextHighlight(false);
        }

        private void ChangeTextHighlight(bool state)
        {
            _tween?.Kill();
            _tween = _text.DOColor(state ? _highlightColor : _disableColor, 0.5f);
        }
    }
}