using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NothingAwaits.Cards.Person
{
    public class BasePerson: MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _personTransform;
        [SerializeField] protected int _maxHp;
        [SerializeField] protected Image _hpImage;
        [SerializeField] protected TMP_Text _hpText;

        protected int _currentHp;
        public int CurrentHp => _currentHp;
        public Transform PersonTransform => _personTransform;

        protected virtual void Start()
        {
            _currentHp = _maxHp;
            UpdateHp();
        }

        protected void UpdateHp()
        {
            _hpImage.fillAmount = (float)_currentHp / _maxHp;
            _hpText.text = $"{_currentHp}/{_maxHp}";
        }

        public virtual void AddHp(int amount)
        {
            if (_currentHp < 1) return;
            
            _currentHp += amount;
            if (_currentHp < 1)
            {
                _currentHp = 0;
                _animator.SetTrigger("death");
            }
            UpdateHp();
        }

        public void WinDance()
        {
            _animator.SetBool("dance", true);
        }
    }
}