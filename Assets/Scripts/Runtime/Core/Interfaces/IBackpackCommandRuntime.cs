using JZQ.InventorySystem.Runtime.Data;

namespace JZQ.InventorySystem.Runtime.Core
{
    /// <summary>
    /// Defines write operations for advanced backpack interactions.
    /// </summary>
    public interface IBackpackCommandRuntime
    {
        /// <summary>
        /// Attempts to drop a number of items from the specified stack.
        /// </summary>
        /// <param name="instanceId">The source item instance identifier.</param>
        /// <param name="count">The number of items to drop.</param>
        /// <param name="droppedItem">The dropped item instance when the operation succeeds.</param>
        /// <returns><c>true</c> if the items were dropped; otherwise, <c>false</c>.</returns>
        bool TryDropItem(string instanceId, int count, out ItemInstance droppedItem);

        /// <summary>
        /// Attempts to split a number of items from the specified stack.
        /// </summary>
        /// <param name="instanceId">The source item instance identifier.</param>
        /// <param name="splitCount">The number of items to split.</param>
        /// <param name="newStack">The newly created stack when the operation succeeds.</param>
        /// <returns><c>true</c> if the stack was split; otherwise, <c>false</c>.</returns>
        bool TrySplitStack(string instanceId, int splitCount, out ItemInstance newStack);

        /// <summary>
        /// Attempts to split a stack and place the split result at the target grid position.
        /// </summary>
        /// <param name="sourceInstanceId">The source item instance identifier.</param>
        /// <param name="splitCount">The number of items to split.</param>
        /// <param name="x">The target grid x coordinate.</param>
        /// <param name="y">The target grid y coordinate.</param>
        /// <param name="rotated">Whether the placed split stack is rotated.</param>
        /// <returns><c>true</c> if the split stack was placed; otherwise, <c>false</c>.</returns>
        bool TrySplitPlaceItem(string sourceInstanceId, int splitCount, int x, int y, bool rotated);

        /// <summary>
        /// Attempts to merge one stack into another stack.
        /// </summary>
        /// <param name="sourceInstanceId">The source item instance identifier.</param>
        /// <param name="targetInstanceId">The target item instance identifier.</param>
        /// <returns><c>true</c> if the merge succeeded; otherwise, <c>false</c>.</returns>
        bool TryMergeItems(string sourceInstanceId, string targetInstanceId);

        /// <summary>
        /// Attempts to split a partial stack and merge it into another stack.
        /// </summary>
        /// <param name="sourceInstanceId">The source item instance identifier.</param>
        /// <param name="splitCount">The number of items to split and merge.</param>
        /// <param name="targetInstanceId">The target item instance identifier.</param>
        /// <returns><c>true</c> if the split merge succeeded; otherwise, <c>false</c>.</returns>
        bool TryMergeSplitItems(string sourceInstanceId, int splitCount, string targetInstanceId);
    }
}
