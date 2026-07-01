using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Core
{
    /// <summary>
    /// Defines shared visual assets used by inventory UI views.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/InventorySystem/InventoryViewConfig")]
    public class InventoryViewConfig : ScriptableObject
    {
        [Header("Quick Bar")]

        /// <summary>
        /// Default quick bar slot background sprite.
        /// </summary>
        public Sprite QuickBarDefaultIcon;

        /// <summary>
        /// Selected quick bar slot background sprite.
        /// </summary>
        public Sprite QuickBarSelectedIcon;

        [Header("Backpack")]

        /// <summary>
        /// Sprite used for unlocked backpack cells.
        /// </summary>
        public Sprite BackpackUnlockedIcon;

        /// <summary>
        /// Sprite used for locked backpack cells.
        /// </summary>
        public Sprite BackpackLockedIcon;

        /// <summary>
        /// Sprite used for valid placement previews.
        /// </summary>
        public Sprite BackpackPreviewValidSprite;

        /// <summary>
        /// Sprite used for invalid placement previews.
        /// </summary>
        public Sprite BackpackPreviewInvalidSprite;
    }
}
