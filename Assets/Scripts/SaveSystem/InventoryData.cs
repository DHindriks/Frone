using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryData
{
    public string[] ItemList;
    public int[] AmountList;
    public InventoryData(Inventory inv)
    {
        ItemList = new string[inv.ItemList.Count];
        AmountList = new int[inv.ItemList.Count];
        for (int i = 0; i < inv.ItemList.Count; i++)
        {
            ItemList[i] = inv.ItemList[i].item.name;
            AmountList[i] = inv.ItemList[i].amount;
        }
    }
}
