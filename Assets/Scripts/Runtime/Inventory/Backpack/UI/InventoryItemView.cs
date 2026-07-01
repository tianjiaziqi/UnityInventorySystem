using TMPro;
using JZQ.InventorySystem.Runtime.Inventory.Backpack;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JZQ.InventorySystem.Runtime.Inventory.Backpack.UI
{
    /// <summary>
    /// Displays a placed inventory item and forwards drag interactions to the grid view.
    /// </summary>
    public class InventoryItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text countText;

        private string instanceId;
        private PlacedItem boundItem;
        private bool selected;
        private InventoryGridView ownerGridView;
        private bool rotated;

        /// <summary>
        /// Binds a placed item to this view.
        /// </summary>
        /// <param name="placedItem">The placed item represented by this view.</param>
        public void Bind(PlacedItem placedItem)
        {
            instanceId = placedItem.InstanceItem.InstanceID;
            boundItem = placedItem;
            iconImage.sprite = placedItem.InstanceItem.Definition.Icon;
            rotated = placedItem.Rotated;
            SetCount(placedItem.InstanceItem.StackCount);
        }

        /// <summary>
        /// Assigns the owner grid view for drag forwarding.
        /// </summary>
        /// <param name="gridView">The owner grid view.</param>
        public void SetOwner(InventoryGridView gridView)
        {
            ownerGridView = gridView;
        }

        /// <summary>
        /// Sets the local position of the item view.
        /// </summary>
        /// <param name="localPosition">The local position to apply.</param>
        public void SetPosition(Vector2 localPosition)
        {
            rectTransform.localPosition = localPosition;
        }

        /// <summary>
        /// Sets the root size of the item view.
        /// </summary>
        /// <param name="size">The target size.</param>
        public void SetSize(Vector2 size)
        {
            rectTransform.sizeDelta = size;
            SetVisualLayout(size, rotated);
        }

        /// <summary>
        /// Updates the item layout to match the specified size and rotation.
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
        /// Gets the current root size of the item view.
        /// </summary>
        /// <returns>The current size.</returns>
        public Vector2 GetSize()
        {
            return rectTransform.sizeDelta;
        }

        /// <summary>
        /// Sets the selection state of the view.
        /// </summary>
        /// <param name="selected">Whether the view is selected.</param>
        public void SetSelected(bool selected)
        {
            this.selected = selected;
        }

        /// <summary>
        /// Gets the bound item instance identifier.
        /// </summary>
        /// <returns>The bound item instance identifier.</returns>
        public string GetInstanceId()
        {
            return instanceId;
        }

        /// <summary>
        /// Gets the bound placed item.
        /// </summary>
        /// <returns>The placed item represented by this view.</returns>
        public PlacedItem GetPlacedItem()
        {
            return boundItem;
        }

        /// <summary>
        /// Updates the displayed stack count.
        /// </summary>
        /// <param name="count">The stack count to display.</param>
        public void SetCount(int count)
        {
            countText.text = count > 1 ? count.ToString() : "";
        }

        /// <summary>
        /// Gets the icon sprite for the bound item.
        /// </summary>
        /// <returns>The bound item icon sprite.</returns>
        public Sprite GetIcon()
        {
            return boundItem.InstanceItem.Definition.Icon;
        }

        /// <summary>
        /// Starts an item drag interaction.
        /// </summary>
        /// <param name="eventData">The Unity drag event data.</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                ownerGridView.BeginSplitDrag(this, eventData);
                return;
            }

            ownerGridView.BeginItemDrag(this, eventData);
        }

        /// <summary>
        /// Updates an active item drag interaction.
        /// </summary>
        /// <param name="eventData">The Unity drag event data.</param>
        public void OnDrag(PointerEventData eventData)
        {
            ownerGridView.UpdateItemDrag(eventData);
        }

        /// <summary>
        /// Completes an active item drag interaction.
        /// </summary>
        /// <param name="eventData">The Unity drag event data.</param>
        public void OnEndDrag(PointerEventData eventData)
        {
            ownerGridView.EndItemDrag(eventData);
        }

        /// <summary>
        /// Hides the item icon and count visuals.
        /// </summary>
        public void HideVisual()
        {
            iconImage.enabled = false;
            countText.enabled = false;
        }

        /// <summary>
        /// Shows the item icon and count visuals.
        /// </summary>
        public void ShowVisual()
        {
            iconImage.enabled = true;
            countText.enabled = true;
        }

        /// <summary>
        /// Gets the owner grid view.
        /// </summary>
        /// <returns>The owner grid view.</returns>
        public InventoryGridView GetOwner()
        {
            return ownerGridView;
        }
    }
}
