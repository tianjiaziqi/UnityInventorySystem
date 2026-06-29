using System.Collections.Generic;
using JZQ.InventorySystem.Runtime.Inventory.Backpack;

namespace JZQ.InventorySystem.Runtime.Core
{
    public interface IBackpackReadOnly
    {
        IReadOnlyList<PlacedItem> GetPlayerPlacedItems();
        bool IsPlayerGridUnlocked(int x, int y);
        float GetPlayerCurrentWeight();
        float GetPlayerMaxWeight();
        float GetPlayerWeightRatio();
        
    }
}
