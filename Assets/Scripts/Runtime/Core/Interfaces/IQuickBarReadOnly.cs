using JZQ.InventorySystem.Runtime.Data;
using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Core
{
    /// <summary>
    /// Provides read-only access to quick bar state.
    /// </summary>
    public interface IQuickBarReadOnly
    {
        /// <summary>
        /// Gets the index of the currently selected quick slot.
        /// </summary>
        /// <returns>The selected quick slot index.</returns>
        int GetQuickBarSelectedIndex();

        /// <summary>
        /// Gets the current quick bar item snapshot.
        /// </summary>
        /// <returns>An array representing the current quick bar items.</returns>
        ItemInstance[] GetQuickBarItems();

        /// <summary>
        /// Gets the item currently selected in the quick bar.
        /// </summary>
        /// <returns>The selected item instance, or <c>null</c> if the selected slot is empty.</returns>
        ItemInstance GetSelectedQuickBarItem();
    }
}
