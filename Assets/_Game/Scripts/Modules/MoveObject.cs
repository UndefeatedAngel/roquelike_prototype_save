using DG.Tweening;
using UnityEngine;

namespace BlockoutRoguelike.Modules
{
    public class MoveObject: MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _targetPosition;
        [SerializeField] private float _timeAnimation;


        public void Move()
        {
            _target.DOLocalMove(_targetPosition, _timeAnimation);
        }
    }
}