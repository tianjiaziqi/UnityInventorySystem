using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JZQ.InventorySystem.Runtime.Core;
using JZQ.InventorySystem.Runtime.Inventory.Backpack;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using InventoryRuntimeSystem = JZQ.InventorySystem.Runtime.Core.InventorySystem;

namespace JZQ.InventorySystem.Runtime.Inventory.Backpack.UI
{
public class InventoryGridView : MonoBehaviour
{
    // 格子背景层
    [SerializeField] private RectTransform slotLayer;

    //物品层
    [SerializeField] private RectTransform itemLayer;

    //预览层
    [SerializeField] private RectTransform previewLayer;

    //格子预制体
    [SerializeField] private SlotCellView slotPrefab;

    //物品框架预制体, 运行时更新信息
    [SerializeField] private InventoryItemView itemPrefab;

    //格子大小
    private float cellSize = 64f;

    // 格子间距
    private Vector2 spacing = Vector2.zero;

    //运行时存储生成的格子, 索引与坐标匹配
    private SlotCellView[,] slotCells;

    //运行时缓存的物品UI, key为Instance ID
    private Dictionary<string, InventoryItemView> itemViews = new();

    //初始化完成标识
    private bool initialized;

    [SerializeField] private GridLayoutGroup slotGridLayout;

    private BackpackLayoutConfig dataConfig;
    
    private InventoryViewConfig viewConfig;

    //拖动时的影子预制体
    [SerializeField] private InventoryDragGhostView dragGhostPrefab;

    // 当前影子
    private InventoryDragGhostView currentGhost;

    //是否正在拖拽
    private bool isDragging;

    //拖拽物品的实例ID
    private string draggingInstanceId;

    //拖拽的物品实例
    private PlacedItem draggingItem;
    
    private InventoryItemView draggingItemView;

    //原始位置
    private Vector2Int originalPosition;

    //原始旋转
    private bool originalRotated;

    //当前旋转
    private bool currentRotated;

    //当前鼠标所在格子
    private Vector2Int hoverGridPosition;

    //当前是否可以放置
    private bool currentPlacementValid;
    //当前鼠标位置
    private Vector2 currentPointerScreenPosition;

    // 标记拖拽是否已经被外部处理
    private bool dragHandledExternally;


