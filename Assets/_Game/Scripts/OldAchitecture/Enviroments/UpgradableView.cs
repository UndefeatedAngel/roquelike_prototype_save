using NothingAwaits.Dialogs;
using NothingAwaits.Modules;
using NothingAwaits.Quests;
using UnityEngine;

namespace _Game.Scripts.OldAchitecture.Enviroments
{
    public class UpgradableView: MonoBehaviour
    {
        [SerializeField] private MoveObject _doorExit;
        [SerializeField] private QuestTarget _questTarget;
        [SerializeField] private Animator _animator;
        private void Start()
        {
            DialogueController.Instance.OnCloseDialog += OnCloseDialog;
        }

        private void OnCloseDialog()
        {
            _doorExit.Move();
            _questTarget.Collect();
        }

        private void OnDestroy()
        {
            DialogueController.Instance.OnCloseDialog -= OnCloseDialog;
        }
    }
}