using JZQ.InventorySystem.Runtime.Inventory.Internal;

namespace JZQ.InventorySystem.Runtime.Core
{
    /// <summary>
    /// Stores the active inventory runtime service references.
    /// </summary>
    public static class InventorySystem
    {
        /// <summary>
        /// Gets the active inventory runtime service.
        /// </summary>
        public static IInventoryRuntime Runtime { get; internal set; }

        /// <summary>
        /// Gets the active inventory event source.
        /// </summary>
        public static IInventoryEventSource Events { get; internal set; }

        /// <summary>
        /// Gets the active read-only backpack service.
        /// </summary>
        public static IBackpackReadOnly BackpackReadOnly { get; internal set; }

        /// <summary>
        /// Gets the active read-only quick bar service.
        /// </summary>
        public static IQuickBarReadOnly QuickBarReadOnly { get; internal set; }

        /// <summary>
        /// Gets the active backpack interaction runtime service.
        /// </summary>
        public static IBackpackViewRuntime BackpackViewRuntime { get; internal set; }

        /// <summary>
        /// Gets the active backpack command runtime service.
        /// </summary>
        public static IBackpackCommandRuntime BackpackCommandRuntime { get; internal set; }

        /// <summary>
        /// Sets the active inventory runtime service.
        /// </summary>
        /// <param name="runtime">The runtime service instance.</param>
        public static void SetCurrent(IInventoryRuntime runtime) => Runtime = runtime;

        /// <summary>
        /// Sets the active inventory event source.
        /// </summary>
        /// <param name="events">The event source instance.</param>
        public static void SetEvents(IInventoryEventSource events) => Events = events;

        /// <summary>
        /// Sets the active read-only backpack service.
        /// </summary>
        /// <param name="backpackReadOnly">The read-only backpack service instance.</param>
        public static void SetBackpackReadOnly(IBackpackReadOnly backpackReadOnly) => BackpackReadOnly = backpackReadOnly;

        /// <summary>
        /// Sets the active read-only quick bar service.
        /// </summary>
        /// <param name="quickBarReadOnly">The read-only quick bar service instance.</param>
        public static void SetQuickBarReadOnly(IQuickBarReadOnly quickBarReadOnly) => QuickBarReadOnly = quickBarReadOnly;

        /// <summary>
        /// Sets the active backpack interaction runtime service.
        /// </summary>
        /// <param name="backpackViewRuntime">The backpack interaction runtime instance.</param>
        public static void SetBackpackViewRuntime(IBackpackViewRuntime backpackViewRuntime) => BackpackViewRuntime = backpackViewRuntime;

        /// <summary>
        /// Sets the active backpack command runtime service.
        /// </summary>
        /// <param name="backpackCommandRuntime">The backpack command runtime instance.</param>
        public static void SetBackpackCommandRuntime(IBackpackCommandRuntime backpackCommandRuntime) => BackpackCommandRuntime = backpackCommandRuntime;

        /// <summary>
        /// Clears all static service references that point to the specified manager.
        /// </summary>
        /// <param name="manager">The manager instance being released.</param>
        public static void ClearInterfaces(InventoryManager manager)
        {
            if (Runtime == manager)
            {
                Runtime = null;
            }

            if (Events == manager)
            {
                Events = null;
            }
            if (BackpackReadOnly == manager)
            {
                BackpackReadOnly = null;
            }
            if (QuickBarReadOnly == manager)
            {
                QuickBarReadOnly = null;
            }

            if (BackpackViewRuntime == manager)
            {
                BackpackViewRuntime = null;
            }
            if (BackpackCommandRuntime == manager)
            {
                BackpackCommandRuntime = null;
            }
        }
    }
}