    /// <summary>
    /// 检查是否已经初始化, 若未初始化则进行
    /// </summary>
    public void InitializeIfNeeded(BackpackLayoutConfig dataConfig, InventoryViewConfig viewConfig)
    {
        if (!initialized) Initialize(dataConfig, viewConfig);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Initialize(BackpackLayoutConfig dataConfig, InventoryViewConfig viewConfig)
    {
        this.dataConfig = dataConfig;
        this.viewConfig = viewConfig;
        cellSize = dataConfig.CellSize;
        spacing = dataConfig.CellSpacing;
        slotCells = new SlotCellView[dataConfig.MaxSize.x, dataConfig.MaxSize.y];
        slotGridLayout.cellSize = new Vector2(cellSize, cellSize);
        slotGridLayout.spacing = spacing;
        slotGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        slotGridLayout.constraintCount = dataConfig.MaxSize.x;
        CreateSlots();
        initialized = true;
    }

    /// <summary>
    /// 创建所有格子
    /// </summary>
    private void CreateSlots()
    {
        for (int y = 0; y < dataConfig.MaxSize.y; y++)
        {
            for (int x = 0; x < dataConfig.MaxSize.x; x++)
            {
                var obj = Instantiate(slotPrefab, slotLayer);
                var cell = obj.GetComponent<SlotCellView>();
                cell.Init(x, y, viewConfig.BackpackUnlockedIcon, viewConfig.BackpackLockedIcon, viewConfig.BackpackPreviewValidSprite, viewConfig.BackpackPreviewInvalidSprite);
                slotCells[x, y] = cell;
            }
        }
    }

    /// <summary>
    /// 刷新所有状态
    /// </summary>
    public void RefreshAll()
    {
        RefreshSlots();
        RefreshItems();
    }

    /// <summary>
    /// 刷新格子状态
    /// </summary>
    private void RefreshSlots()
    {
        for (int x = 0; x < slotCells.GetLength(0); x++)
        {
            for (int y = 0; y < slotCells.GetLength(1); y++)
            {
                slotCells[x, y].SetUnlocked(InventoryRuntimeSystem.Current.IsPlayerGridUnlocked(x, y));
            }
        }
    }


    /// <summary>
    /// 根据玩家背包中的数据刷新背包层物品状态
    /// </summary>
    private void RefreshItems()
    {
        HashSet<string> instanceIds = new();
        InventoryItemView itemView;
        foreach (var placedItem in InventoryRuntimeSystem.Current.GetPlayerPlacedItems())
        {
            instanceIds.Add(placedItem.InstanceItem.InstanceID);
            if (itemViews.ContainsKey(placedItem.InstanceItem.InstanceID))
            {
                itemView = itemViews[placedItem.InstanceItem.InstanceID];
                itemView.SetCount(placedItem.InstanceItem.StackCount);
                itemView.SetPosition(GridToLocalPosition(placedItem.Position.x, placedItem.Position.y));
                itemView.SetSize(GetItemSize(placedItem.Width, placedItem.Height));
                itemView.SetVisualLayout(itemView.GetSize(),placedItem.Rotated);
            }
            else
            {
                itemView = CreateItemView(placedItem);
                itemViews.Add(placedItem.InstanceItem.InstanceID, itemView);
            }
        }

        List<string> keys = new();

        foreach (var key in itemViews.Keys)
        {
            if (!instanceIds.Contains(key))
            {
                keys.Add(key);
            }
        }

        foreach (var key in keys)
        {
            Destroy(itemViews[key].gameObject);
            itemViews.Remove(key);
        }
    }

    /// <summary>
    /// 创建物品UI层实例
    /// </summary>
    /// <param name="placedItem">放下的物品</param>
    /// <returns></returns>
    private InventoryItemView CreateItemView(PlacedItem placedItem)
    {
        InventoryItemView itemView = Instantiate(itemPrefab, itemLayer).GetComponent<InventoryItemView>();
        itemView.Bind(placedItem);
        itemView.SetPosition(GridToLocalPosition(placedItem.Position.x, placedItem.Position.y));
        itemView.SetSize(GetItemSize(placedItem.Width, placedItem.Height));
        itemView.SetOwner(this);
        return itemView;
    }

    /// <summary>
    /// 格子坐标与本地坐标换算
    /// </summary>
    /// <param name="x">格子x坐标</param>
    /// <param name="y">格子y坐标</param>
    /// <returns>本地坐标</returns>
    private Vector2 GridToLocalPosition(int x, int y)
    {
        float posX = x * (cellSize + spacing.x);
        float posY = -y * (cellSize + spacing.y);
        return new Vector2(posX, posY);
    }

    /// <summary>
    /// 根据物品长宽计算物品尺寸
    /// </summary>
    /// <param name="width">物品宽度</param>
    /// <param name="height">物品高度</param>
    /// <returns>物品尺寸</returns>
    private Vector2 GetItemSize(int width, int height)
    {
        float sizeX = width * cellSize + (width - 1) * spacing.x;
        float sizeY = height * cellSize + (height - 1) * spacing.y;
        return new Vector2(sizeX, sizeY);
    }

    /// <summary>
    /// 获取特定位置的格子视图
    /// </summary>
    /// <param name="x">格子x坐标</param>
    /// <param name="y">格子y坐标</param>
    /// <returns>格子视图</returns>
    public SlotCellView GetSlotCell(int x, int y)
    {
        return slotCells[x, y];
    }

    /// <summary>
    /// 获取对应屏幕位置的格子
    /// </summary>
    /// <param name="screenPosition"> 屏幕位置</param>
    /// <param name="x">格子x坐标</param>
    /// <param name="y">格子y坐标</param>
    /// <returns>是否成功获取格子</returns>
    public bool TryGetGridPosition(Vector2 screenPosition, out int x, out int y)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(itemLayer, screenPosition, null, out var point))
        {
            x = (int)Math.Floor(point.x / (cellSize + spacing.x));
            y = (int)Math.Floor(-point.y / (cellSize + spacing.y));
            return x >= 0 && y >= 0 && x < dataConfig.MaxSize.x && y < dataConfig.MaxSize.y;
        }

