using TMPro;
using UnityEngine;

namespace BlockoutRoguelike.Cards.UI
{
    public class MoveEnemyUI: MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        public void Show(string text)
        {
            _text.text = text;
            gameObject.SetActive(true);
        }
    }
}