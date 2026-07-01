using System.Collections.Generic;
using JZQ.InventorySystem.Runtime.Core;
using JZQ.InventorySystem.Runtime.Data;
using JZQ.InventorySystem.Runtime.Inventory.Backpack;
using JZQ.InventorySystem.Runtime.Inventory.Internal;
using JZQ.InventorySystem.Runtime.Inventory.QuickBar;
using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Inventory.Player
{
    /// <summary>
    /// Encapsulates player-facing inventory, backpack, and quick bar behavior.
    /// </summary>
    public class PlayerInventory
    {
        private InventoryGrid mainGrid;
        private float maxWeight;
        private QuickBarModel quickBar;

        private BackpackLayoutConfig backpackConfig;
        private QuickBarConfig quickBarConfig;

        private float currentWeight;
        
        private IInventoryEventSource events;

        /// <summary>
        /// Creates a player inventory runtime from the provided configuration.
        /// </summary>
        /// <param name="backpackConfig">The backpack layout configuration.</param>
        /// <param name="quickBarConfig">The quick bar configuration.</param>
        /// <param name="events">The event source used to dispatch runtime updates.</param>
        public PlayerInventory(BackpackLayoutConfig backpackConfig, QuickBarConfig quickBarConfig, IInventoryEventSource events)
        {
            this.backpackConfig = backpackConfig;
            this.quickBarConfig = quickBarConfig;
            mainGrid = new InventoryGrid(backpackConfig.MaxSize.x, backpackConfig.MaxSize.y);
            maxWeight = backpackConfig.InitialWeight;
            this.events = events;
            quickBar = new QuickBarModel(quickBarConfig.slotCount, events);
        }

        /// <summary>
        /// Attempts to add an item instance to the player's inventory.
        /// </summary>
        /// <param name="item">The item instance to add.</param>
        /// <returns>The amount that could not be added.</returns>
        public int TryAddItem(ItemInstance item)
        {
            var initialCount = item.StackCount;
            var left = mainGrid.TryAdd(item);
            if (initialCount != left)
            {
                RecalculateCurrentWeight();
                events.InvokeEvent(EInventoryEventType.InventoryChange);
            }

            return left;
        }

        /// <summary>
        /// Attempts to remove an item instance from the player's inventory.
        /// </summary>
        /// <param name="instanceId">The item instance identifier.</param>
        /// <param name="removedItem">The removed placed item when successful.</param>
        /// <returns><c>true</c> if the item was removed; otherwise, <c>false</c>.</returns>
        public bool TryRemoveItem(string instanceId, out PlacedItem removedItem)
        {
            if (mainGrid.TryRemove(instanceId, out removedItem))
            {
                RecalculateCurrentWeight();
                UpdateQuickBar();
                events.InvokeEvent(EInventoryEventType.InventoryChange);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to move an item to the specified grid position.
        /// </summary>
        /// <param name="instanceId">The item instance identifier.</param>
        /// <param name="x">The target grid x coordinate.</param>
        /// <param name="y">The target grid y coordinate.</param>
        /// <param name="rotated">Whether the item is rotated.</param>
        /// <returns><c>true</c> if the move succeeded; otherwise, <c>false</c>.</returns>
        public bool TryMoveItem(string instanceId, int x, int y, bool rotated)
        {
            if (mainGrid.TryMove(instanceId, x, y, rotated))
            {
                events.InvokeEvent(EInventoryEventType.InventoryChange);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to unlock the specified backpack cell.
        /// </summary>
        /// <param name="x">The grid x coordinate.</param>
        /// <param name="y">The grid y coordinate.</param>
        /// <returns><c>true</c> if the cell was unlocked; otherwise, <c>false</c>.</returns>
        public bool UnlockGrid(int x, int y)
        {
            if (mainGrid.IsCellUnlocked(x, y))
            {
                return false;
            }
            mainGrid.UnlockCell(x, y);
            events.InvokeEvent(EInventoryEventType.InventoryUnlockChange);
            return true;
        }

        /// <summary>
        /// Unlocks the initial backpack area defined by the layout configuration.
        /// </summary>
        public void Initialize()
        {
            for (int y = 0; y < backpackConfig.InitialSize.y; y++)
            {
                for (int x = 0; x < backpackConfig.InitialSize.x; x++)
                {
                    mainGrid.UnlockCell(x, y);
                }
            }
        }

        private float GetItemWeight(ItemInstance item)
        {
            return item.Definition.Weight * item.StackCount;
        }

        private void RecalculateCurrentWeight()
        {
            float weight = 0;
            foreach (var item in mainGrid.GetAllPlacedItems())
            {
                weight += GetItemWeight(item.InstanceItem);
            }
            currentWeight = weight;
        }

        /// <summary>
        /// Gets the current weight ratio relative to maximum carry capacity.
        /// </summary>
        /// <returns>The current weight ratio.</returns>
        public float GetWeightRatio()
        {
            if (maxWeight <= 0)
            {
                Debug.LogError("[Inventory] Backpack configuration is invalid because the maximum carry weight is zero.");
                return 0;
            }
            return currentWeight / maxWeight;
        }

        /// <summary>
        /// Gets the maximum carry weight.
        /// </summary>
        /// <returns>The maximum carry weight.</returns>
        public float GetMaxWeight() => maxWeight;

        /// <summary>
        /// Gets the current carried weight.
        /// </summary>
        /// <returns>The current carried weight.</returns>
        public float GetCurrentWeight() => currentWeight;

        /// <summary>
        /// Gets all placed items currently stored in the backpack.
        /// </summary>
        /// <returns>A read-only list of placed items.</returns>
        public IReadOnlyList<PlacedItem> GetAllPlacedItems() => mainGrid.GetAllPlacedItems();

        /// <summary>
        /// Gets all runtime item instances currently stored in the backpack.
        /// </summary>
        /// <returns>A read-only list of item instances.</returns>
        public IReadOnlyList<ItemInstance> GetAllItemInstances() => mainGrid.GetAllItemInstances();

        /// <summary>
        /// Gets aggregated item counts grouped by item definition.
        /// </summary>
        /// <returns>A dictionary containing counts per item definition.</returns>
        public Dictionary<ItemDefinition, int> GetAllItemCounts() => mainGrid.GetAllItemCounts();

        /// <summary>
        /// Returns whether the player inventory contains at least one item of the specified type.
        /// </summary>
        /// <param name="itemId">The item type identifier.</param>
        /// <returns><c>true</c> if the player has the item type; otherwise, <c>false</c>.</returns>
        public bool HasItem(string itemId) => mainGrid.GetItemCount(itemId) > 0;

        /// <summary>
        /// Gets the total count of the specified item type.
        /// </summary>
        /// <param name="itemId">The item type identifier.</param>
        /// <returns>The total item count.</returns>
        public int GetItemCount(string itemId) => mainGrid.GetItemCount(itemId);

        /// <summary>
        /// Gets all placed items matching the specified item type.
        /// </summary>
        /// <param name="itemId">The item type identifier.</param>
        /// <returns>A list of matching placed items.</returns>
        public List<PlacedItem> GetItemsById(string itemId) => mainGrid.FindItems(itemId);

        /// <summary>
        /// Checks whether an item can be placed at the specified grid position.
        /// </summary>
        /// <param name="item">The item to validate.</param>
        /// <param name="x">The target grid x coordinate.</param>
        /// <param name="y">The target grid y coordinate.</param>
        /// <param name="rotated">Whether the item is rotated.</param>
        /// <returns><c>true</c> if placement is valid; otherwise, <c>false</c>.</returns>
        public bool CanPlaceItem(ItemInstance item, int x, int y, bool rotated) => mainGrid.CanPlace(item, x, y, rotated);

        /// <summary>
        /// Checks whether an existing item can be moved to the specified grid position.
        /// </summary>
        /// <param name="instanceId">The item instance identifier.</param>
        /// <param name="x">The target grid x coordinate.</param>
        /// <param name="y">The target grid y coordinate.</param>
        /// <param name="rotated">Whether the item is rotated.</param>
        /// <returns><c>true</c> if the move is valid; otherwise, <c>false</c>.</returns>
        public bool CanMoveItemTo(string instanceId, int x, int y, bool rotated)
        {
            PlacedItem item = mainGrid.GetPlacedItem(instanceId);
            if (item == null) return false;

            return mainGrid.CanPlace(item.InstanceItem, x, y, rotated, instanceId);
        }

        /// <summary>
        /// Gets all unlocked grid cell coordinates.
        /// </summary>
        /// <returns>A read-only list of unlocked cell coordinates.</returns>
        public IReadOnlyList<Vector2Int> GetUnlockedSlots() => mainGrid.GetUnlockedSlots();

        /// <summary>
        /// Returns whether the specified grid cell is unlocked.
        /// </summary>
        /// <param name="x">The grid x coordinate.</param>
        /// <param name="y">The grid y coordinate.</param>
        /// <returns><c>true</c> if the cell is unlocked; otherwise, <c>false</c>.</returns>
        public bool IsGridUnlocked(int x, int y) => mainGrid.IsCellUnlocked(x, y);

        /// <summary>
        /// Gets all item instance identifiers currently stored in the inventory.
        /// </summary>
        /// <returns>A set containing all item instance identifiers.</returns>
        public HashSet<string> GetItemIDs()
        {
            var currentItems = GetAllPlacedItems();
            HashSet<string> currentIds = new();

            foreach (var placedItem in currentItems)
            {
                currentIds.Add(placedItem.InstanceItem.InstanceID);
            }
            return currentIds;
        }

        /// <summary>
        /// Binds an item instance to a quick bar slot.
        /// </summary>
        /// <param name="index">The target quick slot index.</param>
        /// <param name="instanceId">The item instance identifier.</param>
        public void BindItemToQuickBar(int index, string instanceId)
        {
            quickBar.BindItem(index, instanceId);
        }

        /// <summary>
        /// Clears the specified quick bar slot.
        /// </summary>
        /// <param name="index">The quick slot index to clear.</param>
        public void ClearQuickBarSlot(int index)
        {
            quickBar.ClearSlot(index);
        }

        /// <summary>
        /// Removes invalid quick bar bindings that no longer reference items in the backpack.
        /// </summary>
        public void UpdateQuickBar()
        {
            string[] ids = quickBar.GetAllItemIds();
            for (int i = 0; i < ids.Length; i++)
            {
                if (mainGrid.GetItemInstance(ids[i]) == null)
                {
                    ClearQuickBarSlot(i);
                }
            }
        }

        /// <summary>
        /// Gets the currently selected quick bar item.
        /// </summary>
        /// <returns>The selected item instance, or <c>null</c> if the slot is empty.</returns>
        public ItemInstance GetSelectedItem()
        {
            string id = quickBar.GetSelectedItemId();
            if (id == "") return null;
            return mainGrid.GetItemInstance(id);
        }

        /// <summary>
        /// Swaps two quick bar slots.
        /// </summary>
        /// <param name="index1">The first quick slot index.</param>
        /// <param name="index2">The second quick slot index.</param>
        public void SwapQuickBarSlots(int index1, int index2)
        {
            quickBar.SwapSlots(index1, index2);
        }

        /// <summary>
        /// Sets the selected quick bar slot.
        /// </summary>
        /// <param name="index">The quick slot index to select.</param>
        public void SetQuickBarSelection(int index)
        {
            quickBar.SetSelectedIndex(index);
        }

        /// <summary>
        /// Selects the next quick bar slot.
        /// </summary>
        public void SelectNextQuickSlot()
        {
            quickBar.SelectNext();
        }

        /// <summary>
        /// Selects the previous quick bar slot.
        /// </summary>
        public void SelectPreviousQuickSlot()
        {
            quickBar.SelectPrevious();
        }

        /// <summary>
        /// Gets the selected quick bar slot index.
        /// </summary>
        /// <returns>The selected quick slot index.</returns>
        public int GetSelectedIndex()
        {
            return quickBar.GetSelectedIndex();
        }

        /// <summary>
        /// Gets the current quick bar item snapshot.
        /// </summary>
        /// <returns>An array representing the current quick bar items.</returns>
        public ItemInstance[] GetQuickBarItems()
        {
            string[] ids = quickBar.GetAllItemIds();
            ItemInstance[] result = new ItemInstance[ids.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                var ins = mainGrid.GetItemInstance(ids[i]);
                result[i] = ins;
            }

            return result;
        }

        /// <summary>
        /// Checks whether the specified amount can be consumed from an item instance.
        /// </summary>
        /// <param name="instanceId">The item instance identifier.</param>
        /// <param name="count">The amount to consume.</param>
        /// <returns><c>true</c> if the amount can be consumed; otherwise, <c>false</c>.</returns>
        public bool CanConsumeByInstanceId(string instanceId, int count)
        {
            if (count <= 0) return false;
            ItemInstance instance = mainGrid.GetItemInstance(instanceId);
            if (instance == null) return false;
            if (instance.StackCount < count) return false;
            return true;
        }

        /// <summary>
        /// Consumes the specified amount from an item instance.
        /// </summary>
        /// <param name="instanceId">The item instance identifier.</param>
        /// <param name="count">The amount to consume.</param>
        public void ConsumeByInstanceId(string instanceId, int count)
        {
            if (count <= 0) return;
            ItemInstance instance = mainGrid.GetItemInstance(instanceId);
            if (instance == null) return;
            if (instance.StackCount - count <= 0)
            {
                mainGrid.Remove(instanceId);
                UpdateQuickBar();
            }
            else
            {
                instance.StackCount -= count;
            }

            RecalculateCurrentWeight();
            events.InvokeEvent(EInventoryEventType.QuickBarChanged);
            events.InvokeEvent(EInventoryEventType.InventoryChange);
        }

        /// <summary>
        /// Attempts to drop a number of items from the specified stack.
        /// </summary>
        /// <param name="instanceId">The source item instance identifier.</param>
        /// <param name="count">The number of items to drop.</param>
        /// <param name="droppedItem">The dropped item instance when successful.</param>
        /// <returns><c>true</c> if the drop succeeded; otherwise, <c>false</c>.</returns>
        public bool TryDropItem(string instanceId, int count, out ItemInstance droppedItem)
        {
            droppedItem = null;
            if (count <= 0) return false;
            var item = mainGrid.GetItemInstance(instanceId);
            if(item == null || item.StackCount < count) return false;
            if(!mainGrid.TrySplitStack(instanceId, count, out var instance)) return false;
            droppedItem = instance;
            UpdateQuickBar();
            RecalculateCurrentWeight();
            events.InvokeEvent(EInventoryEventType.InventoryChange);
            events.InvokeEvent(EInventoryEventType.QuickBarChanged);
            
            return true;
        }

        /// <summary>
        /// Attempts to split a number of items from the specified stack.
        /// </summary>
        /// <param name="instanceId">The source item instance identifier.</param>
        /// <param name="splitCount">The number of items to split.</param>
        /// <param name="newStack">The newly created split stack when successful.</param>
        /// <returns><c>true</c> if the stack was split; otherwise, <c>false</c>.</returns>
        public bool TrySplitStack(string instanceId, int splitCount, out ItemInstance newStack)
        {
            if(!mainGrid.TrySplitStack(instanceId, splitCount, out newStack)) return false;
            UpdateQuickBar();
            events.InvokeEvent(EInventoryEventType.InventoryChange);
            events.InvokeEvent(EInventoryEventType.QuickBarChanged);
            return true;
        }

        /// <summary>
        /// Attempts to split a stack and place the split result at the target grid position.
        /// </summary>
        /// <param name="sourceInstanceId">The source item instance identifier.</param>
        /// <param name="splitCount">The number of items to split.</param>
        /// <param name="x">The target grid x coordinate.</param>
        /// <param name="y">The target grid y coordinate.</param>
        /// <param name="rotated">Whether the split stack is rotated.</param>
        /// <returns><c>true</c> if the split placement succeeded; otherwise, <c>false</c>.</returns>
        public bool TrySplitPlaceItem(string sourceInstanceId, int splitCount, int x, int y, bool rotated)
        {
            if (splitCount <= 0) return false;

            var source = mainGrid.GetItemInstance(sourceInstanceId);
            if (source == null || source.StackCount <= splitCount) return false;

            var splitItem = new ItemInstance(source.Definition, System.Guid.NewGuid().ToString(), splitCount);
            if (!mainGrid.CanPlace(splitItem, x, y, rotated)) return false;

            source.StackCount -= splitCount;
            if (!mainGrid.TryPlace(splitItem, x, y, rotated))
            {
                source.StackCount += splitCount;
                return false;
            }

            events.InvokeEvent(EInventoryEventType.InventoryChange);
            events.InvokeEvent(EInventoryEventType.QuickBarChanged);
            return true;
        }
        
        /// <summary>
        /// Attempts to merge one stack into another stack.
        /// </summary>
        /// <param name="sourceId">The source item instance identifier.</param>
        /// <param name="targetId">The target item instance identifier.</param>
        /// <returns><c>true</c> if the merge succeeded; otherwise, <c>false</c>.</returns>
        public bool TryMergeStack(string sourceId, string targetId)
        {
            if (!mainGrid.TryMergeStack(sourceId, targetId)) return false;
            UpdateQuickBar();
            events.InvokeEvent(EInventoryEventType.InventoryChange);
            events.InvokeEvent(EInventoryEventType.QuickBarChanged);
            return true;
        }

        /// <summary>
        /// Attempts to split a partial stack and merge it into another stack.
        /// </summary>
        /// <param name="sourceInstanceId">The source item instance identifier.</param>
        /// <param name="splitCount">The number of items to split and merge.</param>
        /// <param name="targetInstanceId">The target item instance identifier.</param>
        /// <returns><c>true</c> if the split merge succeeded; otherwise, <c>false</c>.</returns>
        public bool TryMergeSplitItems(string sourceInstanceId, int splitCount, string targetInstanceId)
        {
            if (splitCount <= 0) return false;

            var source = mainGrid.GetItemInstance(sourceInstanceId);
            var target = mainGrid.GetItemInstance(targetInstanceId);
            if (source == null || target == null) return false;
            if (source.InstanceID == target.InstanceID) return false;
            if (source.Definition != target.Definition) return false;
            if (source.StackCount <= splitCount) return false;
            if (target.StackCount + splitCount > target.Definition.MaxStack) return false;

            source.StackCount -= splitCount;
            target.StackCount += splitCount;

            UpdateQuickBar();
            events.InvokeEvent(EInventoryEventType.InventoryChange);
            events.InvokeEvent(EInventoryEventType.QuickBarChanged);
            return true;
        }

        /// <summary>
        /// Attempts to merge the item at one grid position into the item at another grid position.
        /// </summary>
        /// <param name="sourceX">The source grid x coordinate.</param>
        /// <param name="sourceY">The source grid y coordinate.</param>
        /// <param name="targetX">The target grid x coordinate.</param>
        /// <param name="targetY">The target grid y coordinate.</param>
        /// <returns><c>true</c> if the merge succeeded; otherwise, <c>false</c>.</returns>
        public bool TryMergeToGridPosition(int sourceX, int sourceY, int targetX, int targetY)
        {
            var source = mainGrid.GetItemAt(sourceX, sourceY);
            var target = mainGrid.GetItemAt(targetX, targetY);
            if (source == null || target == null) return false;
            if (!mainGrid.TryMergeStack(source.InstanceItem.InstanceID, target.InstanceItem.InstanceID)) return false;
            UpdateQuickBar();
            events.InvokeEvent(EInventoryEventType.InventoryChange);
            events.InvokeEvent(EInventoryEventType.QuickBarChanged);
            return true;
        }
    }
}
