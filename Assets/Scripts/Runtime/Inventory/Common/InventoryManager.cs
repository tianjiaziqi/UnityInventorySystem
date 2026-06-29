using System.Collections.Generic;
using JZQ.InventorySystem.Runtime.Core;
using JZQ.InventorySystem.Runtime.Data;
using JZQ.InventorySystem.Runtime.Inventory.Backpack;
using JZQ.InventorySystem.Runtime.Inventory.Player;
using JZQ.InventorySystem.Runtime.Inventory.QuickBar;
using UnityEngine;
using UnityEngine.Events;

namespace JZQ.InventorySystem.Runtime.Inventory.Common
{
    public class InventoryManager : IInventoryRuntime, IInventoryEventSource, IBackpackReadOnly, IQuickBarReadOnly, IBackpackViewRuntime
    {
        private PlayerInventory playerInventory;
        private BackpackLayoutConfig backpackConfig;
        private QuickBarConfig quickBarConfig;
        private InventoryEventCentre eventCentre;

        public void InvokeEvent(EInventoryEventType eventType)
        {
            eventCentre.InvokeEvent(eventType);
        }

        public void RegisterEvent(EInventoryEventType eventType, UnityAction action)
        {
            eventCentre.RegisterEvent(eventType, action);
        }

        public void UnregisterEvent(EInventoryEventType eventType, UnityAction action)
        {
            eventCentre.UnregisterEvent(eventType, action);
        }

        public InventoryManager(BackpackLayoutConfig backpackConfig, QuickBarConfig quickBarConfig)
        {
            this.backpackConfig = backpackConfig;
            this.quickBarConfig = quickBarConfig;
            eventCentre = new InventoryEventCentre();
            playerInventory = new PlayerInventory(backpackConfig, quickBarConfig, this);
            playerInventory.Initialize();
        }

        public int TryAddItemToPlayer(ItemInstance item) => playerInventory.TryAddItem(item);
        public bool TryMoveItemFromPlayer(string instanceId, int x, int y, bool rotated) => playerInventory.TryMoveItem(instanceId, x, y, rotated);
        public bool TryRemoveItemFromPlayer(string instanceId, out PlacedItem removedItem) => playerInventory.TryRemoveItem(instanceId, out removedItem);
        public IReadOnlyList<PlacedItem> GetPlayerPlacedItems() => playerInventory.GetAllPlacedItems();
        public IReadOnlyList<ItemInstance> GetPlayerItemInstances() => playerInventory.GetAllItemInstances();
        public Dictionary<ItemDefinition, int> GetPlayerItemCounts() => playerInventory.GetAllItemCounts();
        public bool CheckPlayerHasItem(string itemId) => playerInventory.HasItem(itemId);
        public int GetPlayerItemCount(string itemId) => playerInventory.GetItemCount(itemId);
        public List<PlacedItem> GetPlayerItemsById(string itemId) => playerInventory.GetItemsById(itemId);
        public bool CanPlaceItemInPlayer(ItemInstance item, int x, int y, bool rotated) => playerInventory.CanPlaceItem(item, x, y, rotated);
        public bool CanMovePlayerItemTo(string instanceId, int x, int y, bool rotated) => playerInventory.CanMoveItemTo(instanceId, x, y, rotated);
        public bool UnlockPlayerGrid(int x, int y) => playerInventory.UnlockGrid(x, y);
        public IReadOnlyList<Vector2Int> GetPlayerUnlockedSlots() => playerInventory.GetUnlockedSlots();
        public bool IsPlayerGridUnlocked(int x, int y) => playerInventory.IsGridUnlocked(x, y);
        public HashSet<string> GetPlayerItemIds() => playerInventory.GetItemIDs();
        public float GetPlayerMaxWeight() => playerInventory.GetMaxWeight();
        public float GetPlayerCurrentWeight() => playerInventory.GetCurrentWeight();
        public float GetPlayerWeightRatio() => playerInventory.GetWeightRatio();
        public void BindPlayerQuickSlot(int slotIndex, string instanceId) => playerInventory.BindItemToQuickBar(slotIndex, instanceId);
        public void ClearPlayerQuickSlot(int slotIndex) => playerInventory.ClearQuickBarSlot(slotIndex);
        public void SwapPlayerQuickSlots(int a, int b) => playerInventory.SwapQuickBarSlots(a, b);
        public void SetSelectedQuickSlot(int index) => playerInventory.SetQuickBarSelection(index);
        public void SelectNextQuickSlot() => playerInventory.SelectNextQuickSlot();
        public void SelectPreviousQuickSlot() => playerInventory.SelectPreviousQuickSlot();
        public ItemInstance GetSelectedQuickBarItem() => playerInventory.GetSelectedItem();
        public int GetQuickBarSelectedIndex() => playerInventory.GetSelectedIndex();
        public ItemInstance[] GetQuickBarItems() => playerInventory.GetQuickBarItems();
        public bool CanConsumeByInstanceId(string instanceId, int count) => playerInventory.CanConsumeByInstanceId(instanceId, count);
        public void ConsumeByInstanceId(string instanceId, int count) => playerInventory.ConsumeByInstanceId(instanceId, count);
    }
}
