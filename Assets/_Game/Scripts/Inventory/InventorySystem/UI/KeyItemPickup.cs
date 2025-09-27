using UnityEngine;
using UnityEngine.Events;

namespace BlockoutRoguelike.Core.Inventory.InventorySystem.UI
{
    public class KeyItemPickup : MonoBehaviour
    {
        [Header("Item Settings")]
        [SerializeField] private KeyItem item;
        
        [Header("UI Settings")]
        [SerializeField] private GameObject tooltipPrefab;
        [SerializeField] private Vector3 tooltipOffset = new Vector3(0, 2f, 0);
        
        private GameObject tooltipInstance;
        private bool playerInRange = false;
        
        public UnityEvent OnPickup;
        
        private void Start()
        {
            CreateTooltip();
        }
        
        private void CreateTooltip()
        {
            if (tooltipPrefab != null && item != null)
            {
                tooltipInstance = Instantiate(tooltipPrefab, transform.position + tooltipOffset, Quaternion.identity);
                tooltipInstance.transform.SetParent(transform); // Делаем дочерним объектом
                tooltipInstance.SetActive(false);
                
                // Настраиваем текст подсказки
                ItemTooltip tooltip = tooltipInstance.GetComponent<ItemTooltip>();
                if (tooltip != null)
                {
                    tooltip.SetItemInfo(item.Name, item.Description);
                }
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
                ShowTooltip();
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
                HideTooltip();
            }
        }
        
        private void Update()
        {
            if (playerInRange && Input.GetKeyDown(KeyCode.F))
            {
                TryPickupItem();
            }
        }
        
        private void ShowTooltip()
        {
            if (tooltipInstance != null)
            {
                tooltipInstance.SetActive(true);
            }
        }
        
        private void HideTooltip()
        {
            if (tooltipInstance != null)
            {
                tooltipInstance.SetActive(false);
            }
        }
        
        private void TryPickupItem()
        {
            if (InventorySystemController.Instance.AddItem(item))
            {
                Debug.Log($"Подобран предмет: {item.Name}");
                
                // Уничтожаем подсказку и предмет
                if (tooltipInstance != null)
                {
                    Destroy(tooltipInstance);
                }
                
                OnPickup?.Invoke();
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Инвентарь полон!");
            }
        }
        
        private void OnDestroy()
        {
            if (tooltipInstance != null)
            {
                Destroy(tooltipInstance);
            }
        }
        
        // Метод для установки предмета программно
        public void SetItem(KeyItem newItem)
        {
            item = newItem;
            if (tooltipInstance != null)
            {
                ItemTooltip tooltip = tooltipInstance.GetComponent<ItemTooltip>();
                if (tooltip != null)
                {
                    tooltip.SetItemInfo(item.Name, item.Description);
                }
            }
        }
    }
}