using System;

namespace NothingAwaits.Dialogs.Data
{
    [Serializable]
    public class DialogueChoice
    {
        public string ChoiceText;
        public string NextSequence;
        public bool IsEnabled = true;
    }
}