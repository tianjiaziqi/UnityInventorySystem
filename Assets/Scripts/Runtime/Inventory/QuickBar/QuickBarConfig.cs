using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Inventory.QuickBar
{
    [CreateAssetMenu(fileName = "QuickBarData", menuName = "ScriptableObjects/Backpack/QuickBarData")]
    public class QuickBarConfig : ScriptableObject
    {
        public int slotCount;
    }
}
