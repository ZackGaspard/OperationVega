using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Class will hold information about the craftable item.
public class ResourceItem
{
    public string rItem;
    public string rType;

    public override bool Equals(object other)
    {
        if (other.GetType() != typeof(ResourceItem)) return false;
        if (((ResourceItem)other).rItem == this.rItem && ((ResourceItem)other).rType == this.rType)
        {
            return true;
        }
        return false;

    }
}

//Class will determine if the item is craftable.
public class Craftable
{
    //A list that will store the resource input items
    public List<ResourceItem> InputRes = new List<ResourceItem>();

    //A list that will store the resource output items.
    public List<ResourceItem> Output = new List<ResourceItem>();

    public bool NeededItems;
}


