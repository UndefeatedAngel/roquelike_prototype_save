using UnityEngine;
using BlockoutRoguelike.Inventory.Containers;

namespace BlockoutRoguelike.Inventory.UI
{
    public class ContainerWindow : MonoBehaviour
    {
        [SerializeField] private ContainerUI backpackUI;
        [SerializeField] private QuickSlotsUI quickSlotsWithKeysUI;
        [SerializeField] private QuickSlotsUI quickSlotsWithoutKeysUI;
        
        [SerializeField] private KeyCode toggleKey = KeyCode.I;
        
        private bool _isVisible = false;
        
        public void Initialize(
            BackpackContainer backpack,
            QuickSlotsContainer quickSlotsWithKeys,  // С клавишами 1,2,3
            QuickSlotsContainer quickSlotsWithoutKeys) // Без клавиш
        {
            // Инициализируем UI компоненты
            if (backpackUI != null)
                backpackUI.Initialize(backpack);
            
            if (quickSlotsWithKeysUI != null)
                quickSlotsWithKeysUI.Initialize(quickSlotsWithKeys, true); // С горячими клавишами
            
            if (quickSlotsWithoutKeysUI != null)
                quickSlotsWithoutKeysUI.Initialize(quickSlotsWithoutKeys, false); // Без горячих клавиш
            
            // Скрываем окно при старте
            Hide();
        }
        
        private void Update()
        {
            // Переключение окна по горячей клавише
            if (Input.GetKeyDown(toggleKey))
            {
                Toggle();
            }
        }
        
        public void Show()
        {
            _isVisible = true;
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            _isVisible = false;
            gameObject.SetActive(false);
        }
        
        public void Toggle()
        {
            if (_isVisible)
                Hide();
            else
                Show();
        }
    }
}