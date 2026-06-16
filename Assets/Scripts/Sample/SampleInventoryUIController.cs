using System;
using JZQ.InventorySystem.Runtime.Core;
using JZQ.InventorySystem.Runtime.Inventory.Backpack.UI;
using JZQ.InventorySystem.Runtime.Inventory.QuickBar.UI;
using UnityEngine;
using InventoryRuntimeSystem = JZQ.InventorySystem.Runtime.Core.InventorySystem;

namespace JZQ.InventorySystem.Sample
{
public class SampleInventoryUIController : MonoBehaviour
{
    [SerializeField] private InventorySystemBootstrap bootstrap;
    [SerializeField] private BackpackPanel backpackPanel;
    [SerializeField] private QuickBarPanel quickBarPanel;
    
    private SampleInventoryInput inventoryInput;
    private void Awake()
    {
        

        backpackPanel.CloseRequested += HandleBackpackCloseRequested;
        inventoryInput = GetComponent<SampleInventoryInput>();
    }

    private void Start()
    {
        backpackPanel.Initialize(
            bootstrap.Config.BackpackLayoutConfig,
            bootstrap.Config.InventoryViewConfig);

        quickBarPanel.Initialize(
            bootstrap.Config.QuickBarConfig,
            bootstrap.Config.InventoryViewConfig);
        quickBarPanel.Show();
        backpackPanel.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        backpackPanel.CloseRequested -= HandleBackpackCloseRequested;
    }

    private void HandleBackpackCloseRequested()
    {
        CloseBackpack();
    }

    public void CloseBackpack()
    {
        backpackPanel.Hide();
        backpackPanel.gameObject.SetActive(false);
    }

    public void OpenBackpack()
    {
        backpackPanel.gameObject.SetActive(true);
        backpackPanel.Show();
    }
    
    private void Update()
    {
        HandleNumberInput();
        HandleScrollInput();
        HandleRotation();
        HandleBackpackShowHide();
    }

    private void HandleNumberInput()
    {
        int inputRead = inventoryInput.SlotSelection;
        if (inputRead is < 0 or > 9) return;
        
        InventoryRuntimeSystem.Current.SetSelectedQuickSlot(inputRead);
    }

    private void HandleScrollInput()
    {
        float inputRead = inventoryInput.PrevNext;
        if (inputRead == 0) return;
        if (inputRead > 0)
        {
            InventoryRuntimeSystem.Current.SelectPreviousQuickSlot();
        }
        else
        {
            InventoryRuntimeSystem.Current.SelectNextQuickSlot();
        }
    }

    private void HandleRotation()
    {
        if (!inventoryInput.RotatePressed) return;
        if (!backpackPanel.IsActive) return;
        backpackPanel.RotateDrag();

    }

    private void HandleBackpackShowHide()
    {
        if (inventoryInput.BackpackPressed)
        {
            if (backpackPanel.IsActive)
            {
                CloseBackpack();
            }
            else
            {
                OpenBackpack();
            }
        }
    }
}
}
