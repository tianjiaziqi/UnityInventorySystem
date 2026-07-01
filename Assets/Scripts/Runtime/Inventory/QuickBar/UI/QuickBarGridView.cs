using System.Collections.Generic;
using JZQ.InventorySystem.Runtime.Core;
using JZQ.InventorySystem.Runtime.Inventory.QuickBar;
using UnityEngine;
using InventoryRuntimeSystem = JZQ.InventorySystem.Runtime.Core.InventorySystem;

namespace JZQ.InventorySystem.Runtime.Inventory.QuickBar.UI
{
    /// <summary>
    /// Builds and refreshes the quick bar slot views.
    /// </summary>
    public class QuickBarGridView : MonoBehaviour
    {
        [SerializeField] private QuickBarSlotView slotPrefab;
        private List<QuickBarSlotView> slots;
        private QuickBarConfig dataConfig;
        private InventoryViewConfig viewConfig;
        private int selectedIndex;
        private bool initialized;
        private IQuickBarReadOnly quickBarReadOnly;

        /// <summary>
        /// Gets whether the quick bar grid view has been initialized.
        /// </summary>
        public bool Initialized => initialized;

        /// <summary>
        /// Initializes the quick bar grid view when needed.
        /// </summary>
        /// <param name="dataConfig">The quick bar configuration.</param>
        /// <param name="viewConfig">The shared inventory view configuration.</param>
        public void InitializeIfNeed(QuickBarConfig dataConfig, InventoryViewConfig viewConfig)
        {
            if (initialized) return;
            Initialize(dataConfig, viewConfig);
        }

    private void Initialize(QuickBarConfig dataConfig, InventoryViewConfig viewConfig)
    {
        quickBarReadOnly = InventoryRuntimeSystem.QuickBarReadOnly;
        this.dataConfig = dataConfig;
        this.viewConfig = viewConfig;
        slots = new List<QuickBarSlotView>();
        selectedIndex = quickBarReadOnly.GetQuickBarSelectedIndex();
        CreateSlots();
        UpdateSlots();
        initialized = true;
    }

    private void CreateSlots()
    {
        for (int i = 0; i < dataConfig.slotCount; i++)
        {
            var obj = Instantiate(slotPrefab, transform);
            var slot = obj.GetComponent<QuickBarSlotView>();
            slot.Init(i, viewConfig.QuickBarDefaultIcon, viewConfig.QuickBarSelectedIcon, i==selectedIndex);
            slots.Add(slot);
        }
    }

    private void UpdateSlots()
    {
        var instances = quickBarReadOnly.GetQuickBarItems();
        for (int i = 0; i < slots.Count; i++)
        {
            if (instances[i] == null)
            {
                slots[i].SetItem(null);
            }
            else
            {
                slots[i].SetItem(instances[i].Definition.Icon);
                slots[i].SetCount(instances[i].StackCount);
            }
        }
    }

        /// <summary>
        /// Updates the selected slot highlight from runtime state.
        /// </summary>
        public void UpdateSelection()
        {
            int index = quickBarReadOnly.GetQuickBarSelectedIndex();
            if (index != selectedIndex)
            {
                SelectSlot(index);
            }
        }

        /// <summary>
        /// Selects the specified quick bar slot view.
        /// </summary>
        /// <param name="index">The quick bar slot index to highlight.</param>
        public void SelectSlot(int index)
        {
            slots[selectedIndex].SetSelection(false);
            selectedIndex = index;
            slots[selectedIndex].SetSelection(true);
        }

        /// <summary>
        /// Refreshes quick bar slot content and selection state.
        /// </summary>
        public void RefreshAll()
        {
            UpdateSlots();
            UpdateSelection();
        }
    }
}
