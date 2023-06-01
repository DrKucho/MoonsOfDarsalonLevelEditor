using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemGroup : MonoBehaviour
{
    [NonSerialized] public List<Item> items = new List<Item>();

    private bool oneItemHasBeenPickedup;
    private bool failedToGetAllItemsAtOnce;
    private bool allitemsPickedupAtOnce;
    
    public static List<ItemGroup> instances = new List<ItemGroup>();

    void OnEnable()
    {
        instances.Add(this);
    }

    private void OnDisable()
    {
        instances.Remove(this);
    }

    /*public bool AllPickedUp()
    {
        foreach (Item i in items)
        {
            if (i.isOutOfStore)
                return false;
        }
        return true;
    }
    */

    public void OnPlayerTouchGround()
    {
        if (oneItemHasBeenPickedup) //si no se ha pillado ninguno no se empieza a contabilzar
        {
            foreach (Item i in items)//itero todos mis items
            {
                if (i.isOutOfStore) //esta fuera
                {
                    failedToGetAllItemsAtOnce = true;//con que solo uno este fuera ya mal
                    break;
                }
            }

            if (!failedToGetAllItemsAtOnce) // se ha pillado al menos uno y si no ha dada fail es que todos se han pillado
                allitemsPickedupAtOnce = true;
        }
    }

    public void OnItemPickup(Item i)
    {
        oneItemHasBeenPickedup = true;
    }
    public static bool AllItemsPickedupAtOnce()
    {
        if (instances != null && instances.Count > 0)
        {
            foreach (ItemGroup ig in instances)
            {
                if (!ig.allitemsPickedupAtOnce)
                    return false;
            }
            return true;
        }
        return false;
    }
}
