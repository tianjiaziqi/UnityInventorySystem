using JZQ.InventorySystem.Runtime.Inventory.Common;
using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Core
{
    public class InventorySystemBootstrap : MonoBehaviour
    {
        [SerializeField] private InventorySystemConfig config;

        public InventorySystemConfig Config => config;

        public InventoryManager Manager { get; private set; }

        private void Awake()
        {
            SetInterfaces();
            GetComponent<SampleGUI>().Initialize(config.ItemDatabase);
        }

        private void OnDestroy()
        {
            InventorySystem.ClearCurrent(Manager);
        }

        private void SetInterfaces()
        {
            Manager = new InventoryManager(config.BackpackLayoutConfig, config.QuickBarConfig);
            InventorySystem.SetCurrent(Manager);
            InventorySystem.SetEvents(Manager);
            InventorySystem.SetBackpackReadOnly(Manager);
            InventorySystem.SetQuickBarReadOnly(Manager);
            InventorySystem.SetBackpackViewRuntime(Manager);
        }
    }
}
