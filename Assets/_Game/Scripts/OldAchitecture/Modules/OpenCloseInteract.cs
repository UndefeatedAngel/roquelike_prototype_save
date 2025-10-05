using NothingAwaits.Enviroments;
using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.Modules
{
    public class OpenCloseInteract: BaseInteractObject
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _angleRotate;
        [SerializeField] private float _timeRotation;

        private Vector3 _startAngle;

        private Tween _animationTween;

        private void Start()
        {
            _startAngle = _target.localEulerAngles;
        }

        protected override void Interact()
        {
            _animationTween?.Kill();
            _animationTween = _target.DOLocalRotateQuaternion(Quaternion.Euler(_angleRotate), _timeRotation);
        }

        protected override void ClearInteract()
        {
            base.ClearInteract();
            _animationTween?.Kill();
            _animationTween = _target.DOLocalRotateQuaternion(Quaternion.Euler(_startAngle), _timeRotation);
        }
    }
}