using UnityEngine;
using System.Collections.Generic;
using BlockoutRoguelike.Core.Inventory.UI.Presenters;

namespace BlockoutRoguelike.Core.Inventory.UI.Views
{
    public class InventoryView : MonoBehaviour
    {
        [Header("Slot Prefab")]
        [SerializeField] private GameObject _slotPrefab;
        
        private List<InventorySlotUI> _slots = new List<InventorySlotUI>();
        private InventoryPresenter _presenter;

        public int SlotCount => _slots.Count;
        public IReadOnlyList<InventorySlotUI> Slots => _slots;

        private void Start()
        {
            _presenter = GetComponent<InventoryPresenter>();
        }

        public void CreateSlots(int inventorySize)
        {
            ClearAllSlots();
            
            if (_slotPrefab == null)
            {
                Debug.LogError("Slot prefab is not assigned!");
                return;
            }

            for (int i = 0; i < inventorySize; i++)
            {
                CreateSlot(i);
            }
            
            Debug.Log($"Created {inventorySize} inventory slots");
        }

        private void CreateSlot(int slotIndex)
        {
            var slotObject = Instantiate(_slotPrefab, transform);
            slotObject.name = $"InventorySlot_{slotIndex}";
            
            var slotUI = slotObject.GetComponent<InventorySlotUI>();
            if (slotUI != null)
            {
                slotUI.Initialize(slotIndex, OnSlotClicked, OnSlotHoverEnter, OnSlotHoverExit);
                _slots.Add(slotUI);
            }
            else
            {
                Debug.LogError("Slot prefab doesn't have InventorySlotUI component!");
            }
        }

        public void UpdateSlot(int slotIndex, Sprite icon, int quantity, string itemName)
        {
            if (!IsValidSlotIndex(slotIndex)) 
            {
                Debug.LogError($"Invalid slot index: {slotIndex}");
                return;
            }
            
            _slots[slotIndex].UpdateSlot(icon, quantity, itemName);
        }

        public void ClearSlot(int slotIndex)
        {
            if (!IsValidSlotIndex(slotIndex)) return;
            _slots[slotIndex].ClearSlot();
        }

        public void HighlightSlot(int slotIndex, bool highlight)
        {
            if (!IsValidSlotIndex(slotIndex)) return;
            _slots[slotIndex].SetHighlight(highlight);
        }

        public void ClearAllSlots()
        {
            foreach (var slot in _slots)
            {
                if (slot != null && slot.gameObject != null)
                    Destroy(slot.gameObject);
            }
            _slots.Clear();
        }

        private bool IsValidSlotIndex(int slotIndex)
        {
            bool isValid = slotIndex >= 0 && slotIndex < _slots.Count;
            if (!isValid) Debug.LogError($"Invalid slot index: {slotIndex}. Total slots: {_slots.Count}");
            return isValid;
        }

        private void OnSlotClicked(int slotIndex)
        {
            Debug.Log($"View: Slot {slotIndex} clicked");
            _presenter?.OnSlotClicked(slotIndex);
        }

        private void OnSlotHoverEnter(int slotIndex)
        {
            _presenter?.OnSlotHoverEnter(slotIndex);
        }

        private void OnSlotHoverExit(int slotIndex)
        {
            _presenter?.OnSlotHoverExit(slotIndex);
        }
    }
}