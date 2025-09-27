using UnityEngine;

namespace NothingAwaits.Core.Player
{
    public partial class PlayerController
    {
#if UNITY_EDITOR
        [SerializeField] private int _testCoinAddAmount = 50;
        [SerializeField] private int _testCoinSpendAmount = 30;

        public void TestAddCoins()
        {
            if (_playerModel == null) return;
            Debug.Log($"Before add coins: {_playerModel.Coins.Amount}");
            AddCoins(_testCoinAddAmount);
            Debug.Log($"After add coins: {_playerModel.Coins.Amount}");
        }

        public void TestSpendCoins()
        {
            if (_playerModel == null) return;
            Debug.Log($"Before spend coins: {_playerModel.Coins.Amount}");
            bool success = TrySpendCoins(_testCoinSpendAmount);
            Debug.Log($"Spend result: {success}, After spend: {_playerModel.Coins.Amount}");
        }
#endif
    }
}