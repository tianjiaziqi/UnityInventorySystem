using JZQ.InventorySystem.Runtime.Inventory.Backpack.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JZQ.InventorySystem.Runtime.Inventory.QuickBar.UI
{
    /// <summary>
    /// Displays a single quick bar slot and handles drop bindings.
    /// </summary>
    public class QuickBarSlotView : MonoBehaviour, IDropHandler
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text countText;
        private Sprite unSelectedIcon;
        private Sprite selectedIcon;
        private Sprite currentItemIcon;
        private int index;

        /// <summary>
        /// Initializes the slot view.
        /// </summary>
        /// <param name="index">The quick bar slot index.</param>
        /// <param name="unselectedSprite">The default slot background sprite.</param>
        /// <param name="selectedSprite">The selected slot background sprite.</param>
        /// <param name="selected">Whether the slot starts selected.</param>
        /// <param name="itemIcon">An optional initial item icon.</param>
        public void Init(int index, Sprite unselectedSprite, Sprite selectedSprite, bool selected, Sprite itemIcon = null)
        {
            unSelectedIcon = unselectedSprite;
            selectedIcon = selectedSprite;
            this.index = index;
            backgroundImage.sprite = selected ? selectedIcon : unSelectedIcon;
            if (itemIcon != null)
            {
                iconImage.sprite = itemIcon;
                currentItemIcon = itemIcon;
            }
        }

        /// <summary>
        /// Updates the selected visual state of the slot.
        /// </summary>
        /// <param name="selected">Whether the slot is selected.</param>
        public void SetSelection(bool selected)
        {
            backgroundImage.sprite = selected ? selectedIcon : unSelectedIcon;
        }

        /// <summary>
        /// Sets the icon displayed by the slot.
        /// </summary>
        /// <param name="itemIcon">The item icon to display, or <c>null</c> to clear the slot.</param>
        public void SetItem(Sprite itemIcon)
        {
            if (itemIcon == null)
            {
                countText.text = "";
                countText.enabled = false;
                iconImage.enabled = false;
                currentItemIcon = null;
                return;
            }

            if (currentItemIcon == itemIcon) return;

            if (!countText.enabled)
            {
                countText.enabled = true;
            }

            if (!iconImage.enabled)
            {
                iconImage.enabled = true;
            }

            iconImage.sprite = itemIcon;
            currentItemIcon = itemIcon;
        }

        /// <summary>
        /// Sets the displayed stack count for the slot item.
        /// </summary>
        /// <param name="count">The stack count to display.</param>
        public void SetCount(int count)
        {
            countText.text = count.ToString();
        }

        /// <summary>
        /// Handles a dropped inventory item and attempts to bind it to this slot.
        /// </summary>
        /// <param name="eventData">The Unity pointer event data.</param>
        public void OnDrop(PointerEventData eventData)
        {
            GameObject draggingObj = eventData.pointerDrag;
            InventoryItemView itemView = draggingObj.GetComponent<InventoryItemView>();
            if (itemView == null) return;
            InventoryGridView ownerView = itemView.GetOwner();
            if (ownerView != null)
            {
                ownerView.TryBindDraggedItemToQuickSlot(index);
            }
        }
    }
}
