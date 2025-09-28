using UnityEngine;

namespace NothingAwaits.Core.Actor.Views
{
    public class BaseActor: MonoBehaviour
    {
        [SerializeField] protected Animator _animator;
        
        protected bool _isDeath;

        protected virtual void DeathAnimation()
        {
            _isDeath = true;
            _animator.SetTrigger(BaseActorAnimationType.Death.ToString());
        }

        protected virtual void SpeedAnimation(float speed)
        {
            _animator.SetFloat(BaseActorAnimationType.Speed.ToString(), speed);
        }
    }
}