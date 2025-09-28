

using System.Collections.Generic;
using UnityEngine;

namespace NothingAwaits.Dialogs
{
    [System.Serializable]
    public class DialogueLine
    {
        public string characterName;
        [TextArea(3, 10)]
        public string text;
        public AudioClip voiceClip;
        public float displayTime = 3f;
        public Sprite characterPortrait;
    }

    [System.Serializable]
    public class DialogueSequence
    {
        public string sequenceName;
        public List<DialogueLine> lines;
        public bool interruptible = true;
    }
}