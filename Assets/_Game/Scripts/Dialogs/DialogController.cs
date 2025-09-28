using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NothingAwaits.Dialogs
{
    public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI dialogueText;
    public Image characterPortraitImage;
    public AudioSource audioSource;
    
    [Header("Settings")]
    public float textSpeed = 0.05f;
    public float defaultDisplayTime = 3f;
    
    [Header("Dialogues")]
    public List<DialogueSequence> dialogueSequences;
    
    private Queue<DialogueLine> currentLines;
    private Coroutine currentDisplayCoroutine;
    private bool isDialogueActive = false;
    private DialogueSequence currentSequence;
    
    public static DialogueManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        dialoguePanel.SetActive(false);
        currentLines = new Queue<DialogueLine>();
    }
    
    void Update()
    {
        // Пропуск диалога по нажатию клавиши
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            SkipCurrentLine();
        }
    }
    
    public void StartDialogue(string sequenceName)
    {
        DialogueSequence sequence = dialogueSequences.Find(s => s.sequenceName == sequenceName);
        
        if (sequence != null)
        {
            StartDialogue(sequence);
        }
        else
        {
            Debug.LogWarning($"Dialogue sequence '{sequenceName}' not found!");
        }
    }
    
    public void StartDialogue(DialogueSequence sequence)
    {
        if (isDialogueActive && !sequence.interruptible) return;
        
        if (isDialogueActive)
        {
            StopAllCoroutines();
        }
        
        currentSequence = sequence;
        currentLines.Clear();
        
        foreach (DialogueLine line in sequence.lines)
        {
            currentLines.Enqueue(line);
        }
        
        dialoguePanel.SetActive(true);
        isDialogueActive = true;
        
        DisplayNextLine();
    }
    
    public void DisplayNextLine()
    {
        if (currentLines.Count == 0)
        {
            EndDialogue();
            return;
        }
        
        DialogueLine line = currentLines.Dequeue();
        
        if (currentDisplayCoroutine != null)
            StopCoroutine(currentDisplayCoroutine);
        
        currentDisplayCoroutine = StartCoroutine(DisplayLine(line));
    }
    
    private IEnumerator DisplayLine(DialogueLine line)
    {
        // Установка портрета и имени персонажа
        characterNameText.text = line.characterName;
        if (line.characterPortrait != null)
        {
            characterPortraitImage.sprite = line.characterPortrait;
            characterPortraitImage.gameObject.SetActive(true);
        }
        else
        {
            characterPortraitImage.gameObject.SetActive(false);
        }
        
        // Воспроизведение голоса
        if (line.voiceClip != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(line.voiceClip);
        }
        
        // Постепенное отображение текста
        dialogueText.text = "";
        foreach (char letter in line.text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        
        // Автоматическое продолжение через заданное время
        float displayTime = line.displayTime > 0 ? line.displayTime : defaultDisplayTime;
        yield return new WaitForSeconds(displayTime);
        
        DisplayNextLine();
    }
    
    private void SkipCurrentLine()
    {
        if (currentDisplayCoroutine != null)
        {
            StopCoroutine(currentDisplayCoroutine);
        }
        DisplayNextLine();
    }
    
    private void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        
        // Можно добавить события по окончанию диалога
        OnDialogueEnd?.Invoke(currentSequence.sequenceName);
    }
    
    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
    
    // Событие окончания диалога
    public static event Action<string> OnDialogueEnd;
}
}