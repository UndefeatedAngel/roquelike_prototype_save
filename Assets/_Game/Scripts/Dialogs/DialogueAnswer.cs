using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;

namespace NothingAwaits.Dialogs.Data
{
    public class DialogueAnswer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField] public TMP_Text AnswerText;
        [SerializeField] public CanvasGroup SelectedIndicator;
        
        public event Action<DialogueAnswer> OnMouseEnter;
        public event Action<DialogueAnswer> OnMouseClick;
        
        public int ChoiceIndex { get; private set; }
        
        public void Initialize(int choiceIndex)
        {
            ChoiceIndex = choiceIndex;
        }
        
        public void SetSelected(bool selected)
        {
            if (SelectedIndicator != null)
            {
                SelectedIndicator.DOFade(selected ? 1f : 0f, 0.2f);
            }
        }
        
        public void SetText(string text)
        {
            if (AnswerText != null)
                AnswerText.text = text;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnMouseEnter?.Invoke(this);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            OnMouseClick?.Invoke(this);
        }
        
        private void OnDestroy()
        {
            OnMouseEnter = null;
            OnMouseClick = null;
        }
    }
}