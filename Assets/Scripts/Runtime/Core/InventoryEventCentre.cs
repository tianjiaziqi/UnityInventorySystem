using System.Collections.Generic;
using UnityEngine.Events;

namespace JZQ.InventorySystem.Runtime.Core
{
    /// <summary>
    /// Defines all inventory event types exposed by the runtime.
    /// </summary>
    public enum EInventoryEventType
    {
        /// <summary>
        /// Raised when backpack item content changes.
        /// </summary>
        InventoryChange,

        /// <summary>
        /// Raised when backpack unlock state changes.
        /// </summary>
        InventoryUnlockChange,

        /// <summary>
        /// Raised when quick bar bindings change.
        /// </summary>
        QuickBarChanged,

        /// <summary>
        /// Raised when the selected quick bar slot changes.
        /// </summary>
        QuickBarSelectionChanged,
    }

    /// <summary>
    /// Manages inventory event subscriptions and dispatch.
    /// </summary>
    public class InventoryEventCentre
    {
        private readonly Dictionary<EInventoryEventType, UnityAction> events = new();

        /// <summary>
        /// Invokes the specified event.
        /// </summary>
        /// <param name="eventType">The event type to dispatch.</param>
        public void InvokeEvent(EInventoryEventType eventType)
        {
            if (!events.TryGetValue(eventType, out UnityAction action)) return;
            action?.Invoke();
        }

        /// <summary>
        /// Registers a listener for the specified event type.
        /// </summary>
        /// <param name="eventType">The event type to subscribe to.</param>
        /// <param name="action">The listener to register.</param>
        public void RegisterEvent(EInventoryEventType eventType, UnityAction action)
        {
            if (events.TryGetValue(eventType, out UnityAction existingAction))
            {
                existingAction += action;
                events[eventType] = existingAction;
            }
            else
            {
                events.Add(eventType, action);
            }
        }

        /// <summary>
        /// Unregisters a listener from the specified event type.
        /// </summary>
        /// <param name="eventType">The event type to unsubscribe from.</param>
        /// <param name="action">The listener to unregister.</param>
        public void UnregisterEvent(EInventoryEventType eventType, UnityAction action)
        {
            if (!events.TryGetValue(eventType, out UnityAction existingAction)) return;
            existingAction -= action;
            events[eventType] = existingAction;
        }

        /// <summary>
        /// Clears all registered inventory events.
        /// </summary>
        public void ClearEvents()
        {
            events.Clear();
        }

        /// <summary>
        /// Clears all listeners for the specified event type.
        /// </summary>
        /// <param name="eventType">The event type to clear.</param>
        public void ClearEvent(EInventoryEventType eventType)
        {
            events.Remove(eventType);
        }
    }
}
