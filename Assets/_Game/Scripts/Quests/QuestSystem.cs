using System.Linq;

namespace NothingAwaits.Quests
{
    public class QuestSystem
    {
        private Quest _currentQuest;
        
        public Quest CurrentQuest => _currentQuest;

        public void StartQuest(Quest quest)
        {
            _currentQuest = quest;
            _currentQuest.State = QuestState.InProgress;
        }

        public bool Collect(string targetId)
        {
            if (_currentQuest == null || _currentQuest.State != QuestState.InProgress)
                return false;

            var goal = _currentQuest.Goals
                .FirstOrDefault(g => g.TargetId == targetId && !g.IsCompleted);

            if (goal != null)
            {
                goal.Progress++;
                
                if (goal.Progress >= goal.Count)
                {
                    goal.IsCompleted = true;
                }

                if (_currentQuest.AreAllGoalsCompleted)
                {
                    _currentQuest.State = QuestState.Completed;
                }

                return true;
            }

            return false;
        }
    }
}