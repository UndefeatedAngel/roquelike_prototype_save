using System;
using NothingAwaits.Dialogs;
using NothingAwaits.Quests;
using UnityEngine;

namespace _Game.Scripts.OldAchitecture.Enviroments
{
    public class RestView: MonoBehaviour
    {
        [SerializeField] private DialogueTrigger _dialogueTrigger;
        [SerializeField] private QuestTarget _questTarget;

        private Action _onBuff;
        private bool _isBuff;

        public void SetView(Action onBuff)
        {
            _onBuff = onBuff;
            DialogueController.Instance.OnChoiceConfirm += OnChoiceConfirm;
            DialogueController.Instance.OnCloseDialog += OnCloseDialog;
        }
        
        private void OnChoiceConfirm(string dialog)
        {
            if (dialog == "rest_yes")
            {
                _isBuff = true;
            }
        }

        private void OnCloseDialog()
        {
            if (!_isBuff) return;
            
            _dialogueTrigger.gameObject.SetActive(false);
            _onBuff?.Invoke();
        }

        private void OnDestroy()
        {
            DialogueController.Instance.OnChoiceConfirm -= OnChoiceConfirm;
            DialogueController.Instance.OnCloseDialog -= OnCloseDialog;
        }
    }
}