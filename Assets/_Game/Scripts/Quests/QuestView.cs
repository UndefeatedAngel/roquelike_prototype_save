using UnityEngine;
using TMPro;

namespace BlockoutRoguelike.Quests
{
    public class QuestView : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _goalsText;

        public void UpdateView(Quest quest)
        {
            if (quest == null)
            {
                _titleText.text = "Квестов нет";
                _descriptionText.text = "";
                _goalsText.text = "";
                return;
            }

            _titleText.text = quest.Title;
            _descriptionText.text = quest.Description;

            var goalsInfo = "";
            foreach (var goal in quest.Goals)
            {
                var status = goal.IsCompleted ? "[X]" : "[ ]";
                goalsInfo += $"{status} {goal.Description} ";
                if (goal.IsProgressShow)
                {
                    goalsInfo += $"({goal.Progress}/{goal.Count})";
                }
                goalsInfo +="\n";
                 
            }
            _goalsText.text = goalsInfo;
        }
    }
}