using JZQ.InventorySystem.Runtime.Data;
using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Inventory.Backpack
{
    [System.Serializable]
    public class PlacedItem
    {
        public ItemInstance InstanceItem;
        public Vector2Int Position;
        public bool Rotated;
        public int Width => Rotated ? InstanceItem.Definition.Height : InstanceItem.Definition.Width;
        public int Height => Rotated ? InstanceItem.Definition.Width : InstanceItem.Definition.Height;

        public PlacedItem(ItemInstance instanceItem, int x, int y, bool rotated)
        {
            InstanceItem = instanceItem;
            Position = new Vector2Int(x, y);
            Rotated = rotated;
        }
    }
}
