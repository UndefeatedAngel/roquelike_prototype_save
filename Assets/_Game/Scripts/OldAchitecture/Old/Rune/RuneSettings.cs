using System.Linq;
using UnityEngine;

namespace NothingAwaits.Core.Rune
{
    [CreateAssetMenu(fileName = "Rune Settings", menuName = "BlockoutRoguelike/Rune Settings")]
    public class RuneSettings: ScriptableObject
    {
        [SerializeField] private RuneObjectSettings[] _runeObjects;
        [Space]
        [SerializeField] private RuneImageTypeSettings[] _runeImageType;
        
        public RuneObjectSettings[] RuneObjects => _runeObjects;

        public RuneImageTypeSettings GetRuneImageType(RuneType runeType)
        {
            return _runeImageType.FirstOrDefault(x=>x.RuneType == runeType);
        }
    }
}