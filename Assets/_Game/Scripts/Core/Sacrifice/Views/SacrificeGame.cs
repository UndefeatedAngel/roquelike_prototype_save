using _Game.Scripts.OldAchitecture.Modules;
using UnityEngine;
using UnityEngine.AI;

namespace NothingAwaits.Core.Actor.Views
{
    public class SacrificeGame: BaseSacrifice
    {
        [SerializeField] private FollowerAI _followerAI;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private float _rotationSpeed = 5f;

        private void Start()
        {
            _followerAI.OnFollow += OnFollow;
        }

        private void OnFollow()
        {
            UpdateAnimation();
            SmoothRotation();
        }

        void UpdateAnimation()
        {
            if (_animator != null && _navMeshAgent!=null)
            {
                var speed = _navMeshAgent.velocity.magnitude;
                SpeedAnimation(speed);
            }
        }
        
        void SmoothRotation()
        {
            if (_navMeshAgent.velocity.magnitude > 0.1f)
            {
                var targetRotation = Quaternion.LookRotation(_navMeshAgent.velocity.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
                    _rotationSpeed * Time.deltaTime);
            }
        }
    }
}