using UnityEngine;

namespace NothingAwaits.Quests
{
    public class QuestTarget : MonoBehaviour
    {
        [SerializeField] private string _targetId;
        
        public string TargetId => _targetId;

        public void SetTargetId(string targetId)
        {
            _targetId = targetId;
        }

        public void Collect()
        {
            QuestController.Instance.Collect(_targetId);
        }
    }
}