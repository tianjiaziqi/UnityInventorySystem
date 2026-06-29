using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JZQ.InventorySystem.Runtime.Core;
using InventoryRuntimeSystem = JZQ.InventorySystem.Runtime.Core.InventorySystem;

namespace JZQ.InventorySystem.Runtime.Inventory.QuickBar
{
public class QuickBar
{
    private QuickBarSlot[] slots;
    private int selectedIndex;
    private IInventoryEventSource events;

    public QuickBar(int slotCount, IInventoryEventSource events)
    {
        this.events = events;
        slots = new QuickBarSlot[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            slots[i] = new QuickBarSlot();
        }
    }

    public void BindItem(int slotIndex, string instanceId)
    {
        slots[slotIndex].Bind(instanceId);
        events.InvokeEvent(EInventoryEventType.QuickBarChanged);
    }

    public void ClearSlot(int slotIndex)
    {
        if (slots[slotIndex].IsEmpty) return;
        slots[slotIndex].Clear();
        events.InvokeEvent(EInventoryEventType.QuickBarChanged);
    }

    public void SwapSlots(int a, int b)
    {
        if(slots[a].IsEmpty && slots[b].IsEmpty) return;
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

    public void SetSelectedIndex(int index)
    {
        if (index < 0 || index >= slots.Length) return;
        selectedIndex = index;
        events.InvokeEvent(EInventoryEventType.QuickBarSelectionChanged);
    }

    public void SelectNext()
    {
        selectedIndex++;
        if(selectedIndex >= slots.Length) selectedIndex = 0;
        events.InvokeEvent(EInventoryEventType.QuickBarSelectionChanged);
    }

    public void SelectPrevious()
    {
        selectedIndex--;
        if (selectedIndex < 0) selectedIndex = slots.Length - 1;
        events.InvokeEvent(EInventoryEventType.QuickBarSelectionChanged);
    }
    public string GetSelectedItemId()
    {
        if(slots[selectedIndex].IsEmpty) return "";
        return slots[selectedIndex].GetItemInstanceId();
    }

    public string GetItemIdAt(int index)
    {
        if(slots[index].IsEmpty) return "";
        return slots[index].GetItemInstanceId();
    }

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
    
    public int GetSelectedIndex()
    {
        return selectedIndex;
    }
    
    
}
}
