using System.Collections;
using System.Collections.Generic;
using JZQ.InventorySystem.Runtime.Core;
using JZQ.InventorySystem.Runtime.Inventory.QuickBar;
using UnityEngine;
using UnityEngine.UI;
using InventoryRuntimeSystem = JZQ.InventorySystem.Runtime.Core.InventorySystem;

namespace JZQ.InventorySystem.Runtime.Inventory.QuickBar.UI
{
public class QuickBarGridView : MonoBehaviour
{
    [SerializeField] private QuickBarSlotView slotPrefab;
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    private List<QuickBarSlotView> slots;
    private QuickBarConfig dataConfig;
    private InventoryViewConfig viewConfig;
    private int selectedIndex;
    private bool initialized;
    
    public bool Initialized => initialized;

    public void InitializeIfNeed(QuickBarConfig dataConfig, InventoryViewConfig viewConfig)
    {
        if (initialized) return;
        Initialize(dataConfig, viewConfig);
    }

    private void Initialize(QuickBarConfig dataConfig, InventoryViewConfig viewConfig)
    {
        this.dataConfig = dataConfig;
        this.viewConfig = viewConfig;
        slots = new List<QuickBarSlotView>();
        selectedIndex = InventoryRuntimeSystem.Current.GetQuickBarSelectedIndex();
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
        var instances = InventoryRuntimeSystem.Current.GetQuickBarItems();
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

    public void UpdateSelection()
    {
        int index = InventoryRuntimeSystem.Current.GetQuickBarSelectedIndex();
        if (index != selectedIndex)
        {
            SelectSlot(index);
        }
        
    }

    public void SelectSlot(int index)
    {
        slots[selectedIndex].SetSelection(false);
        selectedIndex = index;
        slots[selectedIndex].SetSelection(true);
    }
    
    public void RefreshAll()
    {
        UpdateSlots();
        UpdateSelection();
    }
}
}
