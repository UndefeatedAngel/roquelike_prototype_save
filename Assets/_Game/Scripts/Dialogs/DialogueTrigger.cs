using System;
using NothingAwaits.Core;
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
        [SerializeField] private Transform _hintParent; // Родитель для позиционирования
        [SerializeField] private Vector3 _hintLocalOffset = new Vector3(0f, 2f, 0f); // Локальный offset относительно родителя
        [SerializeField] private float _hintDistance = 1.5f;
        
        private bool _playerInRange = false;
        private Camera _mainCamera;
        private PlayerView playerView;
        
        // Свойство для автоматического выбора родителя
        private Transform HintParent => _hintParent != null ? _hintParent : transform;
        
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
            
            // Используем родителя для позиционирования
            Vector3 parentPosition = HintParent.position;
            Vector3 parentForward = HintParent.forward;
            
            if (_mainCamera != null)
            {
                Vector3 directionToCamera = (_mainCamera.transform.position - parentPosition).normalized;
                directionToCamera.y = 0;
                
                // Применяем локальный offset и мировое смещение
                Vector3 localOffset = HintParent.TransformDirection(_hintLocalOffset);
                Vector3 worldOffset = directionToCamera * _hintDistance;
                
                Vector3 hintPosition = parentPosition + localOffset + worldOffset;
                
                _interactionHint.transform.position = hintPosition;
            }
            else
            {
                // Без камеры используем только локальный offset
                Vector3 localOffset = HintParent.TransformDirection(_hintLocalOffset);
                _interactionHint.transform.position = parentPosition + localOffset;
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

        private void OnDisable()
        {
            if (DialogueController.Instance != null)
            {
                DialogueController.Instance.OnChoiceConfirm -= OnChoiceConfirm;
            }
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
                Transform parent = HintParent;
                Vector3 localOffset = parent.TransformDirection(_hintLocalOffset);
                Vector3 hintPosition = parent.position + localOffset;
                
                Gizmos.DrawWireSphere(hintPosition, 0.3f);
                Gizmos.DrawLine(parent.position, hintPosition);
                
                // Дополнительно показываем направление вперед от родителя
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(parent.position, parent.forward * 1f);
            }
        }
        
        // Методы для настройки в runtime (опционально)
        public void SetHintParent(Transform newParent)
        {
            _hintParent = newParent;
        }
        
        public void SetHintLocalOffset(Vector3 newOffset)
        {
            _hintLocalOffset = newOffset;
        }
    }
}