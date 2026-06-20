using System;
using JZQ.InventorySystem.Runtime.Core;
using JZQ.InventorySystem.Runtime.Data;
using UnityEngine;

public class SampleGUI : MonoBehaviour
{
    private string itemId = "";
    private string count = "1";
    private bool isInitialized;
    private ItemDatabase dataBase;

    public void Initialize(ItemDatabase dataBase)
    {
        if (isInitialized) return;
        this.dataBase = dataBase;
        isInitialized = true;
    }
    private void OnGUI()
    {
        if(!isInitialized)
            return;
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
    
    private void AddItem(string itemID, int count = 1)
    {
        if (!dataBase.TryGetItem(itemID, out ItemDefinition def)) return;
        ItemInstance itemIns = new ItemInstance(def, Guid.NewGuid().ToString(), count);
        int left = InventorySystem.Current.TryAddItemToPlayer(itemIns);
        

        if (left > 0)
        {
            Debug.LogError($"{left} items {itemID} cannot be added to the player inventory");
        }
        
    }
}
