using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Data
{
    [System.Serializable]
    public class ItemInstance
    {
        public string InstanceID;
        public ItemDefinition Definition;
        private int stackCount;
        public int Durability;

        public int StackCount
        {
            get => stackCount;
            set => stackCount = Mathf.Clamp(value, 1, Definition.MaxStack);
        }

        public ItemInstance(ItemDefinition def, string instanceID, int stackCount = 1)
        {
            Definition = def;
            InstanceID = instanceID;
            this.stackCount = Mathf.Clamp(stackCount, 1, Definition.MaxStack);
            Durability = 1;
        }
    }
}
