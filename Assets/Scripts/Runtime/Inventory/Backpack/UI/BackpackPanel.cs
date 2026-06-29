using System.Collections;
using JZQ.InventorySystem.Runtime.Core;
using JZQ.InventorySystem.Runtime.Inventory.Backpack;
using JZQ.InventorySystem.Runtime.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using InventoryRuntimeSystem = JZQ.InventorySystem.Runtime.Core.InventorySystem;

namespace JZQ.InventorySystem.Runtime.Inventory.Backpack.UI
{
public class BackpackPanel : InventoryPanelBase
{
    [SerializeField] InventoryGridView inventoryGridView;
    [SerializeField] private float blinkTime = 0.5f;
    private TMP_Text weightText;
    private Coroutine weightBlinkCoroutine;
    public UnityAction CloseRequested;
    private IBackpackReadOnly backpackSources;
    private IInventoryEventSource events;

    private bool isInitialized;

    private void Start()
    {
        GetUIComponent<Button>("CloseButton").onClick.AddListener(RequestClose);
        backpackSources = InventoryRuntimeSystem.BackpackReadOnly;
        events = InventoryRuntimeSystem.Events;
    }

    public void Initialize(BackpackLayoutConfig dataConfig, InventoryViewConfig viewConfig)
    {
        if (isInitialized) return;
        inventoryGridView.InitializeIfNeeded(dataConfig, viewConfig);
        weightText = GetUIComponent<TMP_Text>("WeightText");
        isInitialized = true;
    }

    public override void Show()
    {
        if (!isInitialized)
        {
            Debug.LogError("BackpackPanel must be initialized before show");
            return;
        }
        events.RegisterEvent(EInventoryEventType.InventoryUnlockChange, OnInventoryChanged);
        events.RegisterEvent(EInventoryEventType.InventoryChange, OnInventoryChanged);
        UpdateWeightState();
        inventoryGridView.RefreshAll();
    }

    public override void Hide()
    {
        events.UnregisterEvent(EInventoryEventType.InventoryUnlockChange, OnInventoryChanged);
        events.UnregisterEvent(EInventoryEventType.InventoryChange, OnInventoryChanged);
        if(weightBlinkCoroutine != null) 
        {
            StopCoroutine(weightBlinkCoroutine);
            weightBlinkCoroutine = null;
        }
    }

    public void OnInventoryChanged()
    {
        inventoryGridView.RefreshAll();
        UpdateWeightState();
    }

    private void UpdateWeightState()
    {
        float currentWeight = backpackSources.GetPlayerCurrentWeight();
        float maxWeight = backpackSources.GetPlayerMaxWeight();
        float ratio = backpackSources.GetPlayerWeightRatio();
        switch (ratio)
        {
            case < 0.95f:
                if (weightBlinkCoroutine != null)
                {
                    StopCoroutine(weightBlinkCoroutine);
                    weightBlinkCoroutine = null;
                }
                weightText.color = Color.black;
                break;
            case <= 1f:
                if (weightBlinkCoroutine != null)
                {
                    StopCoroutine(weightBlinkCoroutine);
                    weightBlinkCoroutine = null;
                }
                weightText.color = Color.yellow;
                break;
            case <= 1.5f:
                if (weightBlinkCoroutine != null) break;
                weightBlinkCoroutine =  StartCoroutine(WeightBlink());
                break;
            case > 1.5f:
                if (weightBlinkCoroutine != null)
                {
                    StopCoroutine(weightBlinkCoroutine);
                    weightBlinkCoroutine = null;
                }
                weightText.color = Color.red;
                break;
        }
        weightText.text = $"{currentWeight}/{maxWeight}";
    }
    

    public void RotateDrag()
    {
        inventoryGridView.RotateCurrentDrag();
    }

    private IEnumerator WeightBlink()
    {
        while (true)
        {
            weightText.color = Color.red;
            yield return new WaitForSeconds(blinkTime);
            weightText.color = Color.black;
            yield return new WaitForSeconds(blinkTime);
        }
    }

    private void RequestClose()
    {
        CloseRequested?.Invoke();
    }
}
}
