using JZQ.InventorySystem.Runtime.Core;
using JZQ.InventorySystem.Runtime.Inventory.QuickBar;
using JZQ.InventorySystem.Runtime.UI;
using UnityEngine;
using InventoryRuntimeSystem = JZQ.InventorySystem.Runtime.Core.InventorySystem;

namespace JZQ.InventorySystem.Runtime.Inventory.QuickBar.UI
{
    /// <summary>
    /// Controls quick bar panel lifecycle and refresh behavior.
    /// </summary>
    public class QuickBarPanel : InventoryPanelBase
    {
        [SerializeField] private QuickBarGridView quickBarGridView;
        private bool isInitialized;
        private IInventoryEventSource events;

        /// <summary>
        /// Initializes the quick bar panel and its grid view.
        /// </summary>
        /// <param name="quickBarConfig">The quick bar configuration.</param>
        /// <param name="viewConfig">The shared inventory view configuration.</param>
        public void Initialize(QuickBarConfig quickBarConfig, InventoryViewConfig viewConfig)
        {
            if (isInitialized) return;
            events = InventoryRuntimeSystem.Events;
            quickBarGridView.InitializeIfNeed(quickBarConfig, viewConfig);
            isInitialized = true;
        }

        /// <summary>
        /// Shows the quick bar panel and starts listening to runtime updates.
        /// </summary>
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

        /// <summary>
        /// Hides the quick bar panel and stops listening to runtime updates.
        /// </summary>
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
