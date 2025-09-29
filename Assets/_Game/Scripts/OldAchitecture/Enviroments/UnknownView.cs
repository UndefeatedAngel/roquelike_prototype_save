using System;
using NothingAwaits.Dialogs;
using NothingAwaits.Modules;
using NothingAwaits.Quests;
using UnityEngine;

namespace _Game.Scripts.OldAchitecture.Enviroments
{
    public enum UnknownType
    {
        Key,
        Buff,
        Debuff
    }
    public class UnknownView: MonoBehaviour
    {
        [SerializeField] private DialogueTrigger _dialogueTrigger;
        [SerializeField] private MoveObject _doorExit;
        [SerializeField] private QuestTarget _questTarget;
        [SerializeField] private Animator _animator;
        
        private Action _onBuff;
        private Action _onDebuff;

        private bool IsFindKey;
        private bool IsBuff;
        private bool IsDebuff;
        private UnknownType _type;
        public void SetView(UnknownType type, Action onBuff, Action onDebuff)
        {
            _type = type;
            _onBuff = onBuff;
            _onDebuff = onDebuff;
            DialogueController.Instance.OnChoiceConfirm += OnChoiceConfirm;
            DialogueController.Instance.OnCloseDialog += OnCloseDialog;
            switch (type)
            {
                case UnknownType.Key:
                    KeySetup();
                    break;
                case UnknownType.Buff:
                    BuffSetup();
                    break;
                case UnknownType.Debuff:
                    DebuffSetup();
                    break;
            }
        }

        private void KeySetup()
        {
            _questTarget = gameObject.AddComponent<QuestTarget>();
            _questTarget.SetTargetId("unknown_key");
            _dialogueTrigger.SetTriggerDialogue("unknown_key");
        }
        
        private void BuffSetup()
        {
            _dialogueTrigger.SetTriggerDialogue("unknown_buff");
        }
        
        private void DebuffSetup()
        {
            _dialogueTrigger.SetTriggerDialogue("unknown_debuff");
        }
        
        private void OnChoiceConfirm(string dialog)
        {
            if (_type == UnknownType.Key && dialog == "unknown_key_yes")
            {
                IsFindKey = true;
            }
            else if (_type == UnknownType.Buff && dialog == "unknown_buff_yes")
            {
                IsBuff = true;
            }
            else if (_type == UnknownType.Debuff && dialog == "unknown_debuff_yes")
            {
                IsDebuff = true;
            }
        }

        private void OnCloseDialog()
        {
            if (IsFindKey)
            {
                _dialogueTrigger.gameObject.SetActive(false);
                _questTarget.Collect();
                _doorExit.Move();
            }
            else if (IsBuff)
            {
                _dialogueTrigger.gameObject.SetActive(false);
                _onBuff?.Invoke();
            }
            else if (IsDebuff)
            {
                _dialogueTrigger.gameObject.SetActive(false);
                _onDebuff?.Invoke();
            }
        }

        private void OnDestroy()
        {
            DialogueController.Instance.OnChoiceConfirm -= OnChoiceConfirm;
            DialogueController.Instance.OnCloseDialog -= OnCloseDialog;
        }
    }
}