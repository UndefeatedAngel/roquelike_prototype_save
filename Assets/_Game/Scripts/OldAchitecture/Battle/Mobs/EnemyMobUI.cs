using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BlockoutRoguelike.Cards.Mobs
{
    public class EnemyMobUI: MobUI
    {
        [SerializeField] private RectTransform _coinPrefab;
        public Action<RectTransform> OnCoinDrop;
        public override void GiveDamage(int amount)
        {
            base.GiveDamage(amount);
            
            StartCoroutine(DropEnemy(0));
        }

        private IEnumerator DropEnemy(float time)
        {
            yield return new WaitForSeconds(time);
            var coin = Instantiate(_coinPrefab, _coinPrefab.transform.parent);
            coin.gameObject.SetActive(true);
            coin.transform.parent = null;
            var xMove = Random.Range(-0.5f, 0.5f);
            var yMove = -0.5f;
            var newPos = coin.anchoredPosition3D + new Vector3(xMove, yMove, 0);
            yield return new WaitForEndOfFrame();
            coin.DOJump(newPos, 0.5f, 1, 0.5f).OnComplete(() =>
            {
                OnCoinDrop?.Invoke(coin);
            });
        }
        
    }
}