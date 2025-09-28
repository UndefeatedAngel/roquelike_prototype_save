using System;
using System.Collections.Generic;

namespace NothingAwaits.Dialogs.Data
{
    [Serializable]
    public class DialogueSequence
    {
        public string SequenceID;
        public List<DialogueLine> Lines;
        public List<DialogueChoice> Choices;
        public string NextSequence;
    }
}