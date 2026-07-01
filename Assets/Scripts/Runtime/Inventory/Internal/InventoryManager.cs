using System.Collections.Generic;
using JZQ.InventorySystem.Runtime.Core;
using JZQ.InventorySystem.Runtime.Data;
using JZQ.InventorySystem.Runtime.Inventory.Backpack;
using JZQ.InventorySystem.Runtime.Inventory.Player;
using JZQ.InventorySystem.Runtime.Inventory.QuickBar;
using UnityEngine;
using UnityEngine.Events;

namespace JZQ.InventorySystem.Runtime.Inventory.Internal
{
    /// <summary>
    /// Default runtime facade that coordinates inventory, backpack, quick bar, and event services.
    /// </summary>
    public class InventoryManager : IInventoryRuntime, IInventoryEventSource, IBackpackReadOnly, IQuickBarReadOnly, IBackpackViewRuntime, IBackpackCommandRuntime
    {
        private PlayerInventory playerInventory;
        private BackpackLayoutConfig backpackConfig;
        private QuickBarConfig quickBarConfig;
        private InventoryEventCentre eventCentre;

        /// <summary>
        /// Dispatches the specified inventory event.
        /// </summary>
        /// <param name="eventType">The inventory event type to dispatch.</param>
        public void InvokeEvent(EInventoryEventType eventType)
        {
            eventCentre.InvokeEvent(eventType);
        }

        /// <summary>
        /// Registers a listener for the specified inventory event type.
        /// </summary>
        /// <param name="eventType">The inventory event type.</param>
        /// <param name="action">The listener to register.</param>
        public void RegisterEvent(EInventoryEventType eventType, UnityAction action)
        {
            eventCentre.RegisterEvent(eventType, action);
        }

        /// <summary>
        /// Unregisters a listener from the specified inventory event type.
        /// </summary>
        /// <param name="eventType">The inventory event type.</param>
        /// <param name="action">The listener to unregister.</param>
        public void UnregisterEvent(EInventoryEventType eventType, UnityAction action)
        {
            eventCentre.UnregisterEvent(eventType, action);
        }

        /// <summary>
        /// Creates a runtime inventory manager with the provided system configuration.
        /// </summary>
        /// <param name="backpackConfig">The backpack layout configuration.</param>
        /// <param name="quickBarConfig">The quick bar configuration.</param>
        public InventoryManager(BackpackLayoutConfig backpackConfig, QuickBarConfig quickBarConfig)
        {
            this.backpackConfig = backpackConfig;
            this.quickBarConfig = quickBarConfig;
            eventCentre = new InventoryEventCentre();
            playerInventory = new PlayerInventory(backpackConfig, quickBarConfig, this);
            playerInventory.Initialize();
        }

        /// <summary>
        /// Attempts to add an item instance to the player inventory.
        /// </summary>
        /// <param name="item">The item instance to add.</param>
        /// <returns>The amount that could not be added.</returns>
        public int TryAddItemToPlayer(ItemInstance item) => playerInventory.TryAddItem(item);

        /// <summary>
        /// Attempts to move an existing player item to the specified grid position.
        /// </summary>
        /// <param name="instanceId">The item instance identifier.</param>
        /// <param name="x">The target grid x coordinate.</param>
        /// <param name="y">The target grid y coordinate.</param>
        /// <param name="rotated">Whether the item is rotated.</param>
        /// <returns><c>true</c> if the move succeeded; otherwise, <c>false</c>.</returns>
        public bool TryMoveItemFromPlayer(string instanceId, int x, int y, bool rotated) => playerInventory.TryMoveItem(instanceId, x, y, rotated);

        /// <summary>
        /// Attempts to remove an item instance from the player inventory.
        /// </summary>
        /// <param name="instanceId">The item instance identifier.</param>
        /// <param name="removedItem">The removed placed item when the operation succeeds.</param>
        /// <returns><c>true</c> if the item was removed; otherwise, <c>false</c>.</returns>
        public bool TryRemoveItemFromPlayer(string instanceId, out PlacedItem removedItem) => playerInventory.TryRemoveItem(instanceId, out removedItem);

        /// <summary>
        /// Gets all items currently placed in the player backpack.
        /// </summary>
        /// <returns>A read-only list of placed items.</returns>
        public IReadOnlyList<PlacedItem> GetPlayerPlacedItems() => playerInventory.GetAllPlacedItems();

        /// <summary>
        /// Gets all runtime item instances currently stored in the player backpack.
        /// </summary>
        /// <returns>A read-only list of item instances.</returns>
        public IReadOnlyList<ItemInstance> GetPlayerItemInstances() => playerInventory.GetAllItemInstances();

        /// <summary>
        /// Gets aggregated item counts grouped by item definition.
        /// </summary>
        /// <returns>A dictionary containing counts per item definition.</returns>
        public Dictionary<ItemDefinition, int> GetPlayerItemCounts() => playerInventory.GetAllItemCounts();

