using UnityEngine;

namespace NothingAwaits.Dialogs
{
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
                _playerInRange = false;
                if (_interactionHint != null)
                    _interactionHint.SetActive(false);
            }
        }
        
        public void TriggerDialogue()
        {
            if (!DialogueController.Instance.IsDialogueActive() && !string.IsNullOrEmpty(_dialogueSequenceId))
            {
                DialogueController.Instance.StartDialogue(_dialogueSequenceId);
            }
        }
        
        public void TriggerDialogue(string sequenceId)
        {
            _dialogueSequenceId = sequenceId;
            TriggerDialogue();
        }
    }
}