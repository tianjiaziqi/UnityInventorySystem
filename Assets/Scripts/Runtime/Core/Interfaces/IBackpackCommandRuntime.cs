using JZQ.InventorySystem.Runtime.Data;

namespace JZQ.InventorySystem.Runtime.Core
{
    public interface IBackpackCommandRuntime
    {
        bool TryDropItem(string instanceId, int count, out ItemInstance droppedItem);
        bool TrySplitStack(string instanceId, int splitCount, out ItemInstance newStack);
        bool TrySplitPlaceItem(string sourceInstanceId, int splitCount, int x, int y, bool rotated);
        bool TryMergeItems(string sourceInstanceId, string targetInstanceId);
        bool TryMergeSplitItems(string sourceInstanceId, int splitCount, string targetInstanceId);
    }
}
