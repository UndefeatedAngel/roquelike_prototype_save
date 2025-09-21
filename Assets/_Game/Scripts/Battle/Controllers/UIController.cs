using System;
using BlockoutRoguelike.Cards.UI;
using UnityEngine;
using UnityEngine.UI;

namespace BlockoutRoguelike.Battle.Controllers
{
    public class UIController: MonoBehaviour
    {
        [SerializeField] private MainUI _mainUI;
        [SerializeField] private GameObject _winUI;
        [SerializeField] private GameObject _loseUI;
        [SerializeField] private Button _winNextButton;
        [SerializeField] private Button _loseRestartButton;
        [SerializeField] private MoveEnemyUI _moveEnemyUI;

        #region MainUI
        public void UpdateStat(PlayerCardType type, int getStat, int getMaxStat)
        {
            _mainUI.UpdateStat(type, getStat, getMaxStat);
        }

        public void FlyCoin(RectTransform rectTransform, Action action)
        {
            _mainUI.FlyCoin(rectTransform, action);
        }

        public void UpdateCoins(int coins)
        {
            _mainUI.UpdateCoins(coins);
        }

        public void UpdateBlood(int bloodCurrent, int bloodMax)
        {
            _mainUI.UpdateBlood(bloodCurrent, bloodMax);
        }
        #endregion

        public void ShowWin(Action nextAction)
        {
            _winNextButton.onClick.RemoveAllListeners();
            _winNextButton.onClick.AddListener(() =>
            {
                _winUI.gameObject.SetActive(false);
                nextAction?.Invoke();
            });
            _winUI.gameObject.SetActive(true);
        }

        public void ShowLose(Action restartAction)
        {
            _loseRestartButton.onClick.RemoveAllListeners();
            _loseRestartButton.onClick.AddListener(() =>
            {
                _loseUI.gameObject.SetActive(false);
                restartAction?.Invoke();
            });
            _loseUI.gameObject.SetActive(true);
        }

        public void ShowMoveEnemy(int slot, CardData data)
        {
            _moveEnemyUI.gameObject.SetActive(true);
            _moveEnemyUI.Show(slot, data);
        }

        public void HideMoveEnemy()
        {
            _moveEnemyUI.gameObject.SetActive(false);
        }
    }
}