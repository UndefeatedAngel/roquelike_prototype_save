using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BlockoutRoguelike.Core.Inventory.UI.Views
{
    public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI Elements")]
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TMP_Text _stackText;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _highlightImage;
        
        [Header("Colors")]
        [SerializeField] private Color _emptyColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        [SerializeField] private Color _filledColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        [SerializeField] private Color _highlightColor = new Color(1f, 1f, 0f, 0.3f);
        
        private int _slotIndex;
        private System.Action<int> _onClick;
        private System.Action<int> _onHoverEnter;
        private System.Action<int> _onHoverExit;
        private bool _isHighlighted = false;

        public void Initialize(int slotIndex, System.Action<int> onClick, System.Action<int> onHoverEnter, System.Action<int> onHoverExit)
        {
            _slotIndex = slotIndex;
            _onClick = onClick;
            _onHoverEnter = onHoverEnter;
            _onHoverExit = onHoverExit;
            
            // Инициализируем подсветку
            if (_highlightImage != null)
            {
                _highlightImage.enabled = false;
                _highlightImage.color = _highlightColor;
            }
            
            ClearSlot();
        }

        public void UpdateSlot(Sprite itemSprite, int quantity, string itemName)
        {
            if (itemSprite != null)
            {
                // Заполненный слот
                if (_itemIcon != null)
                {
                    _itemIcon.sprite = itemSprite;
                    _itemIcon.enabled = true;
                    _itemIcon.color = Color.white;
                }
                
                if (_stackText != null)
                {
                    if (quantity > 1)
                    {
                        _stackText.text = quantity.ToString();
                        _stackText.enabled = true;
                    }
                    else
                    {
                        _stackText.enabled = false;
                    }
                }
                
                if (_backgroundImage != null)
                {
                    _backgroundImage.color = _filledColor;
                }
            }
            else
            {
                // Пустой слот
                ClearSlot();
            }
            
            UpdateHighlight();
        }

        public void ClearSlot()
        {
            if (_itemIcon != null)
            {
                _itemIcon.enabled = false;
                _itemIcon.sprite = null;
            }
            
            if (_stackText != null)
            {
                _stackText.enabled = false;
            }
            
            if (_backgroundImage != null)
            {
                _backgroundImage.color = _emptyColor;
            }
            
            UpdateHighlight();
        }

        public void SetHighlight(bool highlighted)
        {
            _isHighlighted = highlighted;
            UpdateHighlight();
        }

        private void UpdateHighlight()
        {
            if (_highlightImage != null)
            {
                _highlightImage.enabled = _isHighlighted;
            }
        }

        // UI Events
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"Slot {_slotIndex} clicked");
            _onClick?.Invoke(_slotIndex);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log($"Slot {_slotIndex} hover enter");
            _onHoverEnter?.Invoke(_slotIndex);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log($"Slot {_slotIndex} hover exit");
            _onHoverExit?.Invoke(_slotIndex);
        }
    }
}