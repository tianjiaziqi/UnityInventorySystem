using UnityEngine;
using UnityEngine.Events;

namespace JZQ.InventorySystem.Runtime.Core
{
    /// <summary>
    /// Exposes inventory event registration and dispatch.
    /// </summary>
    public interface IInventoryEventSource
    {
        /// <summary>
        /// Registers a listener for the specified inventory event type.
        /// </summary>
        /// <param name="eventType">The inventory event type.</param>
        /// <param name="action">The listener to register.</param>
        void RegisterEvent(EInventoryEventType eventType, UnityAction action);

        /// <summary>
        /// Unregisters a listener from the specified inventory event type.
        /// </summary>
        /// <param name="eventType">The inventory event type.</param>
        /// <param name="action">The listener to unregister.</param>
        void UnregisterEvent(EInventoryEventType eventType, UnityAction action);

        /// <summary>
        /// Dispatches the specified inventory event.
        /// </summary>
        /// <param name="eventType">The inventory event type to dispatch.</param>
        void InvokeEvent(EInventoryEventType eventType);
    }
}
