using UnityEngine;
using TMPro;

namespace NothingAwaits.Dialogs.Data
{
    public class DialogueAnswer : MonoBehaviour
    {
        [SerializeField] public TMP_Text AnswerText;
        [SerializeField] public GameObject SelectedIndicator;
        
        public void SetSelected(bool selected)
        {
            if (SelectedIndicator != null)
                SelectedIndicator.SetActive(selected);
        }
        
        public void SetText(string text)
        {
            if (AnswerText != null)
                AnswerText.text = text;
        }
    }
}