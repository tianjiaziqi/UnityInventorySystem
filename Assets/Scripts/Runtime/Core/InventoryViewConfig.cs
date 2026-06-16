using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Core
{
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
