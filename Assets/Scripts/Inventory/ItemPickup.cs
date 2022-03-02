using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour {
    Item ThisItem;
    int Amount;

    private void Start()
    {
        SetItem(ItemDataBase.Instance.GetItem("Aluminium Alloy"));
        transform.parent = null;
    }

    public void SetItem(Item item, int amount = 1)
    {
        ThisItem = item;
        Amount = amount;
        GameObject ItemObj = Instantiate(item.Model, transform);
        GameObject Particles = Instantiate(item.GroundParticles.gameObject, transform);
    }

    void Update()
    {
        if(transform.position.y <= -10)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Inventory>())
        {
            //add item to temp inventory
            other.GetComponent<Inventory>().AddItem(ThisItem, Amount);
            Destroy(GetComponent<Collider>());
            GameManager.Instance.ShrinkDespawn(gameObject, 0.1f);
        }
    }
}
