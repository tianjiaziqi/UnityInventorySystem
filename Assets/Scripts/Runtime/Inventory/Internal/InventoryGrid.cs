using System;
using System.Collections.Generic;
using JZQ.InventorySystem.Runtime.Data;
using JZQ.InventorySystem.Runtime.Inventory.Backpack;
using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Inventory.Internal
{
    /// <summary>
    /// Stores placed items and validates backpack grid operations.
    /// </summary>
    public class InventoryGrid
    {
        /// <summary>
        /// Gets the width of the grid in cells.
        /// </summary>
        public int GridWidth { get; private set; }

        /// <summary>
        /// Gets the height of the grid in cells.
        /// </summary>
        public int GridHeight { get; private set; }
        private List<PlacedItem> items = new();
        private HashSet<Vector2Int> unlockedSlots = new();

        /// <summary>
        /// Creates a grid with the specified dimensions.
        /// </summary>
        /// <param name="width">The grid width in cells.</param>
        /// <param name="height">The grid height in cells.</param>
        public InventoryGrid(int width, int height)
        {
            GridWidth = width;
            GridHeight = height;
        }

        /// <summary>
        /// Checks whether an item can be placed at the specified grid position.
        /// </summary>
        /// <param name="item">The item to validate.</param>
        /// <param name="x">The target grid x coordinate.</param>
        /// <param name="y">The target grid y coordinate.</param>
        /// <param name="rotated">Whether the item is rotated.</param>
        /// <param name="ignoreInstanceId">An optional instance identifier to ignore during overlap checks.</param>
        /// <returns><c>true</c> if placement is valid; otherwise, <c>false</c>.</returns>
        public bool CanPlace(ItemInstance item, int x, int y, bool rotated, string ignoreInstanceId = null)
        {
            if (rotated && !item.Definition.CanRotate) return false;
            int width = rotated ? item.Definition.Height : item.Definition.Width;
            int height = rotated ? item.Definition.Width : item.Definition.Height;

            if (x < 0 || y < 0) return false;

            for (int i = x; i < x + width; i++)
            {
                for (int j = y; j < y + height; j++)
                {
                    if (!unlockedSlots.Contains(new Vector2Int(i, j))) return false;
                }
            }

            if (x + width > GridWidth || y + height > GridHeight) return false;
            foreach (var other in items)
            {
                if (other.InstanceItem.InstanceID == ignoreInstanceId) continue;
                if (RectOverlap(x, y, width, height, other.Position.x, other.Position.y, other.Width, other.Height))
                {
                    return false;
                }
            }

            return true;
        }

        private bool RectOverlap(int ax, int ay, int aw, int ah, int bx, int by, int bw, int bh)
        {
            return ax < bx + bw &&
                   ax + aw > bx &&
                   ay < by + bh &&
                   ay + ah > by;
        }

        /// <summary>
        /// Unlocks the specified grid cell.
        /// </summary>
        /// <param name="x">The grid x coordinate.</param>
        /// <param name="y">The grid y coordinate.</param>
        public void UnlockCell(int x, int y)
        {
            if (x >= GridWidth || y >= GridHeight)
            {
                Debug.LogError($"[Backpack] Trying to unlock cell at {x}, {y} but it's out of bounds");
                return;
            }

            if (!unlockedSlots.Contains(new Vector2Int(x, y)))
            {
                unlockedSlots.Add(new Vector2Int(x, y));
            }
        }

        /// <summary>
        /// Returns whether the specified grid cell is unlocked.
        /// </summary>
        /// <param name="x">The grid x coordinate.</param>
        /// <param name="y">The grid y coordinate.</param>
        /// <returns><c>true</c> if the cell is unlocked; otherwise, <c>false</c>.</returns>
        public bool IsCellUnlocked(int x, int y)
        {
            return unlockedSlots.Contains(new Vector2Int(x, y));
        }

        /// <summary>
        /// Attempts to place an item at the specified grid position.
        /// </summary>
        /// <param name="item">The item to place.</param>
        /// <param name="x">The target grid x coordinate.</param>
        /// <param name="y">The target grid y coordinate.</param>
        /// <param name="rotated">Whether the item is rotated.</param>
        /// <returns><c>true</c> if the item was placed; otherwise, <c>false</c>.</returns>
        public bool TryPlace(ItemInstance item, int x, int y, bool rotated)
        {
            if (!CanPlace(item, x, y, rotated)) return false;
            items.Add(new PlacedItem(item, x, y, rotated));
            return true;
        }

        /// <summary>
        /// Attempts to add an item to the grid, merging with compatible stacks when possible.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The amount that could not be added.</returns>
        public int TryAdd(ItemInstance item)
        {
            int countLeft = item.StackCount;
            foreach (var i in FindCanStackItems(item))
            {
                if (countLeft <= 0) return 0;
                if (countLeft <= i.InstanceItem.Definition.MaxStack - i.InstanceItem.StackCount)
                {
                    i.InstanceItem.StackCount += countLeft;
                    return 0;
                }

                countLeft -= i.InstanceItem.Definition.MaxStack - i.InstanceItem.StackCount;
                i.InstanceItem.StackCount = i.InstanceItem.Definition.MaxStack;
            }

            item.StackCount = countLeft;
            if (TryFindSpace(item, out int x, out int y, out bool rotated))
            {
                if (TryPlace(item, x, y, rotated))
                {
                    return 0;
                }
            }

            return countLeft;
        }

        private List<PlacedItem> FindCanStackItems(ItemInstance item)
        {
            List<PlacedItem> result = new();
            foreach (var i in items)
            {
                if (i.InstanceItem.Definition.ItemID == item.Definition.ItemID &&
                    i.InstanceItem.StackCount < i.InstanceItem.Definition.MaxStack)
                {
                    result.Add(i);
                }
            }
            return result;
        }

        /// <summary>
        /// Attempts to remove a placed item from the grid.
        /// </summary>
        /// <param name="instanceId">The item instance identifier.</param>
        /// <param name="removedItem">The removed placed item when successful.</param>
        /// <returns><c>true</c> if the item was removed; otherwise, <c>false</c>.</returns>
        public bool TryRemove(string instanceId, out PlacedItem removedItem)
        {
            PlacedItem item = GetPlacedItem(instanceId);
            if (item == null)
            {
                removedItem = null;
                return false;
            }

            items.Remove(item);
            removedItem = item;
            return true;
        }

        /// <summary>
        /// Removes a placed item from the grid without returning it.
        /// </summary>
        /// <param name="instanceId">The item instance identifier.</param>
        public void Remove(string instanceId)
        {
            PlacedItem item = GetPlacedItem(instanceId);
            if (item == null) return;
            items.Remove(item);
        }

        /// <summary>
        /// Attempts to move an existing item to a new grid position.
        /// </summary>
        /// <param name="instanceId">The item instance identifier.</param>
        /// <param name="newX">The target grid x coordinate.</param>
        /// <param name="newY">The target grid y coordinate.</param>
        /// <param name="rotated">Whether the item is rotated.</param>
        /// <returns><c>true</c> if the move succeeded; otherwise, <c>false</c>.</returns>
        public bool TryMove(string instanceId, int newX, int newY, bool rotated)
        {
            PlacedItem item = GetPlacedItem(instanceId);
            if (item == null) return false;
            if (!CanPlace(item.InstanceItem, newX, newY, rotated, instanceId)) return false;
            item.Position = new Vector2Int(newX, newY);
            item.Rotated = rotated;
            return true;
        }

        /// <summary>
        /// Gets a placed item by instance identifier.
        /// </summary>
        /// <param name="instanceId">The item instance identifier.</param>
        /// <returns>The matching placed item, or <c>null</c> if not found.</returns>
        public PlacedItem GetPlacedItem(string instanceId)
        {
            foreach (var i in items)
            {
                if (i.InstanceItem.InstanceID == instanceId)
                {
                    return i;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the placed item covering the specified grid cell.
        /// </summary>
        /// <param name="x">The grid x coordinate.</param>
        /// <param name="y">The grid y coordinate.</param>
        /// <returns>The placed item at the specified cell, or <c>null</c> if none exists.</returns>
        public PlacedItem GetItemAt(int x, int y)
        {
            foreach (var i in items)
            {
                if (x >= i.Position.x && x < i.Position.x + i.Width && y >= i.Position.y && y < i.Position.y + i.Height)
                {
                    return i;
                }
            }

            return null;
        }

        /// <summary>
        /// Attempts to find a valid placement location for the specified item.
        /// </summary>
        /// <param name="item">The item to place.</param>
        /// <param name="x">The resolved grid x coordinate.</param>
        /// <param name="y">The resolved grid y coordinate.</param>
        /// <param name="rotated">The resolved rotation state.</param>
        /// <returns><c>true</c> if a valid location was found; otherwise, <c>false</c>.</returns>
        public bool TryFindSpace(ItemInstance item, out int x, out int y, out bool rotated)
        {
            for (int yPos = 0; yPos < GridHeight; yPos++)
            {
                for (int xPos = 0; xPos < GridWidth; xPos++)
                {
                    for (int k = 0; k < (item.Definition.CanRotate ? 2 : 1); k++)
                    {
                        if (CanPlace(item, xPos, yPos, k == 1))
                        {
                            x = xPos;
                            y = yPos;
                            rotated = k == 1;
                            return true;
                        }
                    }
                }
            }

            x = 0;
            y = 0;
            rotated = false;
            return false;
        }

        /// <summary>
        /// Finds all placed items with the specified item type identifier.
        /// </summary>
        /// <param name="itemId">The item type identifier.</param>
        /// <returns>A list of matching placed items.</returns>
        public List<PlacedItem> FindItems(string itemId)
        {
            List<PlacedItem> result = new();
            foreach (var item in items)
            {
                if (item.InstanceItem.Definition.ItemID == itemId)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the total stack count for the specified item type.
        /// </summary>
        /// <param name="itemId">The item type identifier.</param>
        /// <returns>The total stack count.</returns>
        public int GetItemCount(string itemId)
        {
            int total = 0;
            foreach (var item in items)
            {
                if (item.InstanceItem.Definition.ItemID == itemId)
                {
                    total += item.InstanceItem.StackCount;
                }
            }

            return total;
        }

        /// <summary>
        /// Gets a runtime item instance by instance identifier.
        /// </summary>
        /// <param name="instanceId">The item instance identifier.</param>
        /// <returns>The matching item instance, or <c>null</c> if not found.</returns>
        public ItemInstance GetItemInstance(string instanceId)
        {
            PlacedItem item = GetPlacedItem(instanceId);
            return item?.InstanceItem;
        }

        /// <summary>
        /// Gets all placed items currently stored in the grid.
        /// </summary>
        /// <returns>A read-only list of placed items.</returns>
        public IReadOnlyList<PlacedItem> GetAllPlacedItems() => items;

        /// <summary>
        /// Gets all runtime item instances currently stored in the grid.
        /// </summary>
        /// <returns>A read-only list of item instances.</returns>
        public IReadOnlyList<ItemInstance> GetAllItemInstances()
        {
            List<ItemInstance> result = new();
            foreach (var item in items)
            {
                result.Add(item.InstanceItem);
            }

            return result;
        }

        /// <summary>
        /// Gets aggregated item counts grouped by item definition.
        /// </summary>
        /// <returns>A dictionary containing counts per item definition.</returns>
        public Dictionary<ItemDefinition, int> GetAllItemCounts()
        {
            Dictionary<ItemDefinition, int> result = new();
            foreach (var item in items)
            {
                if (!result.ContainsKey(item.InstanceItem.Definition))
                {
                    result[item.InstanceItem.Definition] = 0;
                }
                result[item.InstanceItem.Definition] += item.InstanceItem.StackCount;
            }

            return result;
        }

        /// <summary>
        /// Gets all unlocked grid cell coordinates.
        /// </summary>
        /// <returns>A read-only list of unlocked cell coordinates.</returns>
        public IReadOnlyList<Vector2Int> GetUnlockedSlots()
        {
            List<Vector2Int> result = new();
            foreach (var slot in unlockedSlots)
            {
                result.Add(slot);
            }
            return result;
        }

        /// <summary>
        /// Checks whether two item instances can be merged.
        /// </summary>
        /// <param name="sourceId">The source item instance identifier.</param>
        /// <param name="targetId">The target item instance identifier.</param>
        /// <returns><c>true</c> if the items can be merged; otherwise, <c>false</c>.</returns>
        public bool CanMerge(string sourceId, string targetId)
        {
            var source = GetItemInstance(sourceId);
            var target = GetItemInstance(targetId);
            if(source == null || target == null) return false;
            if(source.Definition != target.Definition) return false;
            if(source.StackCount + target.StackCount > target.Definition.MaxStack) return false;
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
            if(!CanMerge(sourceId, targetId)) return false;
            var source = GetItemInstance(sourceId);
            var target = GetItemInstance(targetId);
            target.StackCount += source.StackCount;
            Remove(sourceId);
            return true;
        }

        /// <summary>
        /// Attempts to split a number of items from an existing stack.
        /// </summary>
        /// <param name="instanceId">The source item instance identifier.</param>
        /// <param name="count">The number of items to split.</param>
        /// <param name="newInstance">The newly created split stack when successful.</param>
        /// <returns><c>true</c> if the stack was split; otherwise, <c>false</c>.</returns>
        public bool TrySplitStack(string instanceId, int count, out ItemInstance newInstance)
        {
            newInstance = null;
            if (count <= 0) return false;
            var item = GetItemInstance(instanceId);
            if (item.StackCount < count) return false;
            newInstance = new ItemInstance(item.Definition, Guid.NewGuid().ToString(), count);
            int remainingCount = item.StackCount - count;
            if (remainingCount <= 0)
            {
                Remove(item.InstanceID);
                return true;
            }

            item.StackCount = remainingCount;
            return true;
        }
    }
}
