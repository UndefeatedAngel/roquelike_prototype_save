using UnityEngine;

namespace NothingAwaits.Dialogs
{
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("Dialogue Settings")]
        public string dialogueSequenceName;
        public bool triggerOnStart = false;
        public bool triggerOnTriggerEnter = true;
        public bool triggerOnInteract = false;
        public KeyCode interactKey = KeyCode.E;
    
        [Header("Visual Feedback")]
        public GameObject interactionHint;
    
        private bool playerInRange = false;
    
        void Start()
        {
            if (triggerOnStart)
            {
                TriggerDialogue();
            }
        
            if (interactionHint != null)
                interactionHint.SetActive(false);
        }
    
        void Update()
        {
            if (triggerOnInteract && playerInRange && Input.GetKeyDown(interactKey))
            {
                TriggerDialogue();
            }
        }
    
        void OnTriggerEnter(Collider other)
        {
            if (triggerOnTriggerEnter && other.CompareTag("Player"))
            {
                if (triggerOnInteract)
                {
                    playerInRange = true;
                    if (interactionHint != null)
                        interactionHint.SetActive(true);
                }
                else
                {
                    TriggerDialogue();
                }
            }
        }
    
        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
                if (interactionHint != null)
                    interactionHint.SetActive(false);
            }
        }
    
        public void TriggerDialogue()
        {
            if (!DialogueManager.Instance.IsDialogueActive())
            {
                DialogueManager.Instance.StartDialogue(dialogueSequenceName);
            }
        }
    
        // Для вызова через события Unity
        public void TriggerDialogue(string sequenceName)
        {
            dialogueSequenceName = sequenceName;
            TriggerDialogue();
        }
    }
}