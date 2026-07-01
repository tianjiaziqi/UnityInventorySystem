using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Inventory.Backpack
{
    /// <summary>
    /// Defines backpack layout, unlock state, and weight capacity settings.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/InventorySystem/BackpackLayoutConfig")]
    public class BackpackLayoutConfig : ScriptableObject
    {
        /// <summary>
        /// Maximum grid size available to the backpack.
        /// </summary>
        public Vector2Int MaxSize;

        /// <summary>
        /// Initially unlocked grid size.
        /// </summary>
        public Vector2Int InitialSize;

        /// <summary>
        /// Maximum carry weight exposed by this layout.
        /// </summary>
        public float MaxWeight;

        /// <summary>
        /// Initial carry weight capacity assigned at runtime.
        /// </summary>
        public float InitialWeight;

        /// <summary>
        /// Size of one backpack cell in UI units.
        /// </summary>
        public float CellSize;

        /// <summary>
        /// Spacing between backpack cells in UI units.
        /// </summary>
        public Vector2 CellSpacing;
    }
}
