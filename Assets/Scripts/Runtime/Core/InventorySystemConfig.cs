using JZQ.InventorySystem.Runtime.Data;
using JZQ.InventorySystem.Runtime.Inventory.Backpack;
using JZQ.InventorySystem.Runtime.Inventory.QuickBar;
using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Core
{
    /// <summary>
    /// Aggregates the configuration assets required to bootstrap the inventory system.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/InventorySystem/InventorySystemConfig")]
    public class InventorySystemConfig : ScriptableObject
    {
        /// <summary>
        /// Backpack layout and capacity configuration.
        /// </summary>
        public BackpackLayoutConfig BackpackLayoutConfig;

        /// <summary>
        /// Quick bar layout configuration.
        /// </summary>
        public QuickBarConfig QuickBarConfig;

        /// <summary>
        /// Shared inventory UI configuration.
        /// </summary>
        public InventoryViewConfig InventoryViewConfig;

        /// <summary>
        /// Item database used by the sample setup.
        /// </summary>
        public ItemDatabase ItemDatabase;
    }
}
