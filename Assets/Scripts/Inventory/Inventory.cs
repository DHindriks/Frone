using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackItem
{
    public Item item;
    public int amount;
    public StackItem(Item itemToAdd, int Stackamount)
    {
        item = itemToAdd;
        amount = Stackamount;
    }
}

public class Inventory : MonoBehaviour {

    public List<StackItem> ItemList;

	// Use this for initialization
	void Awake () {
        ItemList = new List<StackItem>();
	}

    public void AddItem(Item item, int amount)
    {
        foreach(StackItem stack in ItemList)
        {
            if (stack.item == item)
            {
                Debug.Log("Player has collected " + getItemData(item.name).item.name + " before, player picked up " + amount +" and already has " + stack.amount + " which makes " + (amount + stack.amount));
                stack.amount += amount;
                return;
            }
        }
        ItemList.Add(new StackItem(item, amount));

        Debug.Log("Added an item called " + item.name + ", " + amount + " have been added");
        return;
    }

    public StackItem getItemData(string name)
    {
        foreach (StackItem item in ItemList)
        {
            if (item.item.name == name)
            {
                return item;
            }
        }
        Debug.LogWarning("Couldn't find specified item, returning null.");
        return null;
    }

    public void TransferInventory(Inventory Target)
    {
        foreach (StackItem stack in ItemList)
        {
            Target.AddItem(stack.item, stack.amount);
        }
        ItemList.Clear();
    }

    public void SaveInventory()
    {
        SaveSystem.SaveInventory(this);
    }

    public void LoadInventoryData()
    {
        InventoryData data = SaveSystem.LoadInventory();
        if (data != null)
        {
            for(int i = 0; i < data.ItemList.Length; i++)
            {
                AddItem(ItemDataBase.Instance.GetItem(data.ItemList[i]), data.AmountList[i]);
                Debug.Log("Loaded " + data.AmountList[i] + data.ItemList[i] + " Into inventory");
            }
        }

    }
}
