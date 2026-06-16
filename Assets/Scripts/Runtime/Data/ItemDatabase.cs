using System.Collections.Generic;
using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Data
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "ScriptableObjects/Items/ItemDatabase")]
    public class ItemDatabase : ScriptableObject
    {
        public List<ItemDefinition> Items;

        public bool TryGetItem(string id, out ItemDefinition item)
        {
            item = Items.Find(i => i.ItemID == id);
            return item != null;
        }
    }
}
