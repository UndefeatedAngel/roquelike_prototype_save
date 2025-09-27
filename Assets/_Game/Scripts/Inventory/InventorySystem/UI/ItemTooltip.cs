using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NothingAwaits.Core.Inventory.InventorySystem.UI
{
    public class ItemTooltip : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private TMP_Text itemDescriptionText;
        [SerializeField] private TMP_Text interactText;
        [SerializeField] private Image backgroundImage;
        
        [Header("Settings")]
        [SerializeField] private string interactMessage = "Нажмите F чтобы подобрать";
        [SerializeField] private float followSpeed = 10f;
        
        private Transform target;
        private Vector3 offset;
        
        private void Start()
        {
            // Находим основной камеру для Billboard эффекта
            if (Camera.main != null)
            {
                target = Camera.main.transform;
            }
            
            // Устанавливаем текст взаимодействия
            if (interactText != null)
            {
                interactText.text = interactMessage;
            }
        }
        
        private void Update()
        {
            // Следим за родительским объектом
            if (transform.parent != null)
            {
                transform.position = transform.parent.position + offset;
            }
            
            // Поворачиваем к камере (Billboard эффект)
            if (target != null)
            {
                transform.LookAt(transform.position + target.forward);
            }
        }
        
        public void SetItemInfo(string name, string description)
        {
            if (itemNameText != null)
            {
                itemNameText.text = name;
            }
            
            if (itemDescriptionText != null)
            {
                itemDescriptionText.text = description;
            }
        }
        
        public void SetOffset(Vector3 newOffset)
        {
            offset = newOffset;
            transform.position = transform.parent.position + offset;
        }
        
        public void SetInteractMessage(string message)
        {
            if (interactText != null)
            {
                interactText.text = message;
            }
        }
        
        // Метод для изменения цвета фона
        public void SetBackgroundColor(Color color)
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = color;
            }
        }
        
        // Метод для изменения цвета текста
        public void SetTextColor(Color nameColor, Color descriptionColor)
        {
            if (itemNameText != null)
            {
                itemNameText.color = nameColor;
            }
            
            if (itemDescriptionText != null)
            {
                itemDescriptionText.color = descriptionColor;
            }
        }
    }
}