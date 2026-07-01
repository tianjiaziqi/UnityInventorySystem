using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Inventory.QuickBar
{
    /// <summary>
    /// Defines the layout of the quick bar.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/InventorySystem/QuickBarConfig")]
    public class QuickBarConfig : ScriptableObject
    {
        /// <summary>
        /// Number of slots available in the quick bar.
        /// </summary>
        public int slotCount;
    }
}
