using UnityEngine;
using System.Collections.Generic;

namespace NothingAwaits.Dialogs.Data
{
    [CreateAssetMenu(fileName = "New DialogueSequence", menuName = "NothingAwaits/DialogueSequence")]
    public class DialogueSequenceConfig : ScriptableObject
    {
        public string SequenceID;
        public List<DialogueLine> Lines;
        public List<DialogueChoice> Choices;
        public string NextSequenceId;
        
        public bool HasChoices => Choices != null && Choices.Count > 0;
        public bool HasNextSequence => !string.IsNullOrEmpty(NextSequenceId);
    }
}