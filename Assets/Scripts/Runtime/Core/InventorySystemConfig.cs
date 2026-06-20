using JZQ.InventorySystem.Runtime.Data;
using JZQ.InventorySystem.Runtime.Inventory.Backpack;
using JZQ.InventorySystem.Runtime.Inventory.QuickBar;
using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/InventorySystem/InventorySystemConfig")]
    public class InventorySystemConfig : ScriptableObject
    {
        public BackpackLayoutConfig BackpackLayoutConfig;
        public QuickBarConfig QuickBarConfig;
        public InventoryViewConfig InventoryViewConfig;
        public ItemDatabase ItemDatabase;
    }
}
