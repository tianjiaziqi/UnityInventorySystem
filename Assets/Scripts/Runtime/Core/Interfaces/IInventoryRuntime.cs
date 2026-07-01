using JZQ.InventorySystem.Runtime.Data;
using JZQ.InventorySystem.Runtime.Inventory.Backpack;
using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Core
{
    /// <summary>
    /// Defines the primary runtime entry points for player inventory operations.
    /// </summary>
    public interface IInventoryRuntime
    {
        /// <summary>
        /// Attempts to add an item instance to the player inventory.
        /// </summary>
        /// <param name="item">The item instance to add.</param>
        /// <returns>The amount that could not be added.</returns>
        int TryAddItemToPlayer(ItemInstance item);

        /// <summary>
        /// Attempts to remove an item instance from the player inventory.
        /// </summary>
        /// <param name="instanceId">The instance identifier to remove.</param>
        /// <param name="removedItem">The removed placed item when the operation succeeds.</param>
        /// <returns><c>true</c> if the item was removed; otherwise, <c>false</c>.</returns>
        bool TryRemoveItemFromPlayer(string instanceId, out PlacedItem removedItem);

        /// <summary>
        /// Sets the currently selected quick slot.
        /// </summary>
        /// <param name="index">The quick slot index to select.</param>
        void SetSelectedQuickSlot(int index);

        /// <summary>
        /// Selects the next quick slot.
        /// </summary>
        void SelectNextQuickSlot();

        /// <summary>
        /// Selects the previous quick slot.
        /// </summary>
        void SelectPreviousQuickSlot();

        /// <summary>
        /// Gets the item currently selected in the quick bar.
        /// </summary>
        /// <returns>The selected item instance, or <c>null</c> if the selected slot is empty.</returns>
        ItemInstance GetSelectedQuickBarItem();
    }
}
