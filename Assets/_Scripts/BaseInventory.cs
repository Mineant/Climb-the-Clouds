using System.Collections;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using UnityEngine;

public class BaseInventory : Inventory
{
    [Header("Mine")]
    public List<InventoryItemType> AcceptedItemTypes;

    public bool IsItemTypeAccepted(InventoryItem item)
    {
        if (item == null) return false;
        if (item is BaseInventoryItem baseInventoryItem && !AcceptedItemTypes.Contains(baseInventoryItem.ItemType)) return false;
        return true;
    }

    public override bool AddItem(InventoryItem itemToAdd, int quantity)
    {
        if (!IsItemTypeAccepted(itemToAdd)) return false;
        return base.AddItem(itemToAdd, quantity);
    }

    public override bool AddItemAt(InventoryItem itemToAdd, int quantity, int destinationIndex)
    {
        if (!IsItemTypeAccepted(itemToAdd)) return false;
        return base.AddItemAt(itemToAdd, quantity, destinationIndex);
    }

    protected override bool AddItemToArray(InventoryItem itemToAdd, int quantity)
    {
        if (!IsItemTypeAccepted(itemToAdd)) return false;
        return base.AddItemToArray(itemToAdd, quantity);
    }
}
