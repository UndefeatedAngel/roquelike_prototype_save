using NothingAwaits.Core;
using NothingAwaits.Core.Actor.Views;
using UnityEngine;

namespace NothingAwaits.Dialogs
{
    //TODO: рефакторить, отвязать от контроллеров
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("Dialogue Settings")]
        [SerializeField] private string _dialogueSequenceId;
        [SerializeField] private bool _triggerOnStart;
        [SerializeField] private bool _triggerOnTriggerEnter;
        [SerializeField] private bool _triggerOnInteract = true;
        [SerializeField] private KeyCode _interactKey = KeyCode.F;
        
        [Header("Visual Feedback")]
        [SerializeField] private GameObject _interactionHint;
        
        private bool _playerInRange = false;
        
        private PlayerView playerView;
        
        private void Start()
        {
            if (_triggerOnStart)
            {
                TriggerDialogue();
            }
            
            if (_interactionHint != null)
                _interactionHint.SetActive(false);
        }
        
        private void Update()
        {
            if (_triggerOnInteract && _playerInRange && Input.GetKeyDown(_interactKey))
            {
                TriggerDialogue();
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (_triggerOnTriggerEnter && other.CompareTag("Player"))
            {
                playerView = other.GetComponent<PlayerView>();
                if (_triggerOnInteract)
                {
                    _playerInRange = true;
                    if (_interactionHint != null)
                        _interactionHint.SetActive(true);
                }
                else
                {
                    TriggerDialogue();
                }
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerView = null;
                _playerInRange = false;
                if (_interactionHint != null)
                    _interactionHint.SetActive(false);
            }
        }
        
        public void TriggerDialogue()
        {
            if (!DialogueController.Instance.IsDialogueActive() && !string.IsNullOrEmpty(_dialogueSequenceId))
            {
                DialogueController.Instance.StartNewDialog(_dialogueSequenceId);
                if (playerView == null) return;
                DialogueController.Instance.OnChoiceConfirm = (sequenceId) =>
                {
                    playerView.Attach(sequenceId, this);
                };
                DialogueController.Instance.OnCloseDialog = () =>
                {
                    playerView.ApplyAttach();
                };
            }
        }
        
        public void TriggerDialogue(string sequenceId)
        {
            _dialogueSequenceId = sequenceId;
            TriggerDialogue();
        }
    }
}