using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using NothingAwaits.Dialogs.Data;
using TMPro;
using StarterAssets;

namespace NothingAwaits.Dialogs
{
    public class DialogueController : MonoBehaviour
    {
        public static DialogueController Instance { get; private set; }
        [Header("Settings")]
        [SerializeField] private DialogueSequenceConfig[] _dialogSequences;
        [SerializeField] private float _textSpeed = 0.05f;
        [Space]
        [Header("UI References")]
        [SerializeField] private GameObject _dialoguePanel;
        [SerializeField] private GameObject _npcSection;
        [SerializeField] private GameObject _playerSection;
        [SerializeField] private TMP_Text _npcTitleText;
        [SerializeField] private TMP_Text _npcDescriptionText;
        [SerializeField] private TMP_Text _playerTitleText;
        [SerializeField] private TMP_Text _playerDescriptionText;
        [SerializeField] private GameObject _dialogVertical;
        [SerializeField] private GameObject _choiseVertical;
        [SerializeField] private Transform _choicesContainer;
        [SerializeField] private DialogueAnswer _answerPrefab;
        [Space]
        [Header("Player Controller References")]
        [SerializeField] private ThirdPersonController _playerController;
        [SerializeField] private StarterAssetsInputs _playerInputs;
        [SerializeField] private Animator _playerAnimator;
        [Space]
        [Header("Additional Objects")]
        [SerializeField] private GameObject[] _objectsToEnableOnDialogueEnd;
        
        
        private Queue<DialogueLine> _currentLines;
        private DialogueSequenceConfig _currentSequence;
        private bool _isDialogueActive = false;
        private bool _isShowingChoices = false;
        private CancellationTokenSource _dialogueCancellationTokenSource;
        private List<DialogueAnswer> _currentChoiceAnswers = new();
        private int _selectedChoiceIndex = 0;
        private string _targetAnimateText;
        private TMP_Text _targetAnimateTmpText;
        
        private bool _wasCursorLocked = true;
        private CursorLockMode _previousCursorLockMode;
        private bool _playerControllerWasEnabled = true;
        private bool _playerAnimatorWasEnabled = true;
        
        public Action<string> OnChoiceConfirm;
        public Action OnCloseDialog;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            _dialoguePanel.SetActive(false);
            _npcSection.SetActive(false);
            _playerSection.SetActive(false);
            _dialogVertical.SetActive(false);
            _choiseVertical.SetActive(false);
            _currentLines = new Queue<DialogueLine>();
        }
        
        private void Update()
        {
            if (!_isDialogueActive) return;
            
            if (_isShowingChoices)
            {
                HandleChoiceInput();
            }
            else
            {
                HandleDialogueInput();
            }
        }
        
