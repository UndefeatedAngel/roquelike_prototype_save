using System;
using System.Collections;
using BlockoutRoguelike.Battle.Controllers;
using BlockoutRoguelike.Core.Inventory.InventorySystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace BlockoutRoguelike.Cards.Mobs
{
    public class MobUI: MonoBehaviour
    {
        [SerializeField] private Transform _mobTransform;
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

        [SerializeField] private GameObject[] _abilityIcons;
        
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
        
        private TeamType _team;
        public TeamType Team => _team;

        private void Update()
        {
            if (!isBench) return;
            
            UpdateAnimation();
            SmoothRotation();
        }

        public Transform GetMobTransform()
        {
            return _mobTransform;
        }
        
        public void SetMob(TeamType team, string nameText, CardData cardData, Transform benchPoint, Sprite icon)
        {
            _team = team;
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

            if (cardData.CardType != PlayerCardType.Empty && cardData.CardType!= PlayerCardType.Attack)
            {
                var vA = (int)cardData.CardType - 2;
                for (var i = 0; i < _abilityIcons.Length; i++)
                {
                    _abilityIcons[i].gameObject.SetActive(i == vA);
                }
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
                StartCoroutine(PlayProtectionEffect());
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

        public virtual void AddMage(int amount, MobUI target)
        {
            
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
            var offsetAnimation = 25f;
            var damage = Instantiate(layoutChangePrefab, layoutChangePrefab.transform.parent);
            damage.Init(offsetAnimation);
            damage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, damage.DefaultPosY+countCreate*offsetAnimation);
            damage.AnimateChange(typeChange, amount, () =>
            {
                countCreate = 0;
            });
            countCreate++;
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
            if (_animator != null && _navMeshAgent!=null)
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
        
        private IEnumerator PlayProtectionEffect()
        {
            _animator.SetBool("powerup", true);
            yield return new WaitForSeconds(1f);
            foreach (var shieldEffect in _shieldEffects)
            {
                shieldEffect.Play();
            }
        }
        
        public void WinDance()
        {
            _animator.SetBool("dance", true);
        }

        public void StartMove()
        {
            _uiObj.gameObject.SetActive(false);
        }

        public void EndMove()
        {
            _uiObj.gameObject.SetActive(true);
        }
    }
}