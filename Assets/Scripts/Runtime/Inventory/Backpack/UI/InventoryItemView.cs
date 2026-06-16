using System.Collections;
using System.Collections.Generic;
using TMPro;
using JZQ.InventorySystem.Runtime.Inventory.Backpack;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JZQ.InventorySystem.Runtime.Inventory.Backpack.UI
{
public class InventoryItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text countText;

    //实例ID
    private string instanceId;
    //绑定的物品
    private PlacedItem boundItem;
    //是否被选中
    private bool selected;
    //所属的网格视图
    private InventoryGridView ownerGridView;

    private bool rotated;

    /// <summary>
    /// 将物品绑定到视图
    /// </summary>
    /// <param name="placedItem">要绑定的物品</param>
    public void Bind(PlacedItem placedItem)
    {
        instanceId = placedItem.InstanceItem.InstanceID;
        boundItem = placedItem;
        iconImage.sprite = placedItem.InstanceItem.Definition.Icon;
        rotated = placedItem.Rotated;
        SetCount(placedItem.InstanceItem.StackCount);
    }

    /// <summary>
    /// 设置物品的所属网格视图
    /// </summary>
    /// <param name="gridView">网格视图</param>
    public void SetOwner(InventoryGridView gridView)
    {
        ownerGridView = gridView;
    }
    
    /// <summary>
    /// 设置本地坐标
    /// </summary>
    /// <param name="localPosition">本地坐标</param>

    public void SetPosition(Vector2 localPosition)
    {
        rectTransform.localPosition = localPosition;
    }

    /// <summary>
    /// 设置尺寸
    /// </summary>
    /// <param name="size">尺寸</param>
    public void SetSize(Vector2 size)
    {
        rectTransform.sizeDelta = size;
        SetVisualLayout(size, rotated);
    }
    
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
    
    

    public Vector2 GetSize()
    {
        return rectTransform.sizeDelta;
    }

    /// <summary>
    /// 设置选中状态
    /// </summary>
    /// <param name="selected">选中状态</param>
    public void SetSelected(bool selected)
    {
        this.selected = selected;
    }

    /// <summary>
    /// 获取绑定物品的实例ID
    /// </summary>
    /// <returns>绑定物品的实例ID</returns>
    public string GetInstanceId()
    {
        return instanceId;
    }

    /// <summary>
    /// 获取绑定的物品实例
    /// </summary>
    /// <returns>绑定的物品实例</returns>
    public PlacedItem GetPlacedItem()
    {
        return boundItem;
    }

    /// <summary>
    /// 设置物品数量
    /// </summary>
    /// <param name="count">数量</param>
    public void SetCount(int count)
    {
        countText.text = count > 1 ? count.ToString() : "";
    }

    /// <summary>
    /// 获取物品图标
    /// </summary>
    /// <returns>物品图标</returns>
    public Sprite GetIcon()
    {
        return boundItem.InstanceItem.Definition.Icon;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        ownerGridView.BeginItemDrag(this, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        ownerGridView.UpdateItemDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ownerGridView.EndItemDrag(eventData);
    }

    public void HideVisual()
    {
        iconImage.enabled = false;
        countText.enabled = false;
    }
    public void ShowVisual()
    {
        iconImage.enabled = true;
        countText.enabled = true;
    }

    public InventoryGridView GetOwner()
    {
        return ownerGridView;   
    }
}
}
