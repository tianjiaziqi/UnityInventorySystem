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

    public void Initialize(QuickBarConfig quickBarConfig, InventoryViewConfig viewConfig)
    {
        if (isInitialized) return;
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
        
        InventoryRuntimeSystem.Current.RegisterEvent(EInventoryEventType.QuickBarChanged, OnQuickBarChange);
        InventoryRuntimeSystem.Current.RegisterEvent(EInventoryEventType.QuickBarSelectionChanged, OnQuiBarSelectionChanged);
        InventoryRuntimeSystem.Current.RegisterEvent(EInventoryEventType.InventoryChange, OnInventoryChanged);
        quickBarGridView.RefreshAll();
    }

    public override void Hide()
    {
        InventoryRuntimeSystem.Current.UnregisterEvent(EInventoryEventType.QuickBarChanged, OnQuickBarChange);
        InventoryRuntimeSystem.Current.UnregisterEvent(EInventoryEventType.QuickBarSelectionChanged, OnQuiBarSelectionChanged);
        InventoryRuntimeSystem.Current.UnregisterEvent(EInventoryEventType.InventoryChange, OnInventoryChanged);
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
