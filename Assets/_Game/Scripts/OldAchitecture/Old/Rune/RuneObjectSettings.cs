using System;
using UnityEngine;

namespace NothingAwaits.Core.Rune
{
    [Serializable]
    public class RuneObjectSettings
    {
        [Header("Main Settings")]
        public string Name;
        public string Description;
        public Sprite MainSprite;
        [Header("Other Settings")]
        public RuneType RuneType;
        public MonoBehaviour[] RuneAbilitiesTBD;
    }

    [Serializable]
    public class RuneImageTypeSettings
    {
        public RuneType RuneType;
        public Sprite TypeSprite;
        public Color Color;
    }
}