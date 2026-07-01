using System.Collections.Generic;
using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Data
{
    /// <summary>
    /// Stores all item definitions available to the inventory system.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "ScriptableObjects/Items/ItemDatabase")]
    public class ItemDatabase : ScriptableObject
    {
        /// <summary>
        /// The item definitions contained in this database.
        /// </summary>
        public List<ItemDefinition> Items;

        /// <summary>
        /// Attempts to find an item definition by item identifier.
        /// </summary>
        /// <param name="id">The item identifier to search for.</param>
        /// <param name="item">The matching item definition when found.</param>
        /// <returns><c>true</c> if a matching item definition was found; otherwise, <c>false</c>.</returns>
        public bool TryGetItem(string id, out ItemDefinition item)
        {
            item = Items.Find(i => i.ItemID == id);
            return item != null;
        }
    }
}
