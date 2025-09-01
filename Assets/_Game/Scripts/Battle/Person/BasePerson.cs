using UnityEngine;
using UnityEngine.UI;

namespace BlockoutRoguelike.Cards.Person
{
    public class BasePerson: MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _personTransform;
        [SerializeField] protected int _maxHp;
        [SerializeField] protected Image _hpImage;

        private int currentHp;
        public int CurrentHp => currentHp;
        public Transform PersonTransform => _personTransform;

        protected void Start()
        {
            currentHp = _maxHp;
            UpdateHp(currentHp);
        }

        private void UpdateHp(int count)
        {
            _hpImage.fillAmount = (float)currentHp / _maxHp;
        }

        public void AddHp(int amount)
        {
            if (currentHp < 1) return;
            
            currentHp += amount;
            if (currentHp < 1)
            {
                currentHp = 0;
                _animator.SetTrigger("death");
            }
            UpdateHp(currentHp);
        }
    }
}