using System;
using System.Collections.Generic;
using JZQ.InventorySystem.Runtime.Data;
using JZQ.InventorySystem.Runtime.Inventory.Backpack;
using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Inventory.Common
{
    public class InventoryGrid
    {
        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }
        private List<PlacedItem> items = new();
        private HashSet<Vector2Int> unlockedSlots = new();

        public InventoryGrid(int width, int height)
        {
            GridWidth = width;
            GridHeight = height;
        }

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

        public bool IsCellUnlocked(int x, int y)
        {
            return unlockedSlots.Contains(new Vector2Int(x, y));
        }

        public bool TryPlace(ItemInstance item, int x, int y, bool rotated)
        {
            if (!CanPlace(item, x, y, rotated)) return false;
            items.Add(new PlacedItem(item, x, y, rotated));
            return true;
        }

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

        public void Remove(string instanceId)
        {
            PlacedItem item = GetPlacedItem(instanceId);
            if (item == null) return;
            items.Remove(item);
        }

        public bool TryMove(string instanceId, int newX, int newY, bool rotated)
        {
            PlacedItem item = GetPlacedItem(instanceId);
            if (item == null) return false;
            if (!CanPlace(item.InstanceItem, newX, newY, rotated, instanceId)) return false;
            item.Position = new Vector2Int(newX, newY);
            item.Rotated = rotated;
            return true;
        }

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

        public ItemInstance GetItemInstance(string instanceId)
        {
            PlacedItem item = GetPlacedItem(instanceId);
            return item?.InstanceItem;
        }

        public IReadOnlyList<PlacedItem> GetAllPlacedItems() => items;

        public IReadOnlyList<ItemInstance> GetAllItemInstances()
        {
            List<ItemInstance> result = new();
            foreach (var item in items)
            {
                result.Add(item.InstanceItem);
            }

            return result;
        }

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

        public IReadOnlyList<Vector2Int> GetUnlockedSlots()
        {
            List<Vector2Int> result = new();
            foreach (var slot in unlockedSlots)
            {
                result.Add(slot);
            }
            return result;
        }

        public bool CanMerge(string sourceId, string targetId)
        {
            var source = GetItemInstance(sourceId);
            var target = GetItemInstance(targetId);
            if(source == null || target == null) return false;
            if(source.Definition != target.Definition) return false;
            if(source.StackCount + target.StackCount > target.Definition.MaxStack) return false;
            return true;
        }

        public bool TryMergeStack(string sourceId, string targetId)
        {
            if(!CanMerge(sourceId, targetId)) return false;
            var source = GetItemInstance(sourceId);
            var target = GetItemInstance(targetId);
            target.StackCount += source.StackCount;
            Remove(sourceId);
            return true;
        }

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
