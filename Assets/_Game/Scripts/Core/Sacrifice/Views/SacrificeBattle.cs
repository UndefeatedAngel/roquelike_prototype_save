using System;
using DG.Tweening;
using UnityEngine;

namespace NothingAwaits.Core.Actor.Views
{
    public class SacrificeBattle: BaseSacrifice
    {
        [SerializeField] private Transform[] _knifes;
        [SerializeField] private ParticleSystem[] _bloodParticles;
        [SerializeField] private GameObject _lightObj;
        
        private PlayerStats _playerStats;
        private Tweener _knifeTween;
        
        public Action OnGetBlood;
        

        public void Set(PlayerStats stats)
        {
            _playerStats = stats;
        }

        public void Highlight(bool state)
        {
            if (_isDeath) return;
            
            Debug.Log(state);
            _lightObj.SetActive(state);
        }

        public void Attack()
        {
            if (_isDeath) return;
            
            if (!_playerStats.IsNeedBlood())
            {
                Debug.Log("Blood Maximum");
                return;
            }
            
            if (_knifeTween != null)
            {
                _knifeTween.Kill();
                Blood();
            }
            _hp--;
            var knife = _knifes[_hp];
            knife.gameObject.SetActive(true);
            var knifePos = knife.localPosition;
            knifePos.z = 0.1f;
            _knifeTween = knife.DOLocalMove(knifePos, 0.5f).OnComplete(() =>
            {
                _knifeTween = null;
                Blood();
            });
        }

        private void Blood()
        {
            var knifePos = _knifes[_hp].transform.localPosition;
            knifePos.z = 0.1f;
            _knifes[_hp].localPosition = knifePos;
            
            _bloodObject[_hp].DOColor(Color.gray, 1f);
            _bloodParticles[_hp].gameObject.SetActive(true);
            OnGetBlood?.Invoke();
            if (_hp < 1)
            {
                DeathAnimation();
            }
        }
    }
}