using UnityEngine;
using BlockoutRoguelike.Core.Person.Models;
using BlockoutRoguelike.Core.Person.UI.Views;

namespace BlockoutRoguelike.Core.UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private HealthUI[] _healthUI;
        [SerializeField] private CoinsUI[] _coinsUI;
        
        public void Initialize(PlayerModel playerModel, bool skipInitialAnimation = true)
        {
            foreach (var healthUI in _healthUI)
            {
                if (healthUI != null)
                {
                    healthUI.Initialize(playerModel.Health, skipInitialAnimation);
                }
            }

            foreach (var coinsUI in _coinsUI)
            {
                if (coinsUI != null)
                {
                    coinsUI.Initialize(playerModel.Coins, skipInitialAnimation);
                }
            }
        }
    }
}