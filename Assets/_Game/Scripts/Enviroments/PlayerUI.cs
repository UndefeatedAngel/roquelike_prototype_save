using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlockoutRoguelike
{
    public class PlayerUI: MonoBehaviour
    {
        [SerializeField] private TMP_Text _bottleText;
        
        private void Start()
        {
            Player.OnBottleUpdate = bottle =>
            {
                _bottleText.text = bottle.ToString();
            };
            
            _bottleText.text = Player.Bottle.ToString();
        }

        private void OnDestroy()
        {
            Player.OnHPUpdate = null;
            Player.OnBottleUpdate = null;
        }
    }
}