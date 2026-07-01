namespace JZQ.InventorySystem.Runtime.Inventory.QuickBar
{
    /// <summary>
    /// Stores the item binding state for a single quick bar slot.
    /// </summary>
    public class QuickBarSlot
    {
        private string itemInstanceId;

        /// <summary>
        /// Gets whether the slot is currently empty.
        /// </summary>
        public bool IsEmpty => string.IsNullOrEmpty(itemInstanceId);

        /// <summary>
        /// Binds an item instance to the slot.
        /// </summary>
        /// <param name="instanceId">The item instance identifier to bind.</param>
        public void Bind(string instanceId)
        {
            itemInstanceId = instanceId;
        }

        /// <summary>
        /// Clears the current slot binding.
        /// </summary>
        public void Clear()
        {
            itemInstanceId = null;
        }

        /// <summary>
        /// Gets the bound item instance identifier.
        /// </summary>
        /// <returns>The bound item instance identifier, or <c>null</c> if the slot is empty.</returns>
        public string GetItemInstanceId()
        {
            return itemInstanceId;
        }
    }
}
