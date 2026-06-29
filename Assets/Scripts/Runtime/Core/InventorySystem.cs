using JZQ.InventorySystem.Runtime.Inventory.Common;

namespace JZQ.InventorySystem.Runtime.Core
{
    public static class InventorySystem
    {
        public static IInventoryRuntime Runtime { get; internal set; }
        
        public static IInventoryEventSource Events { get; internal set; }
        
        public static IBackpackReadOnly BackpackReadOnly { get; internal set; }
        public static IQuickBarReadOnly QuickBarReadOnly { get; internal set; }
        
        public static IBackpackViewRuntime BackpackViewRuntime { get; internal set; }

        public static void SetCurrent(IInventoryRuntime runtime) => Runtime = runtime;
        public static void SetEvents(IInventoryEventSource events) => Events = events;
        
        public static void SetBackpackReadOnly(IBackpackReadOnly backpackReadOnly) => BackpackReadOnly = backpackReadOnly;
        public static void SetQuickBarReadOnly(IQuickBarReadOnly quickBarReadOnly) => QuickBarReadOnly = quickBarReadOnly;
        
        public static void SetBackpackViewRuntime(IBackpackViewRuntime backpackViewRuntime) => BackpackViewRuntime = backpackViewRuntime;

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
        }
    }
}
