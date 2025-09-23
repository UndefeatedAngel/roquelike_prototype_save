using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BlockoutRoguelike.Inventory.Items;
using TMPro;

namespace BlockoutRoguelike.Inventory.UI
{
    public class DragDropSystem : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image dragIcon;
        [SerializeField] private TMP_Text dragStackText;
        
        private ItemSlotUI _draggedSlot;
        private IInventoryItem _draggedItem;
        private bool _isDragging = false;
        
        private void Start()
        {
            dragIcon.gameObject.SetActive(false);
        }
        
        public void BeginDrag(ItemSlotUI slotUI)
        {
            if (!slotUI.Container.TryGetItem(slotUI.SlotIndex, out var item)) return;
            
            _draggedSlot = slotUI;
            _draggedItem = item;
            _isDragging = true;
            
            // Настраиваем иконку перетаскивания
            dragIcon.sprite = null; // TODO: Загрузить иконку предмета
            dragStackText.text = item.IsStackable ? item.CurrentStack.ToString() : "";
            dragIcon.gameObject.SetActive(true);
            
            // Убираем предмет из исходного слота (визуально)
            slotUI.UpdateDisplay();
        }
        
        public void Drag(PointerEventData eventData)
        {
            if (!_isDragging) return;
            
            // Перемещаем иконку за курсором
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out Vector2 localPoint);
            
            dragIcon.transform.localPosition = localPoint;
        }
        
        public void EndDrag(ItemSlotUI targetSlot = null)
        {
            if (!_isDragging) return;
            
            if (targetSlot != null && targetSlot != _draggedSlot)
            {
                // Пытаемся переместить или обменять предметы
                TryMoveOrSwapItems(_draggedSlot, targetSlot);
            }
            else
            {
                // Возвращаем предмет обратно
                ReturnItemToOriginalSlot();
            }
            
            CleanupDrag();
        }
        
        private void TryMoveOrSwapItems(ItemSlotUI fromSlot, ItemSlotUI toSlot)
        {
            var fromContainer = fromSlot.Container;
            var toContainer = toSlot.Container;
            int fromIndex = fromSlot.SlotIndex;
            int toIndex = toSlot.SlotIndex;
            
            // Проверяем, можно ли добавить предмет в целевой слот
            if (toContainer.CanAddItem(_draggedItem, toIndex))
            {
                // Удаляем предмет из исходного контейнера
                if (fromContainer.TryRemoveItem(fromIndex, out var removedItem))
                {
                    // Добавляем в целевой контейнер
                    if (toContainer.TryAddItem(removedItem, toIndex))
                    {
                        // Успешно переместили
                        return;
                    }
                    else
                    {
                        // Не удалось добавить - возвращаем обратно
                        fromContainer.TryAddItem(removedItem, fromIndex);
                    }
                }
            }
            else
            {
                // Попытка обмена предметами
                if (toContainer.TryGetItem(toIndex, out var targetItem))
                {
                    // Меняем предметы местами
                    if (fromContainer.TryRemoveItem(fromIndex, out var fromItem) &&
                        toContainer.TryRemoveItem(toIndex, out var toItem))
                    {
                        if (fromContainer.TryAddItem(toItem, fromIndex) &&
                            toContainer.TryAddItem(fromItem, toIndex))
                        {
                            // Успешный обмен
                            return;
                        }
                        else
                        {
                            // Откатываем изменения при неудаче
                            fromContainer.TryAddItem(fromItem, fromIndex);
                            toContainer.TryAddItem(toItem, toIndex);
                        }
                    }
                }
            }
            
            // Если ничего не получилось - возвращаем предмет
            ReturnItemToOriginalSlot();
        }
        
        private void ReturnItemToOriginalSlot()
        {
            _draggedSlot.Container.TryAddItem(_draggedItem, _draggedSlot.SlotIndex);
        }
        
        private void CleanupDrag()
        {
            _isDragging = false;
            _draggedSlot = null;
            _draggedItem = null;
            dragIcon.gameObject.SetActive(false);
        }
        
        private void Update()
        {
            // Завершаем перетаскивание при отпускании кнопки мыши
            if (_isDragging && Input.GetMouseButtonUp(0))
            {
                var eventData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };
                
                // Ищем целевой слот под курсором
                var results = new System.Collections.Generic.List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);
                
                ItemSlotUI targetSlot = null;
                foreach (var result in results)
                {
                    targetSlot = result.gameObject.GetComponent<ItemSlotUI>();
                    if (targetSlot != null) break;
                }
                
                EndDrag(targetSlot);
            }
            
            // Обновляем позицию иконки при перетаскивании
            if (_isDragging)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    Input.mousePosition,
                    canvas.worldCamera,
                    out Vector2 localPoint);
                
                dragIcon.transform.localPosition = localPoint;
            }
        }
    }
}