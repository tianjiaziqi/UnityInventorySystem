using JZQ.InventorySystem.Runtime.Inventory.Common;

namespace JZQ.InventorySystem.Runtime.Core
{
    public static class InventorySystem
    {
        public static InventoryManager Current { get; internal set; }

        public static void SetCurrent(InventoryManager manager) => Current = manager;

        public static void ClearCurrent(InventoryManager manager)
        {
            if (Current == manager)
            {
                Current = null;
            }
        }
    }
}
