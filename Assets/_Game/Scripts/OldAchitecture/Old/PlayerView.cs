using System;
using _Game.Scripts.OldAchitecture.Modules;
using NothingAwaits.Dialogs;
using TMPro;
using UnityEngine;

namespace NothingAwaits.Core
{
    public class PlayerView: MonoBehaviour
    {
        [SerializeField] private TMP_Text _interactText;
        private Action _interact;

        private Transform _lastAttached;
        private FollowerAI _follower;

        private void Start()
        {
            _lastAttached = transform;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                _interact?.Invoke();
                _interactText.text = "";
            }
        }

        public void Interact(Action endAction)
        {
            _interactText.text = "F";
            _interact = endAction;
        }

        public void ClearInteract()
        {
            _interactText.text = "";
            _interact = null;
        }

        public void Attach(string sequenceId, DialogueTrigger dialogueTrigger)
        {
            if (sequenceId == "gameplay_sacrifice_yes")
            {
                _follower = dialogueTrigger.GetComponentInParent<FollowerAI>();
                dialogueTrigger.gameObject.SetActive(false);
            }
        }

        public void ApplyAttach()
        {
            if (_follower == null) return;
            
            _follower.SetTarget(_lastAttached);
            _follower.SetFollowing(true);
            _lastAttached = _follower.transform;
        }
    }
}