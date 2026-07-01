using System;
using System.Collections.Generic;
using JZQ.InventorySystem.Runtime.Core;
using JZQ.InventorySystem.Runtime.Data;
using JZQ.InventorySystem.Runtime.Inventory.Backpack;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using InventoryRuntimeSystem = JZQ.InventorySystem.Runtime.Core.InventorySystem;

namespace JZQ.InventorySystem.Runtime.Inventory.Backpack.UI
{
public class InventoryGridView : MonoBehaviour
{
    private enum InventoryDragMode
    {
        None,
        MoveWholeStack,
        SplitStack
    }

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
    private readonly Dictionary<string, InventoryItemView> itemViews = new();

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
    private InventoryDragMode dragMode;
    private ItemInstance splitPreviewItem;
    private int splitDragCount;
    private string currentMergeTargetId;

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

    private IBackpackViewRuntime backpackView;
    private IBackpackCommandRuntime backpackCommands;

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
        backpackView = InventoryRuntimeSystem.BackpackViewRuntime;
        backpackCommands = InventoryRuntimeSystem.BackpackCommandRuntime;
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
                slotCells[x, y].SetUnlocked(backpackView.IsPlayerGridUnlocked(x, y));
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
        foreach (var placedItem in backpackView.GetPlayerPlacedItems())
        {
            instanceIds.Add(placedItem.InstanceItem.InstanceID);
            if (itemViews.ContainsKey(placedItem.InstanceItem.InstanceID))
            {
                itemView = itemViews[placedItem.InstanceItem.InstanceID];
                itemView.Bind(placedItem);
                itemView.SetCount(placedItem.InstanceItem.StackCount);
                itemView.SetPosition(GridToLocalPosition(placedItem.Position.x, placedItem.Position.y));
                itemView.SetSize(GetItemSize(placedItem.Width, placedItem.Height));
                itemView.SetVisualLayout(itemView.GetSize(), placedItem.Rotated);
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
    private Vector2 GridToLocalPosition(int x, int y)
    {
        float posX = x * (cellSize + spacing.x);
        float posY = -y * (cellSize + spacing.y);
        return new Vector2(posX, posY);
    }

    /// <summary>
    /// 根据物品长宽计算物品尺寸
    /// </summary>
    private Vector2 GetItemSize(int width, int height)
    {
        float sizeX = width * cellSize + (width - 1) * spacing.x;
        float sizeY = height * cellSize + (height - 1) * spacing.y;
        return new Vector2(sizeX, sizeY);
    }

    /// <summary>
    /// 获取特定位置的格子视图
    /// </summary>
    public SlotCellView GetSlotCell(int x, int y)
    {
        return slotCells[x, y];
    }

    /// <summary>
    /// 获取对应屏幕位置的格子
    /// </summary>
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
        if (eventData.button != PointerEventData.InputButton.Left) return;
        BeginDrag(itemView, eventData, InventoryDragMode.MoveWholeStack, 0);
    }

    public void BeginSplitDrag(InventoryItemView itemView, PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;

        var placedItem = itemView.GetPlacedItem();
        if (placedItem == null) return;

        int splitCount = Mathf.CeilToInt(placedItem.InstanceItem.StackCount * 0.5f);
        if (splitCount <= 0 || splitCount >= placedItem.InstanceItem.StackCount) return;

        BeginDrag(itemView, eventData, InventoryDragMode.SplitStack, splitCount);
    }

    private void BeginDrag(InventoryItemView itemView, PointerEventData eventData, InventoryDragMode mode, int pendingSplitCount)
    {
        if (itemView == null) return;
        if (isDragging) CancelCurrentDrag();

        draggingInstanceId = itemView.GetInstanceId();
        draggingItem = itemView.GetPlacedItem();
        draggingItemView = itemView;
        if (draggingItem == null)
        {
            CancelCurrentDrag();
            return;
        }

        dragMode = mode;
        splitDragCount = pendingSplitCount;
        splitPreviewItem = mode == InventoryDragMode.SplitStack
            ? new ItemInstance(draggingItem.InstanceItem.Definition, Guid.NewGuid().ToString(), pendingSplitCount)
            : null;

        isDragging = true;
        currentRotated = draggingItem.Rotated;
        currentPlacementValid = false;
        currentMergeTargetId = null;
        dragHandledExternally = false;
        currentPointerScreenPosition = eventData.position;
        currentGhost = CreateDragGhost(itemView);

        if (mode == InventoryDragMode.MoveWholeStack)
        {
            itemView.HideVisual();
        }

        UpdateDragPreview(eventData.position);
        UpdateGhostPosition(eventData.position);
    }

    public void UpdateItemDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        currentPointerScreenPosition = eventData.position;
        UpdateGhostPosition(eventData.position);
        UpdateDragPreview(eventData.position);
    }

