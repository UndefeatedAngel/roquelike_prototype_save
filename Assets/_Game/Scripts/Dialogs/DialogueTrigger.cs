using NothingAwaits.Core;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
        [SerializeField] private float _hintOffsetY = 2f;
        [SerializeField] private float _hintDistance = 1.5f;
        
        private bool _playerInRange = false;
        private Camera _mainCamera;
        private PlayerView playerView;
        
        private void Start()
        {
            _mainCamera = Camera.main;
            
            if (_triggerOnStart)
            {
                TriggerDialogue();
            }
            
            SetInteractionHintActive(false);
            DialogueController.Instance.OnChoiceConfirm += OnChoiceConfirm;
            DialogueController.Instance.OnCloseDialog += OnCloseDialog;
        }
        
        private void Update()
        {
            if (_triggerOnInteract && _playerInRange && Input.GetKeyDown(_interactKey))
            {
                TriggerDialogue();
            }
            
            UpdateHintRotation();
        }
        
        private void LateUpdate()
        {
            UpdateHintPosition();
        }
        
        private void UpdateHintPosition()
        {
            if (_interactionHint == null || !_interactionHint.activeInHierarchy) return;
            
            Vector3 npcPosition = transform.position;
            if (_mainCamera != null)
            {
                Vector3 directionToCamera = (_mainCamera.transform.position - npcPosition).normalized;
                directionToCamera.y = 0;
                
                Vector3 hintPosition = npcPosition + (directionToCamera * _hintDistance);
                hintPosition.y += _hintOffsetY;
                
                _interactionHint.transform.position = hintPosition;
            }
            else
            {
                _interactionHint.transform.position = npcPosition + Vector3.up * _hintOffsetY;
            }
        }
        
        private void UpdateHintRotation()
        {
            if (_interactionHint == null || !_interactionHint.activeInHierarchy || _mainCamera == null) return;
            
            // Поворачиваем подсказку чтобы она всегда смотрела на камеру
            _interactionHint.transform.LookAt(_interactionHint.transform.position + _mainCamera.transform.rotation * Vector3.forward,
                                            _mainCamera.transform.rotation * Vector3.up);
        }
        
        private void SetInteractionHintActive(bool active)
        {
            if (_interactionHint != null)
            {
                _interactionHint.SetActive(active);
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (_triggerOnTriggerEnter && other.CompareTag("Player"))
            {
                if (_triggerOnInteract)
                {
                    playerView = other.GetComponent<PlayerView>();
                    _playerInRange = true;
                    SetInteractionHintActive(true);
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
                SetInteractionHintActive(false);
            }
        }
        
        public void TriggerDialogue()
        {
            if (!DialogueController.Instance.IsDialogueActive() && !string.IsNullOrEmpty(_dialogueSequenceId))
            {
                DialogueController.Instance.StartNewDialog(_dialogueSequenceId);
                if (playerView == null) return;
                SetInteractionHintActive(false);
            }
        }

        private void OnChoiceConfirm(string sequenceId)
        {
            playerView?.Attach(sequenceId, this);
        }

        private void OnCloseDialog()
        {
            playerView?.ApplyAttach();
            if (_playerInRange)
            {
                SetInteractionHintActive(true);
            }
        }
        
        public void SetTriggerDialogue(string sequenceId)
        {
            _dialogueSequenceId = sequenceId;
        }
        
        private void OnDestroy()
        {
            if (DialogueController.Instance != null)
            {
                DialogueController.Instance.OnChoiceConfirm -= OnChoiceConfirm;
                DialogueController.Instance.OnCloseDialog -= OnCloseDialog;
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            if (_interactionHint != null)
            {
                Gizmos.color = Color.yellow;
                Vector3 hintPosition = transform.position + Vector3.up * _hintOffsetY;
                Gizmos.DrawWireSphere(hintPosition, 0.3f);
                Gizmos.DrawLine(transform.position, hintPosition);
            }
        }
    }
}