        x = 0;
        y = 0;
        return false;
    }

    /// <summary>
    /// 显示放置预览
    /// </summary>
    /// <param name="x">格子x坐标</param>
    /// <param name="y">格子y坐标</param>
    /// <param name="width">物品宽度</param>
    /// <param name="height">物品高度</param>
    /// <param name="valid">是否有效</param>
    public void ShowPlacementPreview(int x, int y, int width, int height, bool valid)
    {
        if (x < 0 || y < 0 || x + width > dataConfig.MaxSize.x || y + height > dataConfig.MaxSize.y) return;
        for (int i = x; i < x + width && i < dataConfig.MaxSize.x; i++)
        {
            for (int j = y; j < y + height && j < dataConfig.MaxSize.y; j++)
            {
                if (slotCells[i, j] != null)
                {
                    slotCells[i, j].ShowPreview(valid);
                }
            }
        }
    }

    /// <summary>
    /// 清除所有预览
    /// </summary>
    public void ClearPreview()
    {
        foreach (var cell in slotCells)
        {
            cell.ClearPreview();
        }
    }

    public void BeginItemDrag(InventoryItemView itemView, PointerEventData eventData)
    {
        draggingInstanceId = itemView.GetInstanceId();
        draggingItem = itemView.GetPlacedItem();
        draggingItemView = itemView;
        if (draggingItem == null)
        {
            CancelCurrentDrag();
            return;
        }
        isDragging = true;
        
        originalPosition = draggingItem.Position;
        originalRotated = currentRotated= draggingItem.Rotated;
        currentPlacementValid = false;
        currentPointerScreenPosition = eventData.position;
        currentGhost = CreateDragGhost(itemView);
        itemView.HideVisual();
        UpdateDragPreview(eventData.position);
        UpdateGhostPosition(eventData.position);
    }

    public void UpdateItemDrag(PointerEventData eventData)
    {
        if(!isDragging) return;
        currentPointerScreenPosition = eventData.position;
        UpdateGhostPosition(eventData.position);
        UpdateDragPreview(eventData.position);
    }

    public void EndItemDrag(PointerEventData eventData)
    {
        if(!isDragging) return;
        if (dragHandledExternally)
        {
            CancelCurrentDrag();
            dragHandledExternally = false;
            return;
        }
        if (currentPlacementValid)
        {
            InventoryRuntimeSystem.Current.TryMoveItemFromPlayer(draggingInstanceId, hoverGridPosition.x, hoverGridPosition.y, currentRotated);
        }
        CancelCurrentDrag();
    }

    public void CancelCurrentDrag()
    {
        ClearPreview();
        ClearCurrentDragState();
        RefreshItems();
    }

    public void RotateCurrentDrag()
    {
        if(!isDragging || currentGhost == null) return;
        if (!draggingItem.InstanceItem.Definition.CanRotate) return;
        currentRotated = !currentRotated;
        int width = currentRotated ? draggingItem.InstanceItem.Definition.Height : draggingItem.InstanceItem.Definition.Width;
        int height = currentRotated ? draggingItem.InstanceItem.Definition.Width : draggingItem.InstanceItem.Definition.Height;
        currentGhost.SetSize(GetItemSize(width, height));
        currentGhost.SetVisualLayout(currentGhost.GetSize(),currentRotated);
        UpdateGhostPosition(currentPointerScreenPosition);
        UpdateDragPreview(currentPointerScreenPosition);
    }

