using System.Collections.Generic;
using JZQ.InventorySystem.Runtime.Inventory.Backpack;

namespace JZQ.InventorySystem.Runtime.Core
{
    /// <summary>
    /// Provides read-only access to backpack state.
    /// </summary>
    public interface IBackpackReadOnly
    {
        /// <summary>
        /// Gets all items currently placed in the player backpack.
        /// </summary>
        /// <returns>A read-only list of placed items.</returns>
        IReadOnlyList<PlacedItem> GetPlayerPlacedItems();

        /// <summary>
        /// Returns whether the specified grid cell is unlocked.
        /// </summary>
        /// <param name="x">The grid x coordinate.</param>
        /// <param name="y">The grid y coordinate.</param>
        /// <returns><c>true</c> if the cell is unlocked; otherwise, <c>false</c>.</returns>
        bool IsPlayerGridUnlocked(int x, int y);

        /// <summary>
        /// Gets the player's current carried weight.
        /// </summary>
        /// <returns>The current carried weight.</returns>
        float GetPlayerCurrentWeight();

        /// <summary>
        /// Gets the player's maximum carry weight.
        /// </summary>
        /// <returns>The maximum carry weight.</returns>
        float GetPlayerMaxWeight();

        /// <summary>
        /// Gets the current weight ratio relative to the maximum carry weight.
        /// </summary>
        /// <returns>The current weight ratio.</returns>
        float GetPlayerWeightRatio();
    }
}
