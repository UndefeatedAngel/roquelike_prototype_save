namespace NothingAwaits.Quests
{
    public class QuestPresenter
    {
        private QuestSystem _system;
        private QuestView _view;

        public QuestPresenter(QuestView view)
        {
            _view = view;
            _system = new QuestSystem();
        }

        public void StartQuest(Quest quest)
        {
            _system.StartQuest(quest);
            _view.UpdateView(quest);
        }

        public bool Collect(string targetId)
        {
            var result = _system.Collect(targetId);
            if (result)
            {
                _view.UpdateView(_system.CurrentQuest);
            }
            return result;
        }
    }
}