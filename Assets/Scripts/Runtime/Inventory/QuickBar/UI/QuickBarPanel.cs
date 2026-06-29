using System.Collections;
using System.Collections.Generic;
using JZQ.InventorySystem.Runtime.Core;
using JZQ.InventorySystem.Runtime.Inventory.QuickBar;
using JZQ.InventorySystem.Runtime.UI;
using UnityEngine;
using InventoryRuntimeSystem = JZQ.InventorySystem.Runtime.Core.InventorySystem;

namespace JZQ.InventorySystem.Runtime.Inventory.QuickBar.UI
{
public class QuickBarPanel : InventoryPanelBase
{
    
    [SerializeField] private QuickBarGridView quickBarGridView;
    
    private bool isInitialized;
    
    private IInventoryEventSource events;

    public void Initialize(QuickBarConfig quickBarConfig, InventoryViewConfig viewConfig)
    {
        if (isInitialized) return;
        events = InventoryRuntimeSystem.Events;
        quickBarGridView.InitializeIfNeed(quickBarConfig, viewConfig);
        isInitialized = true;
    }
    public override void Show()
    {
        if (!isInitialized)
        {
            Debug.LogError("QuickBarPanel must be initialized before show");
            return;
        }
        
        events.RegisterEvent(EInventoryEventType.QuickBarChanged, OnQuickBarChange);
        events.RegisterEvent(EInventoryEventType.QuickBarSelectionChanged, OnQuiBarSelectionChanged);
        events.RegisterEvent(EInventoryEventType.InventoryChange, OnInventoryChanged);
        quickBarGridView.RefreshAll();
    }

    public override void Hide()
    {
        events.UnregisterEvent(EInventoryEventType.QuickBarChanged, OnQuickBarChange);
        events.UnregisterEvent(EInventoryEventType.QuickBarSelectionChanged, OnQuiBarSelectionChanged);
        events.UnregisterEvent(EInventoryEventType.InventoryChange, OnInventoryChanged);
    }

    private void OnQuickBarChange()
    {
        quickBarGridView.RefreshAll();
    }

    private void OnQuiBarSelectionChanged()
    {
        quickBarGridView.UpdateSelection();
    }
    
    private void OnInventoryChanged()
    {
        quickBarGridView.RefreshAll();
    }
}
}