        private void HandleDialogueInput()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                SkipCurrentLine();
            }
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CloseDialogue();
            }
        }
        
        private void HandleChoiceInput()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ChangeSelectedChoice(1);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                ChangeSelectedChoice(-1);
            }
            
            if (Input.GetKeyDown(KeyCode.F))
            {
                ConfirmChoice();
            }
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CloseDialogue();
            }
        }
        
        private void ChangeSelectedChoice(int direction)
        {
            if (_currentChoiceAnswers.Count == 0) return;
            if (_selectedChoiceIndex >= 0 && _selectedChoiceIndex < _currentChoiceAnswers.Count)
            {
                _currentChoiceAnswers[_selectedChoiceIndex].SetSelected(false);
            }
            _selectedChoiceIndex = (_selectedChoiceIndex + direction + _currentChoiceAnswers.Count) % _currentChoiceAnswers.Count;
            if (_selectedChoiceIndex >= 0 && _selectedChoiceIndex < _currentChoiceAnswers.Count)
            {
                _currentChoiceAnswers[_selectedChoiceIndex].SetSelected(true);
            }
        }
        
        private void OnAnswerMouseEnter(DialogueAnswer answer)
        {
            if (!_isShowingChoices) return;
            if (_selectedChoiceIndex >= 0 && _selectedChoiceIndex < _currentChoiceAnswers.Count)
            {
                _currentChoiceAnswers[_selectedChoiceIndex].SetSelected(false);
            }
            _selectedChoiceIndex = answer.ChoiceIndex;
            if (_selectedChoiceIndex >= 0 && _selectedChoiceIndex < _currentChoiceAnswers.Count)
            {
                _currentChoiceAnswers[_selectedChoiceIndex].SetSelected(true);
            }
        }
        
        private void OnAnswerMouseClick(DialogueAnswer answer)
        {
            if (!_isShowingChoices) return;
            ConfirmChoice();
        }
        
        private void ConfirmChoice()
        {
            if (_selectedChoiceIndex < 0 || _selectedChoiceIndex >= _currentSequence.Choices.Count) return;
            
            var selectedChoice = _currentSequence.Choices[_selectedChoiceIndex];
            if (!string.IsNullOrEmpty(selectedChoice.NextSequence))
            {
                OnChoiceConfirm?.Invoke(selectedChoice.NextSequence);
                StartDialogue(selectedChoice.NextSequence);
                return;
            }
            
            CloseDialogue();
        }

        public void StartNewDialog(string sequenceId)
        {
            DisablePlayerControl();
            StartDialogue(sequenceId);
        }

        private void StartDialogue(string sequenceId)
        {
            if (string.IsNullOrEmpty(sequenceId))
            {
                Debug.LogError("DialogueSequenceConfig is null!");
                return;
            }
            
            var sequence = _dialogSequences.FirstOrDefault(x=>x.SequenceID == sequenceId);
            if (sequence == null) return;
            
            _currentSequence = sequence;
            _currentLines.Clear();
            
            foreach (var line in _currentSequence.Lines)
            {
                _currentLines.Enqueue(line);
            }
            
            ClearAllTextFields();
            _dialoguePanel.SetActive(true);
            _dialogVertical.SetActive(true);
            _choiseVertical.SetActive(false);
            _npcSection.SetActive(false);
            _playerSection.SetActive(false);
            _isDialogueActive = true;
            _isShowingChoices = false;
            
            SetAdditionalObjectsActive(false);
            
            _dialogueCancellationTokenSource = new CancellationTokenSource();
            ProcessDialogueAsync(_dialogueCancellationTokenSource.Token).Forget();
        }
        
        private void SetAdditionalObjectsActive(bool active)
        {
            foreach (var obj in _objectsToEnableOnDialogueEnd)
            {
                if (obj != null)
                    obj.SetActive(active);
            }
        }
        
        private void ClearAllTextFields()
        {
            _npcTitleText.text = "";
            _npcDescriptionText.text = "";
            _playerTitleText.text = "";
            _playerDescriptionText.text = "";
        }
        
        private void DisablePlayerControl()
        {
            _wasCursorLocked = Cursor.lockState == CursorLockMode.Locked;
            _previousCursorLockMode = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (_playerController != null)
            {
                _playerControllerWasEnabled = _playerController.enabled;
                _playerController.enabled = false;
            }
            if (_playerAnimator != null)
            {
                _playerAnimatorWasEnabled = _playerAnimator.enabled;
                _playerAnimator.enabled = false;
            }
            
            if (_playerInputs != null)
            {
                _playerInputs.cursorInputForLook = false;
                _playerInputs.cursorLocked = false;
                _playerInputs.move = Vector2.zero;
                _playerInputs.jump = false;
                _playerInputs.sprint = false;
            }
        }
        
        private void EnablePlayerControl()
        {
            if (_wasCursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = _previousCursorLockMode;
                Cursor.visible = true;
            }
            
            if (_playerController != null)
            {
                _playerController.enabled = _playerControllerWasEnabled;
            }
            
            if (_playerAnimator != null)
            {
                _playerAnimator.enabled = _playerAnimatorWasEnabled;
            }
            
            if (_playerInputs != null)
            {
                _playerInputs.cursorInputForLook = true;
                _playerInputs.cursorLocked = _wasCursorLocked;
                
                _playerInputs.move = Vector2.zero;
                _playerInputs.jump = false;
                _playerInputs.sprint = false;
            }
        }
        
        async Task WaitForSpaceKey()
        {
            while (!Input.GetKeyDown(KeyCode.Space) || !Input.GetMouseButtonDown(0))
            {
                await Task.Yield();
            }
        }
        
        private async UniTaskVoid ProcessDialogueAsync(CancellationToken token)
        {
            while (_currentLines.Count > 0 && !token.IsCancellationRequested)
            {
                var line = _currentLines.Dequeue();
                await DisplayLineAsync(line, token);
                await WaitForSpaceKey();
            }
            
            if (!token.IsCancellationRequested)
            {
                await EndSequenceAsync(token);
            }
        }
        
        private async UniTask DisplayLineAsync(DialogueLine line, CancellationToken token)
        {
            if (line.IsPlayerLine)
            {
                _playerSection.SetActive(true);
                _playerTitleText.text = string.IsNullOrEmpty(line.CharacterName) ? "[Player]" : line.CharacterName;
                _playerDescriptionText.text = "";
                await AnimateTextAsync(_playerDescriptionText, line.Text, line.DisplayTime, token);
            }
            else
            {
                _npcSection.SetActive(true);
                _npcTitleText.text = string.IsNullOrEmpty(line.CharacterName) ? "[NPC]" : line.CharacterName;
                _npcDescriptionText.text = "";
                await AnimateTextAsync(_npcDescriptionText, line.Text, line.DisplayTime, token);
            }
        }
        
        private async UniTask AnimateTextAsync(TMP_Text textComponent, string text, float displayTime, CancellationToken token)
        {
            _targetAnimateText = text;
            _targetAnimateTmpText = textComponent;
            textComponent.text = "";
            
            float calculatedSpeed = CalculateTextSpeed(text, displayTime);
            
            foreach (var letter in text)
            {
                if (token.IsCancellationRequested) return;
                
                textComponent.text += letter;
                await UniTask.WaitForSeconds(calculatedSpeed, cancellationToken: token);
            }
        }
        
        private float CalculateTextSpeed(string text, float displayTime)
        {
            if (string.IsNullOrEmpty(text) || displayTime <= 0)
                return _textSpeed;
            
            var minTimePerChar = 0.02f;
            var maxTimePerChar = 0.2f;
            
            var timePerChar = displayTime / text.Length;
            return Mathf.Clamp(timePerChar, minTimePerChar, maxTimePerChar);
        }
        
        private void SkipCurrentLine()
        {
            if (_targetAnimateTmpText != null)
            {
                _targetAnimateTmpText.text = _targetAnimateText;
            }
            
            _dialogueCancellationTokenSource?.Cancel();
            _dialogueCancellationTokenSource = new CancellationTokenSource();
            
            ProcessDialogueAsync(_dialogueCancellationTokenSource.Token).Forget();
        }
        
        private async UniTask EndSequenceAsync(CancellationToken token)
        {
            if (token.IsCancellationRequested) return;
            
            if (_currentSequence.HasChoices)
            {
                await ShowChoicesAsync(token);
            }
            else if (_currentSequence.HasNextSequence)
            {
                StartDialogue(_currentSequence.NextSequenceId);
            }
            else
            {
                CloseDialogue();
            }
        }
        
        private async UniTask ShowChoicesAsync(CancellationToken token)
        {
            _isShowingChoices = true;
            _dialogVertical.SetActive(false);
            _choiseVertical.SetActive(true);
            ClearChoices();
            
            for (int i = 0; i < _currentSequence.Choices.Count; i++)
            {
                var choice = _currentSequence.Choices[i];
                if (!choice.IsEnabled) continue;
                
                var answer = Instantiate(_answerPrefab, _choicesContainer);
                if (answer != null)
                {
                    answer.Initialize(i);
                    answer.SetText(choice.ChoiceText);
                    answer.SetSelected(false);
                    
                    answer.OnMouseEnter += OnAnswerMouseEnter;
                    answer.OnMouseClick += OnAnswerMouseClick;
                    
                    _currentChoiceAnswers.Add(answer);
                }
                else
                {
                    Debug.LogError("DialogueAnswer component not found on answer prefab!");
                }
            }
            
            if (_currentChoiceAnswers.Count > 0)
            {
                _selectedChoiceIndex = 0;
                _currentChoiceAnswers[0].SetSelected(true);
            }
            
            await UniTask.CompletedTask;
        }
        
        private void ClearChoices()
        {
            foreach (var answer in _currentChoiceAnswers)
            {
                if (answer != null)
                {
                    answer.OnMouseEnter -= OnAnswerMouseEnter;
                    answer.OnMouseClick -= OnAnswerMouseClick;
                    
                    if (answer.gameObject != null)
                        Destroy(answer.gameObject);
                }
            }
            _currentChoiceAnswers.Clear();
            _selectedChoiceIndex = 0;
        }
        
        private void CloseDialogue()
        {
            _isDialogueActive = false;
            _isShowingChoices = false;
            _dialoguePanel.SetActive(false);
            _npcSection.SetActive(false);
            _playerSection.SetActive(false);
            _dialogVertical.SetActive(false);
            _choiseVertical.SetActive(false);
            
            ClearChoices();
            EnablePlayerControl();
            
            SetAdditionalObjectsActive(true);
            
            _dialogueCancellationTokenSource?.Cancel();
            _dialogueCancellationTokenSource?.Dispose();
            _dialogueCancellationTokenSource = null;
            OnCloseDialog?.Invoke();
        }
        
        public bool IsDialogueActive()
        {
            return _isDialogueActive;
        }
        
        private void OnDestroy()
        {
            _dialogueCancellationTokenSource?.Cancel();
            _dialogueCancellationTokenSource?.Dispose();
        }
    }
}