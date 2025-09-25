using System;
using UnityEngine;
using BlockoutRoguelike.Core.Inventory.Data.Items;

namespace BlockoutRoguelike.Core.Inventory.WorldInteraction
{
    public class WorldItem : MonoBehaviour
    {
        [Header("Item Settings")]
        [SerializeField] private BaseItemData _itemData;
        [SerializeField] private int _quantity = 1;
        [SerializeField] private bool _canBePickedUp = true;
        
        public BaseItemData ItemData => _itemData;
        public string ItemId => _itemData?.ItemId ?? string.Empty;
        public int Quantity => _quantity;
        public bool CanBePickedUp => _canBePickedUp;

        private void Start()
        {
            ValidateItemData();
        }

        private void ValidateItemData()
        {
            if (_itemData == null)
            {
                Debug.LogError($"WorldItem {gameObject.name} has no ItemData assigned!", this);
                _canBePickedUp = false;
                return;
            }

            if (_quantity <= 0)
            {
                Debug.LogWarning($"WorldItem {gameObject.name} has invalid quantity: {_quantity}", this);
                _quantity = 1;
            }
        }

        public PickupResult TryPickUp()
        {
            if (!_canBePickedUp) 
                return PickupResult.CreateFailed("Cannot pick up this item");
            
            if (_itemData == null)
                return PickupResult.CreateFailed("Item data is missing");

            return new PickupResult
            {
                IsSuccess = true,
                ItemData = _itemData,
                Quantity = _quantity
            };
        }

        public void OnPickupSuccess()
        {
            _canBePickedUp = false;
            gameObject.SetActive(false);
            Debug.Log($"Picked up: {_itemData.DisplayName} x{_quantity}");
        }

        public void OnPickupFailed(string reason)
        {
            Debug.Log($"Cannot pick up {_itemData?.DisplayName}: {reason}");
        }

        public void Setup(BaseItemData itemData, int quantity = 1, bool canBePickedUp = true)
        {
            _itemData = itemData;
            _quantity = Math.Max(1, quantity);
            _canBePickedUp = canBePickedUp;
            ValidateItemData();
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (_itemData != null)
            {
                gameObject.name = $"{_itemData.DisplayName} (World Item)";
            }
        }
        #endif
    }

    public struct PickupResult
    {
        public bool IsSuccess;
        public BaseItemData ItemData;
        public int Quantity;
        public string ErrorMessage;
        
        public static PickupResult CreateFailed(string error) => new PickupResult 
        { 
            IsSuccess = false, 
            ErrorMessage = error 
        };
    }
}