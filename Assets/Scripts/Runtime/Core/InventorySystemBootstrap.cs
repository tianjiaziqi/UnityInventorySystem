using JZQ.InventorySystem.Runtime.Inventory.Internal;
using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Core
{
    /// <summary>
    /// Creates and registers the runtime inventory services for a scene.
    /// </summary>
    public class InventorySystemBootstrap : MonoBehaviour
    {
        [SerializeField] private InventorySystemConfig config;

        /// <summary>
        /// Gets the inventory system configuration used by this bootstrapper.
        /// </summary>
        public InventorySystemConfig Config => config;

        /// <summary>
        /// Gets the runtime inventory manager created by this bootstrapper.
        /// </summary>
        public InventoryManager Manager { get; private set; }

        private void Awake()
        {
            SetInterfaces();
        }

        private void OnDestroy()
        {
            InventorySystem.ClearInterfaces(Manager);
        }

        /// <summary>
        /// Creates the runtime manager and registers all exposed service interfaces.
        /// </summary>
        private void SetInterfaces()
        {
            Manager = new InventoryManager(config.BackpackLayoutConfig, config.QuickBarConfig);
            InventorySystem.SetCurrent(Manager);
            InventorySystem.SetEvents(Manager);
            InventorySystem.SetBackpackReadOnly(Manager);
            InventorySystem.SetQuickBarReadOnly(Manager);
            InventorySystem.SetBackpackViewRuntime(Manager);
            InventorySystem.SetBackpackCommandRuntime(Manager);
        }
    }
}
