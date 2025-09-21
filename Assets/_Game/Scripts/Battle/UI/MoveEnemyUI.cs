using BlockoutRoguelike.Battle.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlockoutRoguelike.Cards.UI
{
    public class MoveEnemyUI: MonoBehaviour
    {
        //TODO:
        [SerializeField] private Sprite[] _iconSprites;
        [SerializeField] private Color[] _colorsMob;
        [SerializeField] private GameObject[] _mobsGameobjects;
        [SerializeField] private Image[] _mobImages;
        [SerializeField] private Image[] _iconImages;
        [SerializeField] private TMP_Text[] _textIcons;
        public void Show(int slot, CardData data)
        {
            for (int i = 0; i < _mobsGameobjects.Length; i++)
            {
                _mobsGameobjects[i].SetActive(false);
                if (i == slot)
                {
                    _mobsGameobjects[i].SetActive(true);
                    var colorIndex = (int)data.CardType - 1;
                    _mobImages[i].color = _colorsMob[colorIndex];
                    _iconImages[i].sprite = _iconSprites[colorIndex];
                    _textIcons[i].text = data.CardType == PlayerCardType.Attack ? $"{data.Attack}" : "2";
                }
            }
        }
    }
}