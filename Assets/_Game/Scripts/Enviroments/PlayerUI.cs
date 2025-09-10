using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlockoutRoguelike
{
    public class PlayerUI: MonoBehaviour
    {
        [SerializeField] private Image _hpImage;
        [SerializeField] private TMP_Text _bottleText;
        
        private void Start()
        {
            Player.OnHPUpdate = hp =>
            {
                _hpImage.fillAmount = (float)hp / Player.MaxHp;
            };
            Player.OnBottleUpdate = bottle =>
            {
                _bottleText.text = bottle.ToString();
            };
            
            _hpImage.fillAmount = (float)Player.HP / Player.MaxHp;
            _bottleText.text = Player.Bottle.ToString();
        }

        private void OnDestroy()
        {
            Player.OnHPUpdate = null;
            Player.OnBottleUpdate = null;
        }
    }
}