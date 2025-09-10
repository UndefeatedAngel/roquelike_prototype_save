using TMPro;
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
        [SerializeField] protected TMP_Text _hpText;

        protected int _currentHp;
        public int CurrentHp => _currentHp;
        public Transform PersonTransform => _personTransform;

        protected virtual void Start()
        {
            _currentHp = _maxHp;
            UpdateHp(_currentHp);
        }

        private void UpdateHp(int count)
        {
            _hpImage.fillAmount = (float)_currentHp / _maxHp;
            _hpText.text = $"{_currentHp}/{_maxHp}";
        }

        public void AddHp(int amount)
        {
            if (_currentHp < 1) return;
            
            _currentHp += amount;
            if (_currentHp < 1)
            {
                _currentHp = 0;
                _animator.SetTrigger("death");
            }
            UpdateHp(_currentHp);
        }
    }
}