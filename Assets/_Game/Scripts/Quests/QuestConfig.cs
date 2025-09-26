using UnityEngine;

namespace BlockoutRoguelike.Quests
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Roguelike/QuestConfig")]
    public class QuestConfig : ScriptableObject
    {
        public string Id;
        public string Title;
        [HideInInspector] public string Description;
        public QuestGoal[] Goals;
    }
}