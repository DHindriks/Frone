using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string name;
    [TextArea(5, 15)]
    public string discription;
    public Sprite icon;
    public GameObject Model;
    [Tooltip("Particle that will emit from this item while in lootable form")]
    public ParticleSystem GroundParticles;

    public Item(int id, string name, string discription, Sprite icon, GameObject model, ParticleSystem groundparticles)
    {
        this.name = name;
        this.discription = discription;
        this.icon = icon;
        this.Model = model;
        this.GroundParticles = groundparticles;
    }

}

public class ItemDataBase : MonoBehaviour {

    public static ItemDataBase Instance = null;

    public GameObject ItemBasePrefab;

    [SerializeField]
    List<Item> AllItems;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public Item GetItem(string name)
    {
        foreach (Item item in AllItems)
        {
            if (item.name == name)
            {
                return item;
            }
        }
        Debug.LogError("Item with the name " + name + " could not be found.");
        return null;
    }
}
