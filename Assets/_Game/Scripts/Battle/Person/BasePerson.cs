using UnityEngine;
using UnityEngine.UI;

namespace BlockoutRoguelike.Cards.Person
{
    public class BasePerson: MonoBehaviour
    {
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
            currentHp += amount;
            if (currentHp < 0)
            {
                return;
            }
            UpdateHp(currentHp);
        }
    }
}