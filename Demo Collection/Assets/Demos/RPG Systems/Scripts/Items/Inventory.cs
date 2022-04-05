using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();

    public void AddToInventory(Item item)
    {
        bool owned = false;
        
        for (int i = 0; i < items.Count; i++)
        {
            if(items[i].properties.type == item.properties.type)
            {
                if (items[i].GetID() == item.GetID())
                {
                    if(items[i].properties.type != 2 && items[i].properties.type != 3 && items[i].properties.type != 4)
                    {
                        items[i].properties.quantity += item.properties.quantity;
                        if (items[i].properties.quantity > 99)
                        {
                            items[i].properties.quantity = 99;
                        }
                        owned = true;
                        break;
                    }
                    break;
                }
            }
        }

        if(!owned)
        {
            items.Add(item);
        }
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
