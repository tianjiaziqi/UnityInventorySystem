using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Inventory.QuickBar
{
public class QuickBarSlot
{
    private string itemInstanceId;
    public bool IsEmpty => string.IsNullOrEmpty(itemInstanceId);

    public void Bind(string instanceId)
    {
        itemInstanceId = instanceId;
    }

    public void Clear()
    {
        itemInstanceId = null;
    }

    public string GetItemInstanceId()
    {
        return itemInstanceId;
    }

}
}
