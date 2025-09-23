using System;
using BlockoutRoguelike.Inventory.Containers;
using BlockoutRoguelike.Inventory.Items;

namespace BlockoutRoguelike.Inventory.Services
{
    public class InventoryService
    {
        public QuickSlotsContainer QuickSlotsWithKeys { get; private set; }
        public QuickSlotsContainer QuickSlotsWithoutKeys { get; private set; }
        public BackpackContainer Backpack { get; private set; }
        public SacrificeContainer SacrificeSlots { get; private set; }
        public BenchContainer Bench { get; private set; }
        
        public event Action OnInventoryInitialized;
        public event Action<string> OnItemAdded;
        public event Action<string> OnItemRemoved;
        
        public InventoryService(
            int quickSlotsCount = 3,
            int backpackSlotsCount = 20,
            int sacrificeSlotsCount = 3,
            int benchSlotsCount = 6)
        {
            InitializeContainers(quickSlotsCount, backpackSlotsCount, sacrificeSlotsCount, benchSlotsCount);
        }
        
        private void InitializeContainers(int quickSlots, int backpack, int sacrifice, int bench)
        {
            QuickSlotsWithKeys = new QuickSlotsContainer(quickSlots);
            QuickSlotsWithoutKeys = new QuickSlotsContainer(quickSlots);
            Backpack = new BackpackContainer(backpack);
            SacrificeSlots = new SacrificeContainer(sacrifice);
            Bench = new BenchContainer(bench);
            
            SubscribeToContainerEvents(QuickSlotsWithKeys);
            SubscribeToContainerEvents(QuickSlotsWithoutKeys);
            SubscribeToContainerEvents(Backpack);
            SubscribeToContainerEvents(SacrificeSlots);
            SubscribeToContainerEvents(Bench);
            
            OnInventoryInitialized?.Invoke();
        }
        
        private void SubscribeToContainerEvents(IContainer container)
        {
            container.OnSlotChanged += (cont, slot) => OnContainerChanged(cont, slot);
            container.OnContainerChanged += OnContainerChanged;
        }
        
        private void OnContainerChanged(IContainer container, int slotIndex = -1)
        {
        }
        
        private void OnContainerChanged(IContainer container)
        {
        }
        
        public bool TryAddItemToAnyContainer(IInventoryItem item)
        {
            if (QuickSlotsWithKeys.TryAddItem(item))
            {
                OnItemAdded?.Invoke(item.ItemId);
                return true;
            }
            
            if (QuickSlotsWithoutKeys.TryAddItem(item))
            {
                OnItemAdded?.Invoke(item.ItemId);
                return true;
            }
            
            if (Backpack.TryAddItem(item))
            {
                OnItemAdded?.Invoke(item.ItemId);
                return true;
            }
            
            return false;
        }
        
        public bool TryAddPotion(PotionItem potion)
        {
            return TryAddItemToAnyContainer(potion);
        }
        
        public bool TryAddKey(KeyItem key)
        {
            return TryAddItemToAnyContainer(key);
        }
        
        public bool TryAddSacrifice(SacrificeItem sacrifice)
        {
            if (SacrificeSlots.TryAddItem(sacrifice))
            {
                OnItemAdded?.Invoke(sacrifice.ItemId);
                return true;
            }
            
            if (Backpack.TryAddItem(sacrifice))
            {
                OnItemAdded?.Invoke(sacrifice.ItemId);
                return true;
            }
            
            return false;
        }
        
        public int GetTotalItemCount(string itemId)
        {
            int total = 0;
            total += QuickSlotsWithKeys.GetTotalItemCount(itemId);
            total += QuickSlotsWithoutKeys.GetTotalItemCount(itemId);
            total += Backpack.GetTotalItemCount(itemId);
            total += SacrificeSlots.GetTotalItemCount(itemId);
            total += Bench.GetTotalItemCount(itemId);
            return total;
        }
        
        public bool HasItem(string itemId)
        {
            return GetTotalItemCount(itemId) > 0;
        }
        
        public bool TryUseQuickSlot(int slotIndex, bool useKeysContainer = true)
        {
            var container = useKeysContainer ? QuickSlotsWithKeys : QuickSlotsWithoutKeys;
            return container.TryUseItem(slotIndex);
        }
        
        public void UpdateBenchHealing(float deltaTime)
        {
            Bench.UpdateHealing(deltaTime);
        }
        
        public void SaveInventory()
        {
            // TODO: Реализация сохранения
        }
        
        public void LoadInventory()
        {
            // TODO: Реализация загрузки
        }
    }
}