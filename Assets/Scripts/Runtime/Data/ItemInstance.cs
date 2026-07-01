using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Data
{
    /// <summary>
    /// Represents a runtime instance of an item definition.
    /// </summary>
    [System.Serializable]
    public class ItemInstance
    {
        /// <summary>
        /// Unique identifier of this runtime item instance.
        /// </summary>
        public string InstanceID;

        /// <summary>
        /// Shared item definition referenced by this instance.
        /// </summary>
        public ItemDefinition Definition;
        private int stackCount;

        /// <summary>
        /// Runtime durability value for this item instance.
        /// </summary>
        public int Durability;

        /// <summary>
        /// Gets or sets the current stack count, clamped to the valid range for the definition.
        /// </summary>
        public int StackCount
        {
            get => stackCount;
            set => stackCount = Mathf.Clamp(value, 1, Definition.MaxStack);
        }

        /// <summary>
        /// Creates a runtime item instance from the specified definition.
        /// </summary>
        /// <param name="def">The item definition.</param>
        /// <param name="instanceID">The unique runtime instance identifier.</param>
        /// <param name="stackCount">The initial stack count.</param>
        public ItemInstance(ItemDefinition def, string instanceID, int stackCount = 1)
        {
            Definition = def;
            InstanceID = instanceID;
            this.stackCount = Mathf.Clamp(stackCount, 1, Definition.MaxStack);
            Durability = 1;
        }
    }
}
