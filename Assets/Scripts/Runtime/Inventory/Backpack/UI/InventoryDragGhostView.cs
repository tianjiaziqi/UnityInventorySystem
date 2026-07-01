using UnityEngine;
using UnityEngine.UI;

namespace JZQ.InventorySystem.Runtime.Inventory.Backpack.UI
{
    /// <summary>
    /// Displays the temporary visual used while dragging an inventory item.
    /// </summary>
    public class InventoryDragGhostView : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image iconImage;
        [SerializeField] private CanvasGroup canvasGroup;

        private bool rotated;

        /// <summary>
        /// Sets the ghost icon sprite.
        /// </summary>
        /// <param name="icon">The icon sprite to display.</param>
        public void SetIcon(Sprite icon)
        {
            iconImage.sprite = icon;
        }

        /// <summary>
        /// Sets the ghost size.
        /// </summary>
        /// <param name="size">The target size in local UI units.</param>
        public void SetSize(Vector2 size)
        {
            rectTransform.sizeDelta = size;
            SetVisualLayout(size, rotated);
        }

        /// <summary>
        /// Gets the current ghost size.
        /// </summary>
        /// <returns>The current size in local UI units.</returns>
        public Vector2 GetSize()
        {
            return rectTransform.sizeDelta;
        }

        /// <summary>
        /// Updates the ghost layout to match the specified size and rotation state.
        /// </summary>
        /// <param name="rootSize">The base item size.</param>
        /// <param name="rotated">Whether the item is rotated.</param>
        public void SetVisualLayout(Vector2 rootSize, bool rotated)
        {
            this.rotated = rotated;
            if (!rotated)
            {
                iconImage.rectTransform.sizeDelta = new Vector2(rootSize.x, rootSize.y);
            }
            else
            {
                iconImage.rectTransform.sizeDelta = new Vector2(rootSize.y, rootSize.x);
            }

            iconImage.rectTransform.localEulerAngles = new Vector3(0, 0, rotated ? 90 : 0);
        }

        /// <summary>
        /// Sets the local anchored position of the ghost.
        /// </summary>
        /// <param name="localPosition">The local position to apply.</param>
        public void SetLocalPosition(Vector2 localPosition)
        {
            rectTransform.localPosition = localPosition;
        }

        /// <summary>
        /// Sets the ghost alpha.
        /// </summary>
        /// <param name="alpha">The alpha value to apply.</param>
        public void SetAlpha(float alpha)
        {
            iconImage.color = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, alpha);
        }
    }
}
