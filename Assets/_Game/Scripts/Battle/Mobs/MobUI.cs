using System;
using System.Collections;
using BlockoutRoguelike.Battle.Controllers;
using BlockoutRoguelike.Core.Inventory.InventorySystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace BlockoutRoguelike.Cards.Mobs
{
    public class MobUI: MonoBehaviour
    {
        [SerializeField] private Transform _attackPoint;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AnimatedTextTooltip _animatedTextTooltip;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private CanvasGroup _protectionGroup;

        [SerializeField] private GameObject _uiObj;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _hpText;
        [SerializeField] private TMP_Text _attackText;
        [SerializeField] private TMP_Text _protectionText;
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _highlight;
        
        [Header("Change(damage, protection, hp)")]
        [SerializeField] private LayoutChange layoutChangePrefab;
        
        [Header("Effects")]
        [SerializeField] private ParticleSystem[] _hpEffects;
        [SerializeField] private ParticleSystem[] _shieldEffects;
        
        public PlayerCardType MobType { get; private set; }
        public int Attack { get; private set; }
        public int Hp { get; private set; }
        public int MaxHp { get; private set; }
        public int Protection { get; private set; }

        public Transform AttackPoint => AttackPoint;

        private CardData _cardData;

        public Action<PlayerCardType> OnDeath;
        public Action<int> OnGetBlood;

        private int countCreate;

        [SerializeField] private Transform _benchPoint;
        private float _rotationSpeed = 5f;
        private bool isBench;

        private Sprite _icon;

        private void Update()
        {
            if (!isBench) return;
            
            UpdateAnimation();
            SmoothRotation();
        }
        
        public void SetMob(string nameText, CardData cardData, Transform benchPoint, Sprite icon)
        {
            _icon = icon;
            _benchPoint = benchPoint;
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
                AnimateChange(TypeChange.Hp, amount);
            }
            else if(amount>0)
            {
                AnimateChange(TypeChange.Hp, amount);
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
                AnimateChange(TypeChange.Protection, amount);
            }
            else if (amount < 0)
            {
                AnimateChange(TypeChange.Protection, amount);
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

        private void AnimateChange(TypeChange typeChange, int amount)
        {
            var damage = Instantiate(layoutChangePrefab, layoutChangePrefab.transform.parent);
            damage.AnimateChange(typeChange, amount, () =>
            {
                countCreate = 0;
            });
            countCreate++;
            damage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, countCreate*0.25f);
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

        public void Bench()
        {
            StartCoroutine(WaitBench());
        }

        private IEnumerator WaitBench()
        {
            if (!InventorySystemController.Instance.HasSpace())
            {
                //TODO: Hint no space
                yield break;
            }
            _navMeshAgent.SetDestination(_benchPoint.position);
            isBench = true;
            _uiObj.SetActive(false);
            InventorySystemController.Instance.AddItem(new GenericImageItem(_icon, Hp));
            GetComponentInParent<Slot>().ActiveMob = null;

            AudioController.Instance.PlayLeaveSound();
            yield return new WaitForSeconds(1f);
            var tempTexts = new[]
            {
                "Мой мускул устал! Иду качать другой!",
                "Мой долг велит мне жить. Вам — нет.",
                "Вы останетесь здесь гнить без меня!",
                "Прощайте. Иду призвать нечто...более послушное!"
            };
            var number = (int)_cardData.CardType - 1;
            var tempText = tempTexts[number];
            _animatedTextTooltip.ShowText(tempText);
            yield return new WaitForSeconds(5f);
            AudioController.Instance.HideLeaveSound();
        }
        
        void UpdateAnimation()
        {
            if (_animator != null)
            {
                // Скорость для анимации движения
                float speed = _navMeshAgent.velocity.magnitude / _navMeshAgent.speed;
                _animator.SetFloat("Speed", speed);
            }
        }
        
        void SmoothRotation()
        {
            if (_navMeshAgent.velocity.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_navMeshAgent.velocity.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
                    _rotationSpeed * Time.deltaTime);
            }
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
        
        public void WinDance()
        {
            _animator.SetBool("dance", true);
        }
    }
}