using System.Collections.Generic;
using JZQ.InventorySystem.Runtime.Data;
using JZQ.InventorySystem.Runtime.Inventory.Backpack;

namespace JZQ.InventorySystem.Runtime.Core
{
    /// <summary>
    /// Provides runtime operations required by backpack UI and interaction flows.
    /// </summary>
    public interface IBackpackViewRuntime
    {
        /// <summary>
        /// Returns whether the specified grid cell is unlocked.
        /// </summary>
        /// <param name="x">The grid x coordinate.</param>
        /// <param name="y">The grid y coordinate.</param>
        /// <returns><c>true</c> if the cell is unlocked; otherwise, <c>false</c>.</returns>
        bool IsPlayerGridUnlocked(int x, int y);

        /// <summary>
        /// Gets all items currently placed in the player backpack.
        /// </summary>
        /// <returns>A read-only list of placed items.</returns>
        IReadOnlyList<PlacedItem> GetPlayerPlacedItems();

        /// <summary>
        /// Checks whether an item can be placed at the specified grid position.
        /// </summary>
        /// <param name="item">The item to validate.</param>
        /// <param name="x">The target grid x coordinate.</param>
        /// <param name="y">The target grid y coordinate.</param>
        /// <param name="rotated">Whether the item is rotated.</param>
        /// <returns><c>true</c> if the item can be placed; otherwise, <c>false</c>.</returns>
        bool CanPlaceItemInPlayer(ItemInstance item, int x, int y, bool rotated);

        /// <summary>
        /// Checks whether an existing player item can be moved to the specified grid position.
        /// </summary>
        /// <param name="instanceId">The item instance identifier.</param>
        /// <param name="x">The target grid x coordinate.</param>
        /// <param name="y">The target grid y coordinate.</param>
        /// <param name="rotated">Whether the item is rotated.</param>
        /// <returns><c>true</c> if the move is valid; otherwise, <c>false</c>.</returns>
        bool CanMovePlayerItemTo(string instanceId, int x, int y, bool rotated);

        /// <summary>
        /// Attempts to move an existing player item to the specified grid position.
        /// </summary>
        /// <param name="instanceId">The item instance identifier.</param>
        /// <param name="x">The target grid x coordinate.</param>
        /// <param name="y">The target grid y coordinate.</param>
        /// <param name="rotated">Whether the item is rotated.</param>
        /// <returns><c>true</c> if the item was moved; otherwise, <c>false</c>.</returns>
        bool TryMoveItemFromPlayer(string instanceId, int x, int y, bool rotated);

        /// <summary>
        /// Binds an item instance to the specified quick slot.
        /// </summary>
        /// <param name="slotIndex">The target quick slot index.</param>
        /// <param name="instanceId">The item instance identifier.</param>
        void BindPlayerQuickSlot(int slotIndex, string instanceId);
    }
}
