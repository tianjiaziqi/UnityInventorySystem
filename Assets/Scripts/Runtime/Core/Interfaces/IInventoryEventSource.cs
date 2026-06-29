using UnityEngine;
using UnityEngine.Events;

namespace JZQ.InventorySystem.Runtime.Core
{
    public interface IInventoryEventSource
    {
        void RegisterEvent(EInventoryEventType eventType, UnityAction action);
        void UnregisterEvent(EInventoryEventType eventType, UnityAction action);
        void InvokeEvent(EInventoryEventType eventType);
    }
}

