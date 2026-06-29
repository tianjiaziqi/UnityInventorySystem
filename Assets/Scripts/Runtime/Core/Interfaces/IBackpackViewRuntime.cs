using System.Collections.Generic;
using JZQ.InventorySystem.Runtime.Inventory.Backpack;

namespace JZQ.InventorySystem.Runtime.Core
{
    public interface IBackpackViewRuntime
    {
        bool IsPlayerGridUnlocked(int x, int y);
        IReadOnlyList<PlacedItem> GetPlayerPlacedItems();
        bool CanMovePlayerItemTo(string instanceId, int x, int y, bool rotated);
        bool TryMoveItemFromPlayer(string instanceId, int x, int y, bool rotated);
        void BindPlayerQuickSlot(int slotIndex, string instanceId);
    }
}