    public void EndItemDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        if (dragHandledExternally)
        {
            CancelCurrentDrag();
            return;
        }

        if (dragMode == InventoryDragMode.MoveWholeStack)
        {
            CompleteMoveDrag(eventData.position);
        }
        else if (dragMode == InventoryDragMode.SplitStack)
        {
            CompleteSplitDrag(eventData.position);
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
        if (!isDragging || currentGhost == null) return;
        if (draggingItem == null || !draggingItem.InstanceItem.Definition.CanRotate) return;

        currentRotated = !currentRotated;
        currentGhost.SetSize(GetCurrentDragItemSize());
        currentGhost.SetVisualLayout(currentGhost.GetSize(), currentRotated);
        UpdateGhostPosition(currentPointerScreenPosition);
        UpdateDragPreview(currentPointerScreenPosition);
    }

    private void CompleteMoveDrag(Vector2 screenPosition)
    {
        if (backpackCommands == null || backpackView == null || draggingItem == null) return;

        if (!string.IsNullOrEmpty(currentMergeTargetId))
        {
            backpackCommands.TryMergeItems(draggingInstanceId, currentMergeTargetId);
            return;
        }

        if (currentPlacementValid)
        {
            backpackView.TryMoveItemFromPlayer(draggingInstanceId, hoverGridPosition.x, hoverGridPosition.y, currentRotated);
            return;
        }

        if (!IsPointerInsideGrid(screenPosition))
        {
            backpackCommands.TryDropItem(draggingInstanceId, draggingItem.InstanceItem.StackCount, out _);
        }
    }

    private void CompleteSplitDrag(Vector2 screenPosition)
    {
        if (backpackCommands == null || draggingItem == null) return;

        if (!string.IsNullOrEmpty(currentMergeTargetId))
        {
            backpackCommands.TryMergeSplitItems(draggingInstanceId, splitDragCount, currentMergeTargetId);
            return;
        }

        if (currentPlacementValid)
        {
            backpackCommands.TrySplitPlaceItem(draggingInstanceId, splitDragCount, hoverGridPosition.x, hoverGridPosition.y, currentRotated);
            return;
        }

        if (!IsPointerInsideGrid(screenPosition))
        {
            backpackCommands.TryDropItem(draggingInstanceId, splitDragCount, out _);
        }
    }

    private void UpdateDragPreview(Vector2 screenPosition)
    {
        ClearPreview();
        currentPlacementValid = false;
        currentMergeTargetId = null;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(itemLayer, screenPosition, null, out var point))
        {
            return;
        }

        int hoveredCellX = (int)Math.Floor(point.x / (cellSize + spacing.x));
        int hoveredCellY = (int)Math.Floor(-point.y / (cellSize + spacing.y));

        var mergeTarget = GetPlacedItemAtCell(hoveredCellX, hoveredCellY);

        if (TryGetPlacementOrigin(point, out var placementOrigin))
        {
            hoverGridPosition = placementOrigin;
            ApplyPlacementPreview(placementOrigin);

            if (currentPlacementValid)
            {
                currentGhost.SetLocalPosition(GridToLocalPosition(placementOrigin.x, placementOrigin.y));
                return;
            }
        }

        if (mergeTarget != null && CanMergeIntoTarget(mergeTarget))
        {
            currentMergeTargetId = mergeTarget.InstanceItem.InstanceID;
            ShowPlacementPreview(mergeTarget.Position.x, mergeTarget.Position.y, mergeTarget.Width, mergeTarget.Height, true);
            currentGhost.SetLocalPosition(GridToLocalPosition(mergeTarget.Position.x, mergeTarget.Position.y));
        }
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
        return GetItemSize(GetCurrentDragWidth(), GetCurrentDragHeight());
    }

