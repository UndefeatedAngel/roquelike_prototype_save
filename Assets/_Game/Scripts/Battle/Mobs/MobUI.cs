using System;
using System.Collections;
using BlockoutRoguelike.Battle.Controllers;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace BlockoutRoguelike.Cards.Mobs
{
    public class MobUI: MonoBehaviour
    {
        [SerializeField] private CanvasGroup _protectionGroup;
        
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _hpText;
        [SerializeField] private TMP_Text _attackText;
        [SerializeField] private TMP_Text _protectionText;
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _highlight;
        
        [Header("Damage")]
        [SerializeField] private LayoutDamage _layoutDamagePrefab;
        
        [Header("Effects")]
        [SerializeField] private ParticleSystem[] _hpEffects;
        [SerializeField] private ParticleSystem[] _shieldEffects;
        
        public PlayerCardType MobType { get; private set; }
        public int Attack { get; private set; }
        public int Hp { get; private set; }
        public int MaxHp { get; private set; }
        public int Protection { get; private set; }

        private CardData _cardData;

        public Action<PlayerCardType> OnDeath;
        public Action<int> OnGetBlood;
        
        public void SetMob(string nameText, CardData cardData)
        {
            _cardData = cardData;

            MobType = cardData.CardType;
            Attack = cardData.Attack;
            Hp = cardData.Health;
            MaxHp = cardData.Health;
            Protection = cardData.Protection;
            
            _nameText.text = nameText;
            _hpText.text = $"{Hp}/{MaxHp}";
            _attackText.text = cardData.Attack.ToString();
            _protectionText.text = cardData.Protection.ToString();
            if (cardData.Protection > 0)
            {
                _protectionGroup.alpha = 1;
            }
        }

        public void AttackAnimation()
        {
            _animator.SetTrigger("attack");
        }

        public virtual void GiveDamage(int amount)
        {
            var attackPower = amount;
            if (Protection > 0)
            {
                var attack = Mathf.Min(attackPower, Protection);
                AddProtection(-attack);
                attackPower -= attack;
            }
            
            AddHp(-attackPower);
        }

        public virtual void AddHp(int amount)
        {
            Hp += amount;
            if (Hp < 1)
            {
                Hp = 0;
            }

            if (Hp > MaxHp)
            {
                Hp = MaxHp;
            }

            if (amount < 0)
            {
                AnimateHp(amount);
            }
            else
            {
                PlayHpEffect();
            }
            _hpText.text = $"{Hp}/{MaxHp}";
        }

        public virtual void AddProtection(int amount)
        {
            Protection += amount;
            if (Protection < 1)
            {
                _protectionGroup.DOFade(0f, 1f);
                Protection = 0;
            }

            if (amount > 0)
            {
                PlayProtectionEffect();
            }
            _protectionText.text = Protection.ToString();
            if (Protection > 0)
            {
                _protectionGroup.DOFade(1f, 1f);
            }
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
            OnDeath?.Invoke(_cardData.CardType);
        }

        private void AnimateHp(int amount)
        {
            var damage = Instantiate(_layoutDamagePrefab, _layoutDamagePrefab.transform.parent);
            damage.AnimateDamage(amount);
        }
        
        public void Highlight(bool state)
        {
            _highlight.SetActive(state);
        }

        public void Blood()
        {
            GiveDamage(Protection+Hp);
            RemoveMob();
            OnGetBlood?.Invoke(_cardData.Blood);
        }

        public int AllHealth()
        {
            return Hp + Protection;
        }

        private void PlayHpEffect()
        {
            foreach (var hpEffect in _hpEffects)
            {
                hpEffect.Play();
            }
        }
        
        private void PlayProtectionEffect()
        {
            foreach (var shieldEffect in _shieldEffects)
            {
                shieldEffect.Play();
            }
        }
    }
}