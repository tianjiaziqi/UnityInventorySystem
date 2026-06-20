using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Inventory.Backpack
{
    [CreateAssetMenu(menuName = "ScriptableObjects/InventorySystem/BackpackLayoutConfig")]
    public class BackpackLayoutConfig : ScriptableObject
    {
        public Vector2Int MaxSize;
        public Vector2Int InitialSize;
        public float MaxWeight;
        public float InitialWeight;
        public float CellSize;
        public Vector2 CellSpacing;
    }
}
