using System.Collections;
using System.Collections.Generic;
using JZQ.InventorySystem.Runtime.Inventory.Backpack.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JZQ.InventorySystem.Runtime.Inventory.QuickBar.UI
{
public class QuickBarSlotView : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text countText;
    private Sprite unSelectedIcon;
    private Sprite selectedIcon;
    private Sprite currentItemIcon;
    private int index;

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
    
    public void SetSelection(bool selected)
    {
        backgroundImage.sprite = selected ? selectedIcon : unSelectedIcon;
    }

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

        if (countText.enabled == false)
        {
            countText.enabled = true;
        }

        if (iconImage.enabled == false)
        {
            iconImage.enabled = true;
        }
        iconImage.sprite = itemIcon;
        currentItemIcon = itemIcon;
    }

    public void SetCount(int count)
    {
        countText.text = count.ToString();
    }

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