    private void UpdateDragPreview(Vector2 screenPosition)
    {
        ClearPreview();

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                itemLayer,
                screenPosition,
                null,
                out var point))
        {
            currentPlacementValid = false;
            return;
        }

        

        int hoveredCellX = (int)Math.Floor(point.x / (cellSize + spacing.x));
        int hoveredCellY = (int)Math.Floor(-point.y / (cellSize + spacing.y));

        int widthCells = currentRotated ? draggingItem.InstanceItem.Definition.Height : draggingItem.InstanceItem.Definition.Width;
        int heightCells = currentRotated ? draggingItem.InstanceItem.Definition.Width : draggingItem.InstanceItem.Definition.Height;

        int pivotOffsetX = (widthCells - 1) / 2;
        int pivotOffsetY = (heightCells - 1) / 2;

        int x = hoveredCellX - pivotOffsetX;
        int y = hoveredCellY - pivotOffsetY;

        if (x < 0 || y < 0 || x + widthCells > dataConfig.MaxSize.x || y + heightCells > dataConfig.MaxSize.y)
        {
            currentPlacementValid = false;
            return;
        }

        hoverGridPosition = new Vector2Int(x, y);
        ApplyPlacementPreview(hoverGridPosition);
        //进行格子吸附, 注释后可自由跟随鼠标移动
        currentGhost.SetLocalPosition(GridToLocalPosition(x, y));
    }

    private void UpdateGhostPosition(Vector2 screenPosition)
    {
        if (currentGhost == null) return;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(previewLayer, screenPosition, null, out var point))
        {
            Vector2 size = GetCurrentDragItemSize();
            Vector2 topLeft = point - new Vector2(size.x / 2, -size.y / 2);
            currentGhost.SetLocalPosition(topLeft);
        }
    }

    private Vector2 GetCurrentDragItemSize()
    {
        if (draggingItem == null) return Vector2.zero;

        int width = currentRotated
            ? draggingItem.InstanceItem.Definition.Height
            : draggingItem.InstanceItem.Definition.Width;

        int height = currentRotated
            ? draggingItem.InstanceItem.Definition.Width
            : draggingItem.InstanceItem.Definition.Height;

        return GetItemSize(width, height);
    }

    private void ApplyPlacementPreview(Vector2Int hoveredGridPos)
    {
        currentPlacementValid = InventoryRuntimeSystem.Current.CanMovePlayerItemTo(draggingInstanceId, hoveredGridPos.x, hoveredGridPos.y, currentRotated);
        int width = currentRotated ? draggingItem.InstanceItem.Definition.Height : draggingItem.InstanceItem.Definition.Width;
        int height = currentRotated ? draggingItem.InstanceItem.Definition.Width : draggingItem.InstanceItem.Definition.Height;
        
        ShowPlacementPreview(hoveredGridPos.x, hoveredGridPos.y, width, height, currentPlacementValid);
    }

    private void ClearCurrentDragState()
    {
        if (draggingItemView != null)
        {
            draggingItemView.ShowVisual();
            draggingItemView = null;
        }
        
        isDragging = false;
        draggingInstanceId = null;
        draggingItem = null;
        currentPlacementValid = false;
        if (currentGhost != null)
        {
            Destroy(currentGhost.gameObject);
            currentGhost = null;
        }
    }

    private InventoryDragGhostView CreateDragGhost(InventoryItemView itemView)
    {
        InventoryDragGhostView ghost = Instantiate(dragGhostPrefab, previewLayer);
        ghost.SetIcon(itemView.GetIcon());
        int width = currentRotated ? draggingItem.InstanceItem.Definition.Height : draggingItem.InstanceItem.Definition.Width;
        int height = currentRotated ? draggingItem.InstanceItem.Definition.Width : draggingItem.InstanceItem.Definition.Height;
        ghost.SetSize(GetItemSize(width, height));
        ghost.SetAlpha(0.6f);
        ghost.SetVisualLayout(ghost.GetSize(),currentRotated);
        return ghost;
    }

    public bool TryBindDraggedItemToQuickSlot(int slotIndex)
    {
        if (!isDragging) return false;
        InventoryRuntimeSystem.Current.BindPlayerQuickSlot(slotIndex, draggingInstanceId);
        dragHandledExternally = true;
        return true;
    }
}
}
