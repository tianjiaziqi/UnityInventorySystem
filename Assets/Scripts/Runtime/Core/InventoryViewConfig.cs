using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/InventorySystem/InventoryViewConfig")]
    
    public class InventoryViewConfig : ScriptableObject
    {
        [Header("Quick Bar")]
        public Sprite QuickBarDefaultIcon;
        public Sprite QuickBarSelectedIcon;

        [Header("Backpack")]
        public Sprite BackpackUnlockedIcon;
        public Sprite BackpackLockedIcon;
        public Sprite BackpackPreviewValidSprite;
        public Sprite BackpackPreviewInvalidSprite;
    }
}