    private int GetCurrentDragWidth()
    {
        return currentRotated ? draggingItem.InstanceItem.Definition.Height : draggingItem.InstanceItem.Definition.Width;
    }

    private int GetCurrentDragHeight()
    {
        return currentRotated ? draggingItem.InstanceItem.Definition.Width : draggingItem.InstanceItem.Definition.Height;
    }

    private ItemInstance GetCurrentDragItem()
    {
        if (dragMode == InventoryDragMode.SplitStack)
        {
            return splitPreviewItem;
        }

        return draggingItem?.InstanceItem;
    }

    private int GetCurrentDragStackCount()
    {
        return dragMode == InventoryDragMode.SplitStack ? splitDragCount : draggingItem?.InstanceItem.StackCount ?? 0;
    }

    private void ApplyPlacementPreview(Vector2Int hoveredGridPos)
    {
        var dragItem = GetCurrentDragItem();
        if (dragItem == null) return;

        currentPlacementValid = dragMode == InventoryDragMode.MoveWholeStack
            ? backpackView.CanMovePlayerItemTo(draggingInstanceId, hoveredGridPos.x, hoveredGridPos.y, currentRotated)
            : backpackView.CanPlaceItemInPlayer(dragItem, hoveredGridPos.x, hoveredGridPos.y, currentRotated);

        ShowPlacementPreview(hoveredGridPos.x, hoveredGridPos.y, GetCurrentDragWidth(), GetCurrentDragHeight(), currentPlacementValid);
    }

    private bool TryGetPlacementOrigin(Vector2 localPoint, out Vector2Int origin)
    {
        int hoveredCellX = (int)Math.Floor(localPoint.x / (cellSize + spacing.x));
        int hoveredCellY = (int)Math.Floor(-localPoint.y / (cellSize + spacing.y));

        int pivotOffsetX = (GetCurrentDragWidth() - 1) / 2;
        int pivotOffsetY = (GetCurrentDragHeight() - 1) / 2;

        int x = hoveredCellX - pivotOffsetX;
        int y = hoveredCellY - pivotOffsetY;

        if (x < 0 || y < 0 || x + GetCurrentDragWidth() > dataConfig.MaxSize.x || y + GetCurrentDragHeight() > dataConfig.MaxSize.y)
        {
            origin = default;
            return false;
        }

        origin = new Vector2Int(x, y);
        return true;
    }

    private PlacedItem GetPlacedItemAtCell(int x, int y)
    {
        if (x < 0 || y < 0 || x >= dataConfig.MaxSize.x || y >= dataConfig.MaxSize.y) return null;

        foreach (var placedItem in backpackView.GetPlayerPlacedItems())
        {
            if (x >= placedItem.Position.x && x < placedItem.Position.x + placedItem.Width &&
                y >= placedItem.Position.y && y < placedItem.Position.y + placedItem.Height)
            {
                return placedItem;
            }
        }

        return null;
    }

    private bool CanMergeIntoTarget(PlacedItem target)
    {
        if (target == null || draggingItem == null) return false;
        if (target.InstanceItem.InstanceID == draggingInstanceId) return false;
        if (target.InstanceItem.Definition != draggingItem.InstanceItem.Definition) return false;
        return target.InstanceItem.StackCount + GetCurrentDragStackCount() <= target.InstanceItem.Definition.MaxStack;
    }

    private bool IsPointerInsideGrid(Vector2 screenPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(itemLayer, screenPosition, null);
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
        dragMode = InventoryDragMode.None;
        splitPreviewItem = null;
        splitDragCount = 0;
        currentMergeTargetId = null;
        currentPlacementValid = false;
        dragHandledExternally = false;

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
        ghost.SetSize(GetCurrentDragItemSize());
        ghost.SetAlpha(0.6f);
        ghost.SetVisualLayout(ghost.GetSize(), currentRotated);
        return ghost;
    }

    public bool TryBindDraggedItemToQuickSlot(int slotIndex)
    {
        if (!isDragging || dragMode != InventoryDragMode.MoveWholeStack) return false;
        backpackView.BindPlayerQuickSlot(slotIndex, draggingInstanceId);
        dragHandledExternally = true;
        return true;
    }
}
}
