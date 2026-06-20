using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Data
{
    [CreateAssetMenu(menuName = "ScriptableObjects/InventorySystem/ItemDefinition")]
    
    public class ItemDefinition : ScriptableObject
    {
        public string ItemID;
        public string DisplayName;
        public Sprite Icon;
        public int Width;
        public int Height;
        public bool CanRotate;
        public int MaxStack;
        public float Weight;
    }
}
