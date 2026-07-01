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
    /// <summary>
    /// Builds the backpack grid UI and coordinates drag-based item interactions.
    /// </summary>
    public class InventoryGridView : MonoBehaviour
    {
    private enum InventoryDragMode
    {
        None,
        MoveWholeStack,
        SplitStack
    }

    [SerializeField] private RectTransform slotLayer;
    [SerializeField] private RectTransform itemLayer;
    [SerializeField] private RectTransform previewLayer;
    [SerializeField] private SlotCellView slotPrefab;
    [SerializeField] private InventoryItemView itemPrefab;
    private float cellSize = 64f;
    private Vector2 spacing = Vector2.zero;
    private SlotCellView[,] slotCells;
    private readonly Dictionary<string, InventoryItemView> itemViews = new();
    private bool initialized;

    [SerializeField] private GridLayoutGroup slotGridLayout;

    private BackpackLayoutConfig dataConfig;
    private InventoryViewConfig viewConfig;

    [SerializeField] private InventoryDragGhostView dragGhostPrefab;
    private InventoryDragGhostView currentGhost;
    private bool isDragging;
    private string draggingInstanceId;
    private PlacedItem draggingItem;

    private InventoryItemView draggingItemView;
    private InventoryDragMode dragMode;
    private ItemInstance splitPreviewItem;
    private int splitDragCount;
    private string currentMergeTargetId;

    private bool currentRotated;
    private Vector2Int hoverGridPosition;
    private bool currentPlacementValid;
    private Vector2 currentPointerScreenPosition;
    private bool dragHandledExternally;

    private IBackpackViewRuntime backpackView;
    private IBackpackCommandRuntime backpackCommands;

    /// <summary>
    /// Initializes the grid view when needed.
    /// </summary>
    /// <param name="dataConfig">The backpack layout configuration.</param>
    /// <param name="viewConfig">The shared inventory view configuration.</param>
    public void InitializeIfNeeded(BackpackLayoutConfig dataConfig, InventoryViewConfig viewConfig)
    {
        if (!initialized) Initialize(dataConfig, viewConfig);
    }

    /// <summary>
    /// Builds the grid view using the provided runtime configuration.
    /// </summary>
    /// <param name="dataConfig">The backpack layout configuration.</param>
    /// <param name="viewConfig">The shared inventory view configuration.</param>
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
    /// Creates all slot cell views for the configured backpack dimensions.
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
    /// Refreshes slot state and item visuals.
    /// </summary>
    public void RefreshAll()
    {
        RefreshSlots();
        RefreshItems();
    }

    /// <summary>
    /// Refreshes the unlocked state of all grid cells.
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
    /// Refreshes item views to match the current backpack runtime state.
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
    /// Creates a new item view for the specified placed item.
    /// </summary>
    /// <param name="placedItem">The placed item to visualize.</param>
    /// <returns>The created item view.</returns>
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
    /// Converts grid coordinates to local UI coordinates.
    /// </summary>
    /// <param name="x">The grid x coordinate.</param>
    /// <param name="y">The grid y coordinate.</param>
    /// <returns>The corresponding local UI position.</returns>
    private Vector2 GridToLocalPosition(int x, int y)
    {
        float posX = x * (cellSize + spacing.x);
        float posY = -y * (cellSize + spacing.y);
        return new Vector2(posX, posY);
    }

    /// <summary>
    /// Calculates the UI size of an item from grid dimensions.
    /// </summary>
    /// <param name="width">The item width in cells.</param>
    /// <param name="height">The item height in cells.</param>
    /// <returns>The calculated UI size.</returns>
    private Vector2 GetItemSize(int width, int height)
    {
        float sizeX = width * cellSize + (width - 1) * spacing.x;
        float sizeY = height * cellSize + (height - 1) * spacing.y;
        return new Vector2(sizeX, sizeY);
    }

    /// <summary>
    /// Gets the slot cell view at the specified grid coordinate.
    /// </summary>
    /// <param name="x">The grid x coordinate.</param>
    /// <param name="y">The grid y coordinate.</param>
    /// <returns>The matching slot cell view.</returns>
    public SlotCellView GetSlotCell(int x, int y)
    {
        return slotCells[x, y];
    }

    /// <summary>
    /// Converts a screen position to a backpack grid coordinate.
    /// </summary>
    /// <param name="screenPosition">The screen position to convert.</param>
    /// <param name="x">The resolved grid x coordinate.</param>
    /// <param name="y">The resolved grid y coordinate.</param>
    /// <returns><c>true</c> if the position resolves to a valid grid cell; otherwise, <c>false</c>.</returns>
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
    /// Displays a placement preview over the specified grid area.
    /// </summary>
    /// <param name="x">The preview origin x coordinate.</param>
    /// <param name="y">The preview origin y coordinate.</param>
    /// <param name="width">The preview width in cells.</param>
    /// <param name="height">The preview height in cells.</param>
    /// <param name="valid">Whether the preview should use the valid state.</param>
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
    /// Clears all placement preview visuals.
    /// </summary>
    public void ClearPreview()
    {
        foreach (var cell in slotCells)
        {
            cell.ClearPreview();
        }
    }

    /// <summary>
    /// Begins dragging a full item stack.
    /// </summary>
    /// <param name="itemView">The item view being dragged.</param>
    /// <param name="eventData">The Unity pointer event data.</param>
    public void BeginItemDrag(InventoryItemView itemView, PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        BeginDrag(itemView, eventData, InventoryDragMode.MoveWholeStack, 0);
    }

    /// <summary>
    /// Begins dragging a split stack preview.
    /// </summary>
    /// <param name="itemView">The item view being dragged.</param>
    /// <param name="eventData">The Unity pointer event data.</param>
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

    /// <summary>
    /// Updates the active drag interaction.
    /// </summary>
    /// <param name="eventData">The Unity pointer event data.</param>
    public void UpdateItemDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        currentPointerScreenPosition = eventData.position;
        UpdateGhostPosition(eventData.position);
        UpdateDragPreview(eventData.position);
    }

    /// <summary>
    /// Ends the active drag interaction.
    /// </summary>
    /// <param name="eventData">The Unity pointer event data.</param>
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

    /// <summary>
    /// Cancels the current drag interaction and restores the UI state.
    /// </summary>
    public void CancelCurrentDrag()
    {
        ClearPreview();
        ClearCurrentDragState();
        RefreshItems();
    }

    /// <summary>
    /// Rotates the current drag preview when supported by the item.
    /// </summary>
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

    /// <summary>
    /// Attempts to bind the currently dragged item to a quick slot.
    /// </summary>
    /// <param name="slotIndex">The target quick slot index.</param>
    /// <returns><c>true</c> if the bind request was accepted; otherwise, <c>false</c>.</returns>
    public bool TryBindDraggedItemToQuickSlot(int slotIndex)
    {
        if (!isDragging || dragMode != InventoryDragMode.MoveWholeStack) return false;
        backpackView.BindPlayerQuickSlot(slotIndex, draggingInstanceId);
        dragHandledExternally = true;
        return true;
    }
}
}
