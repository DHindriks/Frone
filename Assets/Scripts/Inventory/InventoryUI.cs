using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {
    [SerializeField]
    GameObject ItemBtnPrefab;

	// Use this for initialization
	void OnEnable () {

        foreach (Transform obj in transform)
        {
            Destroy(obj.gameObject);
        }

        if (GameManager.Instance != null)
        {
            foreach (StackItem ItemStack in GameManager.Instance.MainInventory.ItemList)
            {
                GameObject ItemBtn = Instantiate(ItemBtnPrefab, transform);
                ItemBtn.transform.GetChild(0).GetComponent<Image>().sprite = ItemStack.item.icon;
                ItemBtn.transform.GetChild(1).GetComponentInChildren<Text>().text = ItemStack.item.name + " " + ItemStack.amount;
            }
        }
	}
}
