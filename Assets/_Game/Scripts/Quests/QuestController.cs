using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

namespace BlockoutRoguelike.Quests
{
    public class QuestController : MonoBehaviour
    {
        private static QuestController _instance;
        public static QuestController Instance => _instance;
        
        [SerializeField] private QuestView _questView;
        [SerializeField] private QuestConfig _questConfig;
        [SerializeField] private bool _isPlayOnStart;
        
        private QuestPresenter _presenter;
        private Quest _currentQuest;

        public event Action<QuestGoal, Transform[]> OnCurrentQuestTargets;

        private void Start()
        {
            if (!_isPlayOnStart) return;
            
            Setup();
            StartQuest();
        }

        public void Setup()
        {
            _instance = this;
            
            _presenter = new QuestPresenter(_questView);
            
            if (_questConfig != null)
            {
                CreateQuestFromConfig();
            }
        }

        private void CreateQuestFromConfig()
        {
            var allTargets = FindObjectsOfType<QuestTarget>();
            var targetGroups = allTargets.GroupBy(t => t.TargetId)
                                        .ToDictionary(g => g.Key, g => g.ToList());

            _currentQuest = new Quest
            {
                Id = _questConfig.Id,
                Title = _questConfig.Title,
                Description = _questConfig.Description,
                Goals = new List<QuestGoal>(),
                State = QuestState.NotStarted
            };

            foreach (var goalConfig in _questConfig.Goals)
            {
                if (targetGroups.TryGetValue(goalConfig.TargetId, out var targets))
                {
                    var goal = new QuestGoal
                    {
                        Description = goalConfig.Description,
                        TargetId = goalConfig.TargetId,
                        Count = targets.Count,
                        Progress = 0,
                        IsCompleted = false,
                        IsProgressShow = goalConfig.IsProgressShow,
                        HideMarkers = goalConfig.HideMarkers
                    };
                    
                    _currentQuest.Goals.Add(goal);
                }
                else
                {
                    Debug.LogWarning($"Объекты с TargetId '{goalConfig.TargetId}' не найдены на сцене");
                }
            }

            UpdateQuestTargets();
        }

        public void StartQuest()
        {
            if (_currentQuest != null)
            {
                _presenter.StartQuest(_currentQuest);
                UpdateQuestTargets();
            }
        }

        public void Collect(string targetId)
        {
            if (_presenter.Collect(targetId))
            {
                UpdateQuestTargets();
            }
        }

        private void UpdateQuestTargets()
        {
            if (_currentQuest == null || _currentQuest.State != QuestState.InProgress)
            {
                OnCurrentQuestTargets?.Invoke(null, Array.Empty<Transform>());
                return;
            }

            var activeTargets = new List<Transform>();
            var allTargets = FindObjectsOfType<QuestTarget>();

            QuestGoal activeGoal = null;
            foreach (var goal in _currentQuest.Goals)
            {
                activeGoal = goal;
                if (goal.IsCompleted) continue;

                var targetsForGoal = allTargets
                    .Where(t => t.TargetId == goal.TargetId)
                    .Select(t => t.transform)
                    .ToArray();

                activeTargets.AddRange(targetsForGoal);
                break;
            }

            OnCurrentQuestTargets?.Invoke(activeGoal, activeTargets.ToArray());
        }
    }
}