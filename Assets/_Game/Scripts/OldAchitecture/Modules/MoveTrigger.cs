using BlockoutRoguelike.Enviroments;
using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.Modules
{
    public class MoveTrigger: BaseInteractObject
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _targetPosition;
        [SerializeField] private float _timeAnimation;

        private Vector3 _startPosition;

        protected override void TriggerEnter()
        {
            _startPosition = _target.transform.localPosition;
            _target.DOLocalMove(_targetPosition, _timeAnimation);
        }

        protected override void ClearInteract()
        {
            base.ClearInteract();
            _target.DOLocalMove(_startPosition, _timeAnimation);
        }
    }
}