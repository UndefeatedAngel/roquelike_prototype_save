using UnityEngine;
using System.Collections.Generic;
using BlockoutRoguelike.Inventory.Containers;

namespace BlockoutRoguelike.Inventory.UI
{
    public class QuickSlotsUI : MonoBehaviour
    {
        [SerializeField] private GameObject itemSlotUIPrefab;
        [SerializeField] private Transform slotsParent;
        [SerializeField] private bool enableHotkeys = true;
        [SerializeField] private KeyCode[] useKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };
        
        private QuickSlotsContainer _quickSlots;
        private List<ItemSlotUI> _slotUIs = new List<ItemSlotUI>();
        
        public void Initialize(QuickSlotsContainer quickSlots, bool enableHotkeys = true)
        {
            this.enableHotkeys = enableHotkeys;
            _quickSlots = quickSlots;
            CreateSlotUIs();
            
            _quickSlots.OnContainerChanged += OnContainerChanged;
            _quickSlots.OnSlotChanged += OnSlotChanged;
            _quickSlots.OnQuickSlotUsed += OnQuickSlotUsed;
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
            
            // Создаем слоты быстрого доступа
            for (int i = 0; i < _quickSlots.SlotCount; i++)
            {
                var slotUIObject = Instantiate(itemSlotUIPrefab, slotsParent);
                var slotUI = slotUIObject.GetComponent<ItemSlotUI>();
                
                if (slotUI != null)
                {
                    slotUI.Initialize(_quickSlots, i);
                    
                    // Добавляем обработчик клика для использования
                    if (enableHotkeys)
                    {
                        slotUI.OnSlotClicked += OnSlotClicked;
                    }
                    _slotUIs.Add(slotUI);
                }
            }
        }
        
        private void Update()
        {
            if (!enableHotkeys) return;
            
            // Обработка горячих клавиш
            for (int i = 0; i < useKeys.Length && i < _quickSlots.SlotCount; i++)
            {
                if (Input.GetKeyDown(useKeys[i]))
                {
                    _quickSlots.TryUseItem(i);
                }
            }
        }
        
        private void OnSlotClicked(ItemSlotUI slotUI)
        {
            // Использование предмета по клику
            _quickSlots.TryUseItem(slotUI.SlotIndex);
        }
        
        private void OnQuickSlotUsed(int slotIndex)
        {
            // Визуальная обратная связь при использовании слота
            if (slotIndex >= 0 && slotIndex < _slotUIs.Count)
            {
                _slotUIs[slotIndex].UpdateDisplay();
            }
        }
        
        private void OnContainerChanged(IContainer container)
        {
            foreach (var slotUI in _slotUIs)
            {
                slotUI.UpdateDisplay();
            }
        }
        
        private void OnSlotChanged(IContainer container, int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < _slotUIs.Count)
            {
                _slotUIs[slotIndex].UpdateDisplay();
            }
        }
        
        private void OnDestroy()
        {
            if (_quickSlots != null)
            {
                _quickSlots.OnContainerChanged -= OnContainerChanged;
                _quickSlots.OnSlotChanged -= OnSlotChanged;
                _quickSlots.OnQuickSlotUsed -= OnQuickSlotUsed;
                
                foreach (var slotUI in _slotUIs)
                {
                    if (slotUI != null && enableHotkeys)
                        slotUI.OnSlotClicked -= OnSlotClicked;
                }
            }
        }
    }
}