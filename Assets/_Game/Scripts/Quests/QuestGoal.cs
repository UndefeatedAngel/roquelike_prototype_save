using UnityEngine;

namespace BlockoutRoguelike.Quests
{
    [System.Serializable]
    public class QuestGoal
    {
        public string Description;
        public string TargetId; 
        [HideInInspector] public int Count; 
        [HideInInspector] public int Progress;
        [HideInInspector] public bool IsCompleted;
        public bool IsProgressShow;
        public bool HideMarkers;
    }
}