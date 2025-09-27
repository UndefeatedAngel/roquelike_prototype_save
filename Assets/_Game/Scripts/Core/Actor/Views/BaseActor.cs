using UnityEngine;

namespace NothingAwaits.Core.Actor.Views
{
    public class BaseActor: MonoBehaviour
    {
        [SerializeField] protected Animator _animator;
        
        protected bool _isDeath;

        protected virtual void Death()
        {
            _isDeath = true;
            _animator.SetTrigger("death");
        }
    }
}