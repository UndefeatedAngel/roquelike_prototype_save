using BlockoutRoguelike.Core.Actor.Models;
using BlockoutRoguelike.Core.Actor.UI.Views;
using UnityEngine;

namespace BlockoutRoguelike.Core.UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private HealthView[] _healthViews;
        [SerializeField] private CoinsView[] _coinsViews;
        
        public void Initialize(PlayerModel playerModel, bool skipInitialAnimation = true)
        {
            foreach (var healthView in _healthViews)
            {
                if (healthView != null)
                {
                    healthView.Initialize(playerModel.Health, skipInitialAnimation);
                }
            }

            foreach (var coinsView in _coinsViews)
            {
                if (coinsView != null)
                {
                    coinsView.Initialize(playerModel.Coins, skipInitialAnimation);
                }
            }
        }
    }
}