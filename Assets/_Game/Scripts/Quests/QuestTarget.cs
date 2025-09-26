using UnityEngine;

namespace BlockoutRoguelike.Quests
{
    public class QuestTarget : MonoBehaviour
    {
        [SerializeField] private string _targetId;
        
        public string TargetId => _targetId;

        public void Collect()
        {
            QuestController.Instance.Collect(_targetId);
        }
    }
}