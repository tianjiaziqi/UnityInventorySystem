using System.Collections.Generic;
using UnityEngine.Events;

namespace JZQ.InventorySystem.Runtime.Core
{
    public enum EInventoryEventType
    {
        InventoryChange,
        InventoryUnlockChange,
        QuickBarChanged,
        QuickBarSelectionChanged,
    }

    public class InventoryEventCentre
    {
        private Dictionary<EInventoryEventType, UnityAction> events = new();

        public void InvokeEvent(EInventoryEventType eventType)
        {
            if (!events.TryGetValue(eventType, out UnityAction action)) return;
            action?.Invoke();
        }

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

        public void UnregisterEvent(EInventoryEventType eventType, UnityAction action)
        {
            if (!events.TryGetValue(eventType, out UnityAction existingAction)) return;
            existingAction -= action;
            events[eventType] = existingAction;
        }

        public void ClearEvents()
        {
            events.Clear();
        }

        public void ClearEvent(EInventoryEventType eventType)
        {
            events.Remove(eventType);
        }
    }
}
