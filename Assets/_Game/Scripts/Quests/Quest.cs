using System.Collections.Generic;
using System.Linq;

namespace NothingAwaits.Quests
{
    [System.Serializable]
    public class Quest
    {
        public string Id;
        public string Title;
        public string Description;
        public QuestState State = QuestState.NotStarted;
        public List<QuestGoal> Goals;

        public bool AreAllGoalsCompleted
        {
            get
            {
                return Goals.All(goal => goal.IsCompleted);
            }
        }
    }
}