        /// <summary>
        /// Returns whether the player has at least one item of the specified type.
        /// </summary>
        /// <param name="itemId">The item type identifier.</param>
        /// <returns><c>true</c> if the player has the item type; otherwise, <c>false</c>.</returns>
        public bool CheckPlayerHasItem(string itemId) => playerInventory.HasItem(itemId);

        /// <summary>
        /// Gets the total count of the specified item type.
        /// </summary>
        /// <param name="itemId">The item type identifier.</param>
        /// <returns>The total item count.</returns>
        public int GetPlayerItemCount(string itemId) => playerInventory.GetItemCount(itemId);

        /// <summary>
        /// Gets all placed items matching the specified item type.
        /// </summary>
        /// <param name="itemId">The item type identifier.</param>
        /// <returns>A list of matching placed items.</returns>
        public List<PlacedItem> GetPlayerItemsById(string itemId) => playerInventory.GetItemsById(itemId);
        /// <summary>
        /// Checks whether an item can be placed at the specified grid position.
        /// </summary>
        /// <param name="item">The item to validate.</param>
        /// <param name="x">The target grid x coordinate.</param>
        /// <param name="y">The target grid y coordinate.</param>
        /// <param name="rotated">Whether the item is rotated.</param>
        /// <returns><c>true</c> if placement is valid; otherwise, <c>false</c>.</returns>
        public bool CanPlaceItemInPlayer(ItemInstance item, int x, int y, bool rotated) => playerInventory.CanPlaceItem(item, x, y, rotated);

        /// <summary>
        /// Checks whether an existing player item can be moved to the specified grid position.
        /// </summary>
        /// <param name="instanceId">The item instance identifier.</param>
        /// <param name="x">The target grid x coordinate.</param>
        /// <param name="y">The target grid y coordinate.</param>
        /// <param name="rotated">Whether the item is rotated.</param>
        /// <returns><c>true</c> if the move is valid; otherwise, <c>false</c>.</returns>
        public bool CanMovePlayerItemTo(string instanceId, int x, int y, bool rotated) => playerInventory.CanMoveItemTo(instanceId, x, y, rotated);

        /// <summary>
        /// Attempts to unlock the specified grid cell.
        /// </summary>
        /// <param name="x">The grid x coordinate.</param>
        /// <param name="y">The grid y coordinate.</param>
        /// <returns><c>true</c> if the cell was unlocked; otherwise, <c>false</c>.</returns>
        public bool UnlockPlayerGrid(int x, int y) => playerInventory.UnlockGrid(x, y);

        /// <summary>
        /// Gets all unlocked backpack grid coordinates.
        /// </summary>
        /// <returns>A read-only list of unlocked cell coordinates.</returns>
        public IReadOnlyList<Vector2Int> GetPlayerUnlockedSlots() => playerInventory.GetUnlockedSlots();
        /// <summary>
        /// Returns whether the specified grid cell is unlocked.
        /// </summary>
        /// <param name="x">The grid x coordinate.</param>
        /// <param name="y">The grid y coordinate.</param>
        /// <returns><c>true</c> if the cell is unlocked; otherwise, <c>false</c>.</returns>
        public bool IsPlayerGridUnlocked(int x, int y) => playerInventory.IsGridUnlocked(x, y);

        /// <summary>
        /// Gets all item instance identifiers currently stored by the player.
        /// </summary>
        /// <returns>A set containing all item instance identifiers.</returns>
        public HashSet<string> GetPlayerItemIds() => playerInventory.GetItemIDs();
        /// <summary>
        /// Gets the player's maximum carry weight.
        /// </summary>
        /// <returns>The maximum carry weight.</returns>
        public float GetPlayerMaxWeight() => playerInventory.GetMaxWeight();

        /// <summary>
        /// Gets the player's current carried weight.
        /// </summary>
        /// <returns>The current carried weight.</returns>
        public float GetPlayerCurrentWeight() => playerInventory.GetCurrentWeight();

        /// <summary>
        /// Gets the current weight ratio relative to the maximum carry weight.
        /// </summary>
        /// <returns>The current weight ratio.</returns>
        public float GetPlayerWeightRatio() => playerInventory.GetWeightRatio();

        /// <summary>
        /// Binds an item instance to the specified quick slot.
        /// </summary>
        /// <param name="slotIndex">The target quick slot index.</param>
        /// <param name="instanceId">The item instance identifier.</param>
        public void BindPlayerQuickSlot(int slotIndex, string instanceId) => playerInventory.BindItemToQuickBar(slotIndex, instanceId);

        /// <summary>
        /// Clears the specified quick slot.
        /// </summary>
        /// <param name="slotIndex">The quick slot index to clear.</param>
        public void ClearPlayerQuickSlot(int slotIndex) => playerInventory.ClearQuickBarSlot(slotIndex);

        /// <summary>
        /// Swaps two quick bar slots.
        /// </summary>
        /// <param name="a">The first quick slot index.</param>
        /// <param name="b">The second quick slot index.</param>
        public void SwapPlayerQuickSlots(int a, int b) => playerInventory.SwapQuickBarSlots(a, b);
        /// <summary>
        /// Sets the currently selected quick slot.
        /// </summary>
        /// <param name="index">The quick slot index to select.</param>
        public void SetSelectedQuickSlot(int index) => playerInventory.SetQuickBarSelection(index);

        /// <summary>
        /// Selects the next quick slot.
        /// </summary>
        public void SelectNextQuickSlot() => playerInventory.SelectNextQuickSlot();

        /// <summary>
        /// Selects the previous quick slot.
        /// </summary>
        public void SelectPreviousQuickSlot() => playerInventory.SelectPreviousQuickSlot();

        /// <summary>
        /// Gets the item currently selected in the quick bar.
        /// </summary>
        /// <returns>The selected item instance, or <c>null</c> if the slot is empty.</returns>
        public ItemInstance GetSelectedQuickBarItem() => playerInventory.GetSelectedItem();

        /// <summary>
        /// Gets the index of the currently selected quick slot.
        /// </summary>
        /// <returns>The selected quick slot index.</returns>
        public int GetQuickBarSelectedIndex() => playerInventory.GetSelectedIndex();

        /// <summary>
        /// Gets the current quick bar item snapshot.
        /// </summary>
        /// <returns>An array representing the current quick bar items.</returns>
        public ItemInstance[] GetQuickBarItems() => playerInventory.GetQuickBarItems();

        /// <summary>
        /// Checks whether the specified amount can be consumed from an item instance.
        /// </summary>
        /// <param name="instanceId">The item instance identifier.</param>
        /// <param name="count">The amount to consume.</param>
        /// <returns><c>true</c> if the amount can be consumed; otherwise, <c>false</c>.</returns>
        public bool CanConsumeByInstanceId(string instanceId, int count) => playerInventory.CanConsumeByInstanceId(instanceId, count);

        /// <summary>
        /// Consumes the specified amount from an item instance.
        /// </summary>
        /// <param name="instanceId">The item instance identifier.</param>
        /// <param name="count">The amount to consume.</param>
        public void ConsumeByInstanceId(string instanceId, int count) => playerInventory.ConsumeByInstanceId(instanceId, count);
        /// <summary>
        /// Attempts to drop a number of items from the specified stack.
        /// </summary>
        /// <param name="instanceId">The source item instance identifier.</param>
        /// <param name="count">The number of items to drop.</param>
        /// <param name="droppedItem">The dropped item instance when the operation succeeds.</param>
        /// <returns><c>true</c> if the drop succeeded; otherwise, <c>false</c>.</returns>
        public bool TryDropItem(string instanceId, int count, out ItemInstance droppedItem) => playerInventory.TryDropItem(instanceId, count, out droppedItem);

        /// <summary>
        /// Attempts to split a number of items from the specified stack.
        /// </summary>
        /// <param name="instanceId">The source item instance identifier.</param>
        /// <param name="splitCount">The number of items to split.</param>
        /// <param name="newStack">The created split stack when the operation succeeds.</param>
        /// <returns><c>true</c> if the split succeeded; otherwise, <c>false</c>.</returns>
        public bool TrySplitStack(string instanceId, int splitCount, out ItemInstance newStack) => playerInventory.TrySplitStack(instanceId, splitCount, out newStack);

        /// <summary>
        /// Attempts to split a stack and place the split result at the target grid position.
        /// </summary>
        /// <param name="sourceInstanceId">The source item instance identifier.</param>
        /// <param name="splitCount">The number of items to split.</param>
        /// <param name="x">The target grid x coordinate.</param>
        /// <param name="y">The target grid y coordinate.</param>
        /// <param name="rotated">Whether the split stack is rotated.</param>
        /// <returns><c>true</c> if the split placement succeeded; otherwise, <c>false</c>.</returns>
        public bool TrySplitPlaceItem(string sourceInstanceId, int splitCount, int x, int y, bool rotated) => playerInventory.TrySplitPlaceItem(sourceInstanceId, splitCount, x, y, rotated);

        /// <summary>
        /// Attempts to merge one stack into another stack.
        /// </summary>
        /// <param name="sourceInstanceId">The source item instance identifier.</param>
        /// <param name="targetInstanceId">The target item instance identifier.</param>
        /// <returns><c>true</c> if the merge succeeded; otherwise, <c>false</c>.</returns>
        public bool TryMergeItems(string sourceInstanceId, string targetInstanceId) => playerInventory.TryMergeStack(sourceInstanceId, targetInstanceId);

        /// <summary>
        /// Attempts to split a partial stack and merge it into another stack.
        /// </summary>
        /// <param name="sourceInstanceId">The source item instance identifier.</param>
        /// <param name="splitCount">The number of items to split and merge.</param>
        /// <param name="targetInstanceId">The target item instance identifier.</param>
        /// <returns><c>true</c> if the split merge succeeded; otherwise, <c>false</c>.</returns>
        public bool TryMergeSplitItems(string sourceInstanceId, int splitCount, string targetInstanceId) => playerInventory.TryMergeSplitItems(sourceInstanceId, splitCount, targetInstanceId);
    }
}
