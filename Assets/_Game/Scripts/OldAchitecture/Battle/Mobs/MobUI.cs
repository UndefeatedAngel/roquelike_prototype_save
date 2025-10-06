using System;
using System.Collections;
using DG.Tweening;
using NothingAwaits.Battle.Controllers;
using NothingAwaits.Core.Cards.Data;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace NothingAwaits.Cards.Mobs
{
    public class MobUI: MonoBehaviour, IDragAndReturnSelector
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
        [SerializeField] private TMP_Text _abilityText;
        [SerializeField] private TMP_Text _protectionText;
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _highlight;
        
        [Header("Change(damage, protection, hp)")]
        [SerializeField] private LayoutChange layoutChangePrefab;

        [SerializeField] private GameObject[] _abilityIcons;
        
        [Header("Effects")]
        [SerializeField] private ParticleSystem[] _hpEffects;
        [SerializeField] private ParticleSystem[] _shieldEffects;
        
        public CardType MobType { get; private set; }

        private int _baseAttack;
        private int _additionalAttack;
        public int Attack => _baseAttack + _additionalAttack;
        
        public int Hp { get; private set; }
        public int MaxHp { get; private set; }
        public int Protection { get; private set; }
        
        private int _baseAbilityCount;
        private int _additionalAbilityCount;
        public int AbilityCount => _baseAbilityCount + _additionalAbilityCount;

        public Transform AttackPoint => AttackPoint;

        private CardData _cardData;

        public Action<CardType> OnDeath;
        public Action<int> OnGetBlood;

        private int countCreate;
        private float _rotationSpeed = 5f;
        private bool isBench;

        private Sprite _icon;
        
        private TeamType _team;
        public TeamType Team => _team;
        
        public bool IsBugDebuf { get; set; }

        private void Update()
        {
            UpdateAnimation();
            SmoothRotation();
        }

        public Transform GetMobTransform()
        {
            return _mobTransform;
        }
        
        public void SetMob(TeamType team, string nameText, CardData cardData, Sprite icon)
        {
            _team = team;
            _icon = icon;
            _cardData = cardData;

            MobType = cardData.CardType;
            _baseAttack = cardData.Attack;
            Hp = cardData.Health;
            MaxHp = cardData.Health;
            Protection = cardData.Protection;
            
            _nameText.text = nameText;
            UpdateHpText();
            UpdateAttackText();
            _protectionText.text = cardData.Protection.ToString();
            if (cardData.Protection > 0)
            {
                _protectionGroup.alpha = 1;
            }

            if (cardData.CardType == CardType.Protection || cardData.CardType == CardType.Health)
            {
                _baseAbilityCount = 2;
                UpdateAbilityText();
            }

            if (cardData.CardType != CardType.Empty && cardData.CardType!= CardType.Attack)
            {
                var vA = (int)cardData.CardType - 2;
                for (var i = 0; i < _abilityIcons.Length; i++)
                {
                    _abilityIcons[i].gameObject.SetActive(i == vA);
                }
            }
        }

        private void UpdateHpText()
        {
            _hpText.text = $"{Hp}/{MaxHp}";
        }

        private void UpdateAttackText()
        {
            if (_additionalAttack > 0)
            {
                _attackText.color = Color.green;
            }
            else if (_additionalAttack < 0)
            {
                _attackText.color = Color.red;
            }
            else
            {
                _attackText.color = Color.white;
            }
            
            _attackText.text = Attack.ToString();
        }

        private void UpdateAbilityText()
        {
            if (_additionalAbilityCount > 0)
            {
                _abilityText.color = Color.green;
            }
            else if (_additionalAbilityCount < 0)
            {
                _abilityText.color = Color.red;
            }
            else
            {
                _abilityText.color = Color.white;
            }
            
            _abilityText.text = AbilityCount.ToString();
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
            
            _animator.SetTrigger("hit");
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
                StartCoroutine(PlayHpEffect());
                AnimateChange(TypeChange.Hp, amount);
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
            if (_highlight != null)
            {
                _highlight.SetActive(state);
            }
        }

        public void Blood()
        {
            if (_team == TeamType.Enemy) return;
            if (isBench) return;
            
            GiveDamage(Protection+Hp);
            RemoveMob();
            OnGetBlood?.Invoke(_cardData.Blood);
        }

        private IEnumerator WaitForDestination(Vector3 destination)
        {
            while (Vector3.Distance(transform.position, destination) > 0.1f)
            {
                yield return null;
            }
    
            //_navMeshAgent.isStopped = true;
        }
        
        public void ReturnSlot(Slot slot, Transform returnPoint)
        {
            if(!isBench) return;
            if (_team == TeamType.Enemy) return;
            StopAllCoroutines();
            StartCoroutine(WaitReturnSlot(slot, returnPoint));
        }
        private IEnumerator WaitReturnSlot(Slot slot, Transform returnPoint)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.speed = 5f;
            _navMeshAgent.SetDestination(returnPoint.position);
            transform.parent = slot.transform;
            slot.ActiveMob = this;
            AudioController.Instance.PlayReturnSound();
            yield return new WaitForSeconds(1f);
            var tempTexts = new[]
            {
                "Буду ломать тебя полностью!",
                "Буду защищать из последних сил.",
                "Возможно я могу еще кого-то исцелить!",
                "Превращу тебя в пыль!"
            };
            var number = (int)_cardData.CardType - 1;
            var tempText = tempTexts[number];
            _animatedTextTooltip.ShowText(tempText);
            yield return StartCoroutine(WaitForDestination(returnPoint.position));
            _uiObj.SetActive(true);
            isBench = false;
            yield return new WaitForSeconds(5f);
            AudioController.Instance.HideReturnSound();
        }

        public void Bench(Transform benchPoint)
        {
            if(isBench) return;
            if (_team == TeamType.Enemy) return;
            StopAllCoroutines();
            StartCoroutine(WaitBench(benchPoint));
        }

        private IEnumerator WaitBench(Transform benchPoint)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.speed = 1.5f;
            _navMeshAgent.SetDestination(benchPoint.position);
            StartCoroutine(WaitForDestination(benchPoint.position));
            isBench = true;
            _uiObj.SetActive(false);
            GetComponentInParent<Slot>().ActiveMob = null;
            transform.parent = benchPoint;

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
            if (_animator == null || _navMeshAgent == null) return;
            
            var speed = _navMeshAgent.isStopped? 0f : _navMeshAgent.velocity.magnitude;
            _animator.SetFloat("Speed", speed);
        }
        
        void SmoothRotation()
        {
            if (_animator == null || _navMeshAgent == null) return;
            
            if (_navMeshAgent.velocity.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_navMeshAgent.velocity.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
                    _rotationSpeed * Time.deltaTime);
            }
            else
            {
                transform.eulerAngles = Vector3.zero;
            }
        }

        public int AllHealth()
        {
            return Hp + Protection;
        }
        
        public void PlayHealEffect(MobUI mobUI)
        {
            if (_team == mobUI.Team && MobType == mobUI.MobType) return;
            
            var target = mobUI.GetMobTransform();
            _animator.SetBool("heal", true);
            StartCoroutine(HealRotationRoutine(target));
        }

        private IEnumerator PlayHpEffect()
        {
            _animator.SetBool("powerup", true);
            yield return new WaitForSeconds(1f);
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
        
        private IEnumerator HealRotationRoutine(Transform target)
        {
            Quaternion originalRotation = transform.rotation;
            float rotationDuration = 0.3f;
    
            float timer = 0;
            Vector3 directionToTarget = target.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
    
            while (timer < rotationDuration)
            {
                transform.rotation = Quaternion.Slerp(originalRotation, targetRotation, timer / rotationDuration);
                timer += Time.deltaTime;
                yield return null;
            }
            transform.rotation = targetRotation;
    
            yield return new WaitForSeconds(GetHealAnimationLength());
    
            timer = 0;
            Quaternion currentRotation = transform.rotation;
    
            while (timer < rotationDuration)
            {
                transform.rotation = Quaternion.Slerp(currentRotation, originalRotation, timer / rotationDuration);
                timer += Time.deltaTime;
                yield return null;
            }
            transform.rotation = originalRotation;
    
            _animator.SetBool("heal", false);
        }
        
        private float GetHealAnimationLength()
        {
            AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name.ToLower().Contains("heal"))
                {
                    return clip.length;
                }
            }
            return 1f;
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

        public void Buf()
        {
            var ability = Random.Range(0, 2) == 1;
            if (ability)
            {
                _additionalAbilityCount++;
                UpdateAbilityText();
                if (MobType == CardType.Health)
                {
                    AnimateChange(TypeChange.Hp, 1);
                }
                else if (MobType == CardType.Protection)
                {
                    AnimateChange(TypeChange.Protection, 1);
                }
            }
            else
            {
                _additionalAttack++;
                UpdateAttackText();
                AnimateChange(TypeChange.Attack, 1);
            }
        }

        public void DeBuf()
        {
            var ability = Random.Range(0, 2) == 1;
            if (ability)
            {
                _additionalAbilityCount--;
                if (_baseAbilityCount + _additionalAbilityCount < 0)
                {
                    _additionalAbilityCount++;
                }
                else
                {
                    if (MobType == CardType.Health)
                    {
                        AnimateChange(TypeChange.Hp, -1);
                    }
                    else if (MobType == CardType.Protection)
                    {
                        AnimateChange(TypeChange.Protection, -1);
                    }
                    UpdateAbilityText();
                }
            }
            else
            {
                _additionalAttack--;
                if (_baseAttack + _additionalAttack < 0)
                {
                    _additionalAttack++;
                }
                else
                {
                    AnimateChange(TypeChange.Attack, -1);
                    UpdateAttackText();
                }
            }
            
        }

        public void Highlight()
        {
            if (isBench) return;
            
            Highlight(true);
        }

        public void Clear()
        {
            Highlight(false);
        }

        public Sprite GetIcon()
        {
            return _icon;
        }

        public bool IsBenchMob()
        {
            return isBench;
        }
    }
}