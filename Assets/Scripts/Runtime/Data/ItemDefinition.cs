using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Data
{
    /// <summary>
    /// Defines the immutable data shared by all instances of an item type.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/InventorySystem/ItemDefinition")]
    public class ItemDefinition : ScriptableObject
    {
        /// <summary>
        /// Unique identifier of the item type.
        /// </summary>
        public string ItemID;

        /// <summary>
        /// Localized or display-friendly item name.
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// Icon used to represent the item in UI.
        /// </summary>
        public Sprite Icon;

        /// <summary>
        /// Item width in inventory grid cells.
        /// </summary>
        public int Width;

        /// <summary>
        /// Item height in inventory grid cells.
        /// </summary>
        public int Height;

        /// <summary>
        /// Indicates whether the item can be rotated in the grid.
        /// </summary>
        public bool CanRotate;

        /// <summary>
        /// Maximum stack size for this item type.
        /// </summary>
        public int MaxStack;

        /// <summary>
        /// Weight of a single item unit.
        /// </summary>
        public float Weight;
    }
}
