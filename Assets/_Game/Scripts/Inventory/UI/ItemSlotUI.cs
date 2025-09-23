using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using BlockoutRoguelike.Inventory.Containers;

namespace BlockoutRoguelike.Inventory.UI
{
    public class ItemSlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image itemIcon;
        [SerializeField] private TMP_Text stackText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Sprite emptySlotSprite;
        [SerializeField] private Sprite filledSlotSprite;
        
        public IContainer Container { get; private set; }
        public int SlotIndex { get; private set; }
        
        public event System.Action<ItemSlotUI> OnSlotClicked;
        public event System.Action<ItemSlotUI> OnSlotBeginDrag;
        public event System.Action<ItemSlotUI> OnSlotDrag;
        public event System.Action<ItemSlotUI> OnSlotEndDrag;
        
        public void Initialize(IContainer container, int slotIndex)
        {
            Container = container;
            SlotIndex = slotIndex;
            
            Container.OnSlotChanged += OnContainerSlotChanged;
            UpdateDisplay();
        }
        
        public void UpdateDisplay() // ДЕЛАЕМ PUBLIC
        {
            if (Container.TryGetItem(SlotIndex, out var item))
            {
                // TODO: Загрузка иконки предмета
                itemIcon.gameObject.SetActive(true);
                stackText.text = item.IsStackable ? item.CurrentStack.ToString() : "";
                backgroundImage.sprite = filledSlotSprite;
            }
            else
            {
                itemIcon.gameObject.SetActive(false);
                stackText.text = "";
                backgroundImage.sprite = emptySlotSprite;
            }
        }
        
        private void OnContainerSlotChanged(IContainer container, int slotIndex)
        {
            if (slotIndex == SlotIndex)
            {
                UpdateDisplay();
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            OnSlotClicked?.Invoke(this);
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (Container.TryGetItem(SlotIndex, out _))
            {
                OnSlotBeginDrag?.Invoke(this);
            }
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            OnSlotDrag?.Invoke(this);
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            OnSlotEndDrag?.Invoke(this);
        }
        
        private void OnDestroy()
        {
            if (Container != null)
            {
                Container.OnSlotChanged -= OnContainerSlotChanged;
            }
        }
    }
}