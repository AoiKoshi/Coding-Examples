using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] [Min(1)] private int maxItemCapacity;
    [Tooltip("1 - Items, 2 - Weapons, 3 - Restoratives")]
    public List<Item> items = new List<Item>();

    public virtual bool AddToInventory(Item item)
    {
        bool owned = false;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].properties.type == item.properties.type)
            {
                if (items[i].GetID() == item.GetID())
                {
                    if (items[i].properties.doesStack)
                    {
                        items[i].properties.quantity += item.properties.quantity;
                        if (items[i].properties.quantity > 99)
                        {
                            items[i].properties.quantity = 99;
                        }
                        owned = true;
                        return true;
                    }
                    break;
                }
            }
        }

        if (!owned && items.Count < maxItemCapacity)
        {
            items.Add(item);
            return true;
        }
        return false;
    }

    public void RemoveFromInventory(Item item)
    {
        if (item.properties.quantity > 1)
        {
            item.properties.quantity--;
        }
        else if (item.properties.quantity == 1)
        {
            items.Remove(item);
        }
    }
}
