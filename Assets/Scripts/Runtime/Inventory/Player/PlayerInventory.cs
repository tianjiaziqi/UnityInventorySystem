using System.Collections.Generic;
using JZQ.InventorySystem.Runtime.Core;
using JZQ.InventorySystem.Runtime.Data;
using JZQ.InventorySystem.Runtime.Inventory.Backpack;
using JZQ.InventorySystem.Runtime.Inventory.Common;
using JZQ.InventorySystem.Runtime.Inventory.QuickBar;
using UnityEngine;
using InventoryRuntimeSystem = JZQ.InventorySystem.Runtime.Core.InventorySystem;
using QuickBarModel = JZQ.InventorySystem.Runtime.Inventory.QuickBar.QuickBar;

namespace JZQ.InventorySystem.Runtime.Inventory.Player
{
    public class PlayerInventory
    {
        private InventoryGrid mainGrid;
        private float maxWeight;
        private QuickBarModel quickBar;

        private BackpackLayoutConfig backpackConfig;
        private QuickBarConfig quickBarConfig;

        private float currentWeight;

        public PlayerInventory(BackpackLayoutConfig backpackConfig, QuickBarConfig quickBarConfig, IInventoryEventSource events)
        {
            this.backpackConfig = backpackConfig;
            this.quickBarConfig = quickBarConfig;
            mainGrid = new InventoryGrid(backpackConfig.MaxSize.x, backpackConfig.MaxSize.y);
            maxWeight = backpackConfig.InitialWeight;
            quickBar = new QuickBarModel(quickBarConfig.slotCount, events);
        }

        public int TryAddItem(ItemInstance item)
        {
            var initialCount = item.StackCount;
            var left = mainGrid.TryAdd(item);
            if (initialCount != left)
            {
                RecalculateCurrentWeight();
                InventoryRuntimeSystem.Events.InvokeEvent(EInventoryEventType.InventoryChange);
            }

            return left;
        }

        public bool TryRemoveItem(string instanceId, out PlacedItem removedItem)
        {
            if (mainGrid.TryRemove(instanceId, out removedItem))
            {
                RecalculateCurrentWeight();
                UpdateQuickBar();
                InventoryRuntimeSystem.Events.InvokeEvent(EInventoryEventType.InventoryChange);
                return true;
            }

            return false;
        }

        public bool TryMoveItem(string instanceId, int x, int y, bool rotated)
        {
            if (mainGrid.TryMove(instanceId, x, y, rotated))
            {
                InventoryRuntimeSystem.Events.InvokeEvent(EInventoryEventType.InventoryChange);
                return true;
            }

            return false;
        }

        public bool UnlockGrid(int x, int y)
        {
            if (mainGrid.IsCellUnlocked(x, y))
            {
                return false;
            }
            mainGrid.UnlockCell(x, y);
            InventoryRuntimeSystem.Events.InvokeEvent(EInventoryEventType.InventoryUnlockChange);
            return true;
        }

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

        public float GetWeightRatio()
        {
            if (maxWeight <= 0)
            {
                Debug.LogError("请检查背包数据配置, 当前最大负重为0");
                return 0;
            }
            return currentWeight / maxWeight;
        }

        public float GetMaxWeight() => maxWeight;
        public float GetCurrentWeight() => currentWeight;
        public IReadOnlyList<PlacedItem> GetAllPlacedItems() => mainGrid.GetAllPlacedItems();
        public IReadOnlyList<ItemInstance> GetAllItemInstances() => mainGrid.GetAllItemInstances();
        public Dictionary<ItemDefinition, int> GetAllItemCounts() => mainGrid.GetAllItemCounts();
        public bool HasItem(string itemId) => mainGrid.GetItemCount(itemId) > 0;
        public int GetItemCount(string itemId) => mainGrid.GetItemCount(itemId);
        public List<PlacedItem> GetItemsById(string itemId) => mainGrid.FindItems(itemId);
        public bool CanPlaceItem(ItemInstance item, int x, int y, bool rotated) => mainGrid.CanPlace(item, x, y, rotated);

        public bool CanMoveItemTo(string instanceId, int x, int y, bool rotated)
        {
            PlacedItem item = mainGrid.GetPlacedItem(instanceId);
            if (item == null) return false;

            return mainGrid.CanPlace(item.InstanceItem, x, y, rotated, instanceId);
        }

        public IReadOnlyList<Vector2Int> GetUnlockedSlots() => mainGrid.GetUnlockedSlots();
        public bool IsGridUnlocked(int x, int y) => mainGrid.IsCellUnlocked(x, y);

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

        public void BindItemToQuickBar(int index, string instanceId)
        {
            quickBar.BindItem(index, instanceId);
        }

        public void ClearQuickBarSlot(int index)
        {
            quickBar.ClearSlot(index);
        }

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

        public ItemInstance GetSelectedItem()
        {
            string id = quickBar.GetSelectedItemId();
            if (id == "") return null;
            return mainGrid.GetItemInstance(id);
        }

        public void SwapQuickBarSlots(int index1, int index2)
        {
            quickBar.SwapSlots(index1, index2);
        }

        public void SetQuickBarSelection(int index)
        {
            quickBar.SetSelectedIndex(index);
        }

        public void SelectNextQuickSlot()
        {
            quickBar.SelectNext();
        }

        public void SelectPreviousQuickSlot()
        {
            quickBar.SelectPrevious();
        }

        public int GetSelectedIndex()
        {
            return quickBar.GetSelectedIndex();
        }

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

        public bool CanConsumeByInstanceId(string instanceId, int count)
        {
            ItemInstance instance = mainGrid.GetItemInstance(instanceId);
            if (instance == null) return false;
            if (instance.StackCount < count) return false;
            return true;
        }

        public void ConsumeByInstanceId(string instanceId, int count)
        {
            ItemInstance instance = mainGrid.GetItemInstance(instanceId);
            if (instance == null) return;
            if (instance.StackCount - count <= 0)
            {
                mainGrid.Remove(instanceId);
            }
            else
            {
                instance.StackCount -= count;
            }

            RecalculateCurrentWeight();
            InventoryRuntimeSystem.Events.InvokeEvent(EInventoryEventType.InventoryChange);
        }
    }
}
