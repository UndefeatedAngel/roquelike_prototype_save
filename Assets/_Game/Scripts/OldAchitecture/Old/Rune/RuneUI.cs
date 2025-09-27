using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NothingAwaits.Core.Rune
{
    public class RuneUI: MonoBehaviour
    {
        [SerializeField] private Image _typeIconImage;
        [SerializeField] private Image _mainIconImage;
        [SerializeField] private Image _glowImage;
        [SerializeField] private Image _lightImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;

        public void Init(Sprite typeSprite, Sprite mainSprite, Color color, string nameText, string descriptionText)
        {
            name += $"[{nameText}]";
            _typeIconImage.sprite = typeSprite;
            _mainIconImage.gameObject.SetActive(mainSprite!=null);
            _mainIconImage.sprite = mainSprite;
            _glowImage.color = color;
            _lightImage.color = color;
            _nameText.text = nameText;
            _descriptionText.text = descriptionText;
        }
    }
}