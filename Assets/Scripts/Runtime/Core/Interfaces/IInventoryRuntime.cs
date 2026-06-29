using JZQ.InventorySystem.Runtime.Data;
using JZQ.InventorySystem.Runtime.Inventory.Backpack;
using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Core
{
    public interface IInventoryRuntime
    {
        int TryAddItemToPlayer(ItemInstance item);
        bool TryRemoveItemFromPlayer(string instanceId, out PlacedItem removedItem);
        void SetSelectedQuickSlot(int index);
        void SelectNextQuickSlot();
        void SelectPreviousQuickSlot();
        ItemInstance GetSelectedQuickBarItem();
    }
}

