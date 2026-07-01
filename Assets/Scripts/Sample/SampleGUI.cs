using System;
using JZQ.InventorySystem.Runtime.Core;
using JZQ.InventorySystem.Runtime.Data;
using UnityEngine;

/// <summary>
/// Provides a minimal IMGUI-based helper for spawning sample items at runtime.
/// </summary>
public class SampleGUI : MonoBehaviour
{
    private string itemId = "";
    private string count = "1";
    [SerializeField] private ItemDatabase itemDataBase;

    private void OnGUI()
    {
        itemId = GUI.TextField(new Rect(10, 10, 100, 20), itemId);
        count = GUI.TextField(new Rect(10, 40, 100, 20), count);
        if (GUI.Button(new Rect(120, 10, 100, 20), "Add"))
        {
            if (int.TryParse(count, out int countNum))
            {
                AddItem(itemId, countNum);
            }
            else
            {
                Debug.LogError("Invalid count value");
            }
        }
    }
    
    /// <summary>
    /// Adds an item to the active sample inventory runtime.
    /// </summary>
    /// <param name="itemID">The item type identifier.</param>
    /// <param name="count">The amount to add.</param>
    private void AddItem(string itemID, int count = 1)
    {
        if (!itemDataBase.TryGetItem(itemID, out ItemDefinition def)) return;
        ItemInstance itemIns = new ItemInstance(def, Guid.NewGuid().ToString(), count);
        int left = InventorySystem.Runtime.TryAddItemToPlayer(itemIns);

        if (left > 0)
        {
            Debug.LogError($"{left} items {itemID} cannot be added to the player inventory");
        }
    }
}
