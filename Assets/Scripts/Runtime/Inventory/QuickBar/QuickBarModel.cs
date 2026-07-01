using JZQ.InventorySystem.Runtime.Core;

namespace JZQ.InventorySystem.Runtime.Inventory.QuickBar
{
    /// <summary>
    /// Stores quick bar bindings and selection state.
    /// </summary>
    public class QuickBarModel
    {
        private readonly QuickBarSlot[] slots;
        private readonly IInventoryEventSource events;
        private int selectedIndex;

        /// <summary>
        /// Creates a quick bar model with the specified slot count.
        /// </summary>
        /// <param name="slotCount">The number of quick bar slots.</param>
        /// <param name="events">The event source used to dispatch quick bar updates.</param>
        public QuickBarModel(int slotCount, IInventoryEventSource events)
        {
            this.events = events;
            slots = new QuickBarSlot[slotCount];
            for (int i = 0; i < slotCount; i++)
            {
                slots[i] = new QuickBarSlot();
            }
        }

        /// <summary>
        /// Binds an item instance to the specified quick slot.
        /// </summary>
        /// <param name="slotIndex">The target quick slot index.</param>
        /// <param name="instanceId">The item instance identifier.</param>
        public void BindItem(int slotIndex, string instanceId)
        {
            if (slotIndex < 0 || slotIndex >= slots.Length) return;
            slots[slotIndex].Bind(instanceId);
            events.InvokeEvent(EInventoryEventType.QuickBarChanged);
        }

        /// <summary>
        /// Clears the specified quick slot.
        /// </summary>
        /// <param name="slotIndex">The quick slot index to clear.</param>
        public void ClearSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slots.Length) return;
            if (slots[slotIndex].IsEmpty) return;
            slots[slotIndex].Clear();
            events.InvokeEvent(EInventoryEventType.QuickBarChanged);
        }

        /// <summary>
        /// Swaps the bindings of two quick slots.
        /// </summary>
        /// <param name="a">The first quick slot index.</param>
        /// <param name="b">The second quick slot index.</param>
        public void SwapSlots(int a, int b)
        {
            if (a < 0 || a >= slots.Length || b < 0 || b >= slots.Length) return;
            if (slots[a].IsEmpty && slots[b].IsEmpty) return;
            if (slots[a].IsEmpty)
            {
                slots[a].Bind(slots[b].GetItemInstanceId());
                slots[b].Clear();
                events.InvokeEvent(EInventoryEventType.QuickBarChanged);
                return;
            }

            if (slots[b].IsEmpty)
            {
                slots[b].Bind(slots[a].GetItemInstanceId());
                slots[a].Clear();
                events.InvokeEvent(EInventoryEventType.QuickBarChanged);
                return;
            }

            string temp = slots[a].GetItemInstanceId();
            slots[a].Bind(slots[b].GetItemInstanceId());
            slots[b].Bind(temp);
            events.InvokeEvent(EInventoryEventType.QuickBarChanged);
        }

        /// <summary>
        /// Sets the selected quick slot.
        /// </summary>
        /// <param name="index">The quick slot index to select.</param>
        public void SetSelectedIndex(int index)
        {
            if (index < 0 || index >= slots.Length) return;
            selectedIndex = index;
            events.InvokeEvent(EInventoryEventType.QuickBarSelectionChanged);
        }

        /// <summary>
        /// Selects the next quick slot.
        /// </summary>
        public void SelectNext()
        {
            selectedIndex++;
            if (selectedIndex >= slots.Length) selectedIndex = 0;
            events.InvokeEvent(EInventoryEventType.QuickBarSelectionChanged);
        }

        /// <summary>
        /// Selects the previous quick slot.
        /// </summary>
        public void SelectPrevious()
        {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = slots.Length - 1;
            events.InvokeEvent(EInventoryEventType.QuickBarSelectionChanged);
        }

        /// <summary>
        /// Gets the item instance identifier of the selected quick slot.
        /// </summary>
        /// <returns>The selected item instance identifier, or an empty string if the slot is empty.</returns>
        public string GetSelectedItemId()
        {
            if (slots[selectedIndex].IsEmpty) return "";
            return slots[selectedIndex].GetItemInstanceId();
        }

        /// <summary>
        /// Gets the item instance identifier bound to the specified quick slot.
        /// </summary>
        /// <param name="index">The quick slot index.</param>
        /// <returns>The bound item instance identifier, or an empty string if the slot is empty.</returns>
        public string GetItemIdAt(int index)
        {
            if (slots[index].IsEmpty) return "";
            return slots[index].GetItemInstanceId();
        }

        /// <summary>
        /// Gets all quick slot bindings as item instance identifiers.
        /// </summary>
        /// <returns>An array containing all slot bindings.</returns>
        public string[] GetAllItemIds()
        {
            string[] result = new string[slots.Length];
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].IsEmpty) continue;
                result[i] = slots[i].GetItemInstanceId();
            }

            return result;
        }

        /// <summary>
        /// Gets the currently selected quick slot index.
        /// </summary>
        /// <returns>The selected quick slot index.</returns>
        public int GetSelectedIndex()
        {
            return selectedIndex;
        }
    }
}
