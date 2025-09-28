using System;
using UnityEngine;
using UnityEngine.AI;

namespace _Game.Scripts.OldAchitecture.Modules
{
    public class FollowerAI: MonoBehaviour
    {
        [Header("Следование")]
        [SerializeField] private Transform _target;
        [SerializeField] private float stoppingDistance = 2.0f;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private bool _isFollowing;

        public Action OnFollow;
        private void Start()
        {
            if (agent == null)
            {
                Debug.LogError("NavMeshAgent не найден на объекте: " + gameObject.name);
            }

            agent.stoppingDistance = stoppingDistance;
        }

        void Update()
        {
            if (_isFollowing && _target != null)
            {
                agent.SetDestination(_target.position);
                OnFollow?.Invoke();
            }
        }

        public void SetFollowing(bool follow)
        {
            _isFollowing = follow;
            agent.isStopped = !follow;

            if (!follow)
            {
                agent.ResetPath();
            }
        }

        public void SetTarget(Transform newTarget)
        {
            _target = newTarget;
        }
    }
}