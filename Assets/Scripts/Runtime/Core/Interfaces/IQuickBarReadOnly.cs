using JZQ.InventorySystem.Runtime.Data;
using UnityEngine;

namespace JZQ.InventorySystem.Runtime.Core
{
    public interface IQuickBarReadOnly
    {
        int GetQuickBarSelectedIndex();
        ItemInstance[] GetQuickBarItems();
        ItemInstance GetSelectedQuickBarItem();
    }
}

