using UnityEngine;
using System.Collections.Generic;
using BlockoutRoguelike.Inventory.Containers;

namespace BlockoutRoguelike.Inventory.UI
{
    public class ContainerUI : MonoBehaviour
    {
        [SerializeField] private GameObject itemSlotUIPrefab;
        [SerializeField] private Transform slotsParent;
        
        private IContainer _container;
        private List<ItemSlotUI> _slotUIs = new List<ItemSlotUI>();
        
        public void Initialize(IContainer container)
        {
            _container = container;
            CreateSlotUIs();
            
            _container.OnContainerChanged += OnContainerChanged;
            _container.OnSlotChanged += OnSlotChanged;
        }
        
        private void CreateSlotUIs()
        {
            // Очищаем старые слоты
            foreach (var slotUI in _slotUIs)
            {
                if (slotUI != null)
                    Destroy(slotUI.gameObject);
            }
            _slotUIs.Clear();
            
            // Создаем новые слоты
            for (int i = 0; i < _container.SlotCount; i++)
            {
                var slotUIObject = Instantiate(itemSlotUIPrefab, slotsParent);
                var slotUI = slotUIObject.GetComponent<ItemSlotUI>();
                
                if (slotUI != null)
                {
                    slotUI.Initialize(_container, i);
                    _slotUIs.Add(slotUI);
                }
            }
        }
        
        private void OnContainerChanged(IContainer container)
        {
            // Обновляем все слоты при изменении контейнера
            foreach (var slotUI in _slotUIs)
            {
                slotUI.UpdateDisplay();
            }
        }
        
        private void OnSlotChanged(IContainer container, int slotIndex)
        {
            // Обновляем конкретный слот
            if (slotIndex >= 0 && slotIndex < _slotUIs.Count)
            {
                _slotUIs[slotIndex].UpdateDisplay();
            }
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        private void OnDestroy()
        {
            if (_container != null)
            {
                _container.OnContainerChanged -= OnContainerChanged;
                _container.OnSlotChanged -= OnSlotChanged;
            }
        }
    }
}