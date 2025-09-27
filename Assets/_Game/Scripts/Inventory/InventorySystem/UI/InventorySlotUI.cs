using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NothingAwaits.Core.Inventory.InventorySystem.UI
{
    public class InventorySlotUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TMP_Text _stackText;
        [SerializeField] private Image _backgroundImage;
        
        [Header("Colors")]
        [SerializeField] private Color _emptyColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        [SerializeField] private Color _filledColor = Color.white;
        
        private BaseInventoryItem currentItem;
        
        public void Initialize(BaseInventoryItem item)
        {
            currentItem = item;
            
            if (item != null && _itemIcon != null)
            {
                // Устанавливаем иконку
                _itemIcon.sprite = item.Icon;
                _itemIcon.enabled = true;
                _itemIcon.color = Color.white;
                
                // Устанавливаем текст стака
                if (_stackText != null)
                {
                    if (item.IsStackable && item.CurrentStack > 1)
                    {
                        _stackText.text = item.CurrentStack.ToString();
                        _stackText.enabled = true;
                    }
                    else
                    {
                        _stackText.enabled = false;
                    }
                }
                
                // Меняем цвет фона
                if (_backgroundImage != null)
                {
                    _backgroundImage.color = _filledColor;
                }
            }
            else
            {
                // Очищаем слот
                if (_itemIcon != null)
                {
                    _itemIcon.enabled = false;
                }
                
                if (_stackText != null)
                {
                    _stackText.enabled = false;
                }
                
                if (_backgroundImage != null)
                {
                    _backgroundImage.color = _emptyColor;
                }
            }
        }
        
        public BaseInventoryItem GetItem()
        {
            return currentItem;
        }
        
        public bool IsEmpty()
        {
            return currentItem == null;
        }
        
        // Метод для принудительного обновления отображения
        public void RefreshDisplay()
        {
            Initialize(currentItem);
        }
        
        // Метод для очистки слота
        public void ClearSlot()
        {
            Initialize(null);
        }
    }
}