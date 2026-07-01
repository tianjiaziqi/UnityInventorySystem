using UnityEngine;
using UnityEngine.UI;

namespace JZQ.InventorySystem.Runtime.Inventory.Backpack.UI
{
    /// <summary>
    /// Displays the visual state of a single backpack grid cell.
    /// </summary>
    public class SlotCellView : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        private Sprite unlockedSprite;
        private Sprite lockedSprite;
        private Sprite previewValidSprite;
        private Sprite previewInvalidSprite;
        private bool unlocked;

        /// <summary>
        /// Gets the grid x coordinate represented by this cell.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Gets the grid y coordinate represented by this cell.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Initializes the cell view with grid coordinates and state sprites.
        /// </summary>
        /// <param name="x">The grid x coordinate.</param>
        /// <param name="y">The grid y coordinate.</param>
        /// <param name="unlockedIcon">The sprite for unlocked state.</param>
        /// <param name="lockedIcon">The sprite for locked state.</param>
        /// <param name="previewValidIcon">The sprite for valid preview state.</param>
        /// <param name="previewInvalidIcon">The sprite for invalid preview state.</param>
        public void Init(int x, int y, Sprite unlockedIcon, Sprite lockedIcon, Sprite previewValidIcon, Sprite previewInvalidIcon)
        {
            X = x;
            Y = y;
            unlockedSprite = unlockedIcon;
            lockedSprite = lockedIcon;
            previewInvalidSprite = previewInvalidIcon;
            previewValidSprite = previewValidIcon;
        }

        /// <summary>
        /// Updates the unlocked visual state of the cell.
        /// </summary>
        /// <param name="isUnlocked">Whether the cell is unlocked.</param>
        public void SetUnlocked(bool isUnlocked)
        {
            unlocked = isUnlocked;
            Sprite targetSprite = unlocked ? unlockedSprite : lockedSprite;

            if (backgroundImage.sprite != targetSprite)
            {
                backgroundImage.sprite = targetSprite;
            }
        }

        /// <summary>
        /// Displays a placement preview state.
        /// </summary>
        /// <param name="valid">Whether the preview should use the valid state.</param>
        public void ShowPreview(bool valid)
        {
            backgroundImage.sprite = valid ? previewValidSprite : previewInvalidSprite;
        }

        /// <summary>
        /// Restores the normal locked or unlocked sprite after previewing.
        /// </summary>
        public void ClearPreview()
        {
            backgroundImage.sprite = unlocked ? unlockedSprite : lockedSprite;
        }
    }
}
