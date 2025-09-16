using System;
using BlockoutRoguelike.Core.Rune;
using UnityEngine;
using UnityEngine.UI;

namespace BlockoutRoguelike.Core
{
    public class InventoryUI: MonoBehaviour
    {
        [SerializeField] private RuneUI _runeUIPrefab;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Transform _gridTransform;
        
        private RuneSettings _runeSettings;

        public Action OnHide;
        
        public void Init(RuneSettings runeSettings)
        {
            _runeSettings = runeSettings;
            _closeButton.onClick.AddListener(Hide);
            CreateGrid();
        }

        private void CreateGrid()
        {
            foreach (var main in _runeSettings.RuneObjects)
            {
                var rune = Instantiate(_runeUIPrefab, _gridTransform);
                var runeType = _runeSettings.GetRuneImageType(main.RuneType);
                rune.Init(runeType.TypeSprite, main.MainSprite, runeType.Color, main.Name, main.Description);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            OnHide?.Invoke();
        }
    }
}