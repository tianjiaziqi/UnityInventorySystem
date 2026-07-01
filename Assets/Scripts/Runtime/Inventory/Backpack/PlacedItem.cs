using JZQ.InventorySystem.Runtime.Data;
using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Inventory.Backpack
{
    /// <summary>
    /// Represents an item instance placed at a specific position in the backpack grid.
    /// </summary>
    [System.Serializable]
    public class PlacedItem
    {
        /// <summary>
        /// The runtime item instance represented by this placement.
        /// </summary>
        public ItemInstance InstanceItem;

        /// <summary>
        /// The top-left grid position of the placed item.
        /// </summary>
        public Vector2Int Position;

        /// <summary>
        /// Indicates whether the placed item is rotated.
        /// </summary>
        public bool Rotated;

        /// <summary>
        /// Gets the current width of the placed item in grid cells.
        /// </summary>
        public int Width => Rotated ? InstanceItem.Definition.Height : InstanceItem.Definition.Width;

        /// <summary>
        /// Gets the current height of the placed item in grid cells.
        /// </summary>
        public int Height => Rotated ? InstanceItem.Definition.Width : InstanceItem.Definition.Height;

        /// <summary>
        /// Creates a placed item representation for a backpack grid.
        /// </summary>
        /// <param name="instanceItem">The runtime item instance.</param>
        /// <param name="x">The grid x coordinate.</param>
        /// <param name="y">The grid y coordinate.</param>
        /// <param name="rotated">Whether the item is rotated.</param>
        public PlacedItem(ItemInstance instanceItem, int x, int y, bool rotated)
        {
            InstanceItem = instanceItem;
            Position = new Vector2Int(x, y);
            Rotated = rotated;
        }
    }
}
