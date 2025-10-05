using System;
using UnityEngine;

namespace NothingAwaits.Dialogs.Data
{
    [Serializable]
    public class DialogueLine
    {
        public string CharacterName;
        [TextArea(3, 10)]
        public string Text;
        public bool IsPlayerLine;
    }
}