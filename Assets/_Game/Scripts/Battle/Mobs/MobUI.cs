using System.Collections;
using BlockoutRoguelike.Battle.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlockoutRoguelike.Cards.Mobs
{
    public class MobUI: MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _hpText;
        [SerializeField] private TMP_Text _attackText;
        [SerializeField] private TMP_Text _protectionText;
        [SerializeField] private Animator _animator;
        
        [Header("Damage")]
        [SerializeField] private LayoutDamage _layoutDamagePrefab;
        
        public int Attack { get; private set; }
        public int Hp { get; private set; }
        public int Protection { get; private set; }

        private CardData _cardData;
        public void SetMob(string nameText, CardData cardData)
        {
            _cardData = _cardData;

            Attack = cardData.Attack;
            Hp = cardData.Health;
            Protection = cardData.Protection;
            
            _nameText.text = nameText;
            _hpText.text = cardData.Health.ToString();
            _attackText.text = cardData.Attack.ToString();
            _protectionText.text = cardData.Protection.ToString();
        }

        public void AttackAnimation()
        {
            _animator.SetTrigger("attack");
        }

        public void AddHp(int amount)
        {
            Hp += amount;
            if (Hp < 1)
            {
                Hp = 0;
            }

            AnimateHp(amount);
            _hpText.text = Hp.ToString();
        }

        public void RemoveMob()
        {
            StartCoroutine(WaitRemoveMob());
        }

        public IEnumerator WaitRemoveMob()
        {
            _animator.SetTrigger("death");
            yield return new WaitForSeconds(1f);
            DestroyImmediate(gameObject);

        }

        private void AnimateHp(int amount)
        {
            var damage = Instantiate(_layoutDamagePrefab, _layoutDamagePrefab.transform.parent);
            damage.AnimateDamage(amount);
        }
    }
}