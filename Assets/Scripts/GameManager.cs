using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DiscordPresence;

public class GameManager : MonoBehaviour
{
    List<Faction> Factions = new List<Faction>();

    public static GameManager Instance = null;

    public Color CurrentPrimary = Color.white;
    public Color CurrentSecondary = Color.white;
    public Color CurrentDetails = Color.white;
    public Color CurrentEnergy = Color.white;

    [Space(20)]
    //saved vars
    string ModelName;

    [HideInInspector]
    public Camerascript cam;

    [HideInInspector]
    public GameObject player; //player
    GameObject playerskin;


    //managers
    [HideInInspector]
    public Spawnmanager SpawnManager;

    [HideInInspector]
    public ObjectiveManager ObjectiveManager;

    [HideInInspector]
    public ManageFloor FloorManager;

    [HideInInspector]
    public Inventory MainInventory;

    //initialization
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        SetFactions();
        DontDestroyOnLoad(this.gameObject);
        MainInventory = GetComponent<Inventory>();
        MainInventory.Invoke("LoadInventoryData", 0.1f);
    }


    public void UnlockDrone(string DroneName)
    {
        //PlayerPrefs.SetInt(DroneName + "Unlocked", 1);
        if (Resources.Load("Drones/" + DroneName) != null)
        {
            GameObject UnlockedDrone = Resources.Load("Drones/" + DroneName) as GameObject;
            DroneSettings settings = UnlockedDrone.GetComponent<DroneSettings>();
            settings.LoadDroneData();
            settings.UnlockState = 1;
            settings.SaveDroneData();
            Debug.Log("Unlocked " + DroneName);
        }else
        {
            Debug.LogError("Drone not found");
        }
    }

    public void AddReputation(string FactionName, int amount, bool AffectAlliances = true)
    {
        Faction FacToAddTo = new Faction(); 

        //gets faction struct to get the factions alliances
        foreach(Faction faction in Factions)
        {
            if (faction.Name == FactionName)
            {
                FacToAddTo = faction;
                break;
            }
        }

        //if (FacToAddTo == null)
        //{
        //    Debug.LogError("");
        //} 

        //adds reputation to supporting faction, if possible
        int Reputation = PlayerPrefs.GetInt(FacToAddTo.Name + "Reputation", 0);
        int RepToAdd = amount;
        if (Reputation + RepToAdd >= 1000)
        {
            //player reached max rep
            PlayerPrefs.SetInt(FacToAddTo.Name + "Reputation", 1000);
            RepToAdd = 0;
        }
        PlayerPrefs.SetInt(FactionName + "Reputation", Reputation + RepToAdd);


        //adds/ subtracts reputation to alliances, if possible
        if (AffectAlliances == true)
        {
            //adds 50% to ally
            RepToAdd = amount / 2;
            Reputation = PlayerPrefs.GetInt(FacToAddTo.Ally + "Reputation");
            if (Reputation + RepToAdd >= 1000)
            {
                //max rep
                PlayerPrefs.SetInt(FacToAddTo.Ally + "Reputation", 1000);
                RepToAdd = 0;
            }
            PlayerPrefs.SetInt(FacToAddTo.Ally + "Reputation", Reputation + RepToAdd);


            //Subtracts 50% from enemy
            RepToAdd = amount / 2;
            Reputation = PlayerPrefs.GetInt(FacToAddTo.Enemy + ("Reputation"));
            if (Reputation + RepToAdd <= -1000)
            {
                //min rep
                PlayerPrefs.SetInt(FacToAddTo.Enemy + "Reputation", -1000);
                RepToAdd = 0;
            }
            Debug.Log(-RepToAdd + " " + FacToAddTo.Enemy);
            PlayerPrefs.SetInt(FacToAddTo.Enemy + "Reputation", Reputation + -RepToAdd);
        }

    }

    void Update()
    {
        //if (SceneManager.GetActiveScene().name == "Ingame")
        //{
        //    //Debug.Log("player in Ingame");
        //}

        //else if (SceneManager.GetActiveScene().name == "Customize")
        //{
        //    //Debug.Log("player in Customize");
        //}

        //else if (SceneManager.GetActiveScene().name == "MainMenu")
        //{
        //    //Debug.Log("player in MainMenu");
        //}


    }

    void SetFactions()
    {
        Faction Null = new Faction
        {
            Name = "Null",
            Emblem = Resources.Load("Emblems/Null") as Sprite,
            Ally = "Spy",
            Enemy = "TestFac3"
        };
        Factions.Add(Null);

        Faction Spy = new Faction
        {
            Name = "Spy",
            Emblem = Resources.Load("Emblems/Spy") as Sprite,
            Ally = "Null",
            Enemy = "TestFac3"
        };
        Factions.Add(Spy);

        Faction TestFac3 = new Faction
        {
            Name = "TestFac3",
            Emblem = Resources.Load("Emblems/TestFac3") as Sprite,
            Ally = "Null",
            Enemy = "Spy"
        };
        Factions.Add(TestFac3);
    }

    public void ShrinkDespawn(GameObject gameObject, float delay = 0)
    {
        StartCoroutine(SDespawn(gameObject, delay));
    }

    IEnumerator SDespawn(GameObject gameObject, float delay)
    {
        float Rate = 2;
        float i = 0;
        yield return new WaitForSeconds(delay);
        while (i < 1)
        {
            i += Time.deltaTime * Rate;
            gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, Vector3.zero, i);
            yield return 0;
        }
        Destroy(gameObject);
    }

    
    //TODO: Stop using playerprefs, adjust this to use general save file
    public void Setplayerskin(string Model = "") //if no arg is given, loads the skin that's currently saved in playerprefs. Else load the model with the same name as arg.
    {

        if (Model == "")
        {
            ModelName = PlayerPrefs.GetString("Model");
            PresenceManager.UpdatePresence(smallText: ModelName);
        } else {
            ModelName = Model;
        }


        player = GameObject.FindGameObjectWithTag("Player");
        foreach(Transform child in player.transform.GetChild(0))
        {
            Destroy(child.gameObject);
        } 

        playerskin = Instantiate(Resources.Load("Drones/" + ModelName)) as GameObject;
        playerskin.transform.SetParent(player.transform.GetChild(0).transform);
        playerskin.transform.localPosition = new Vector3(0, 0, 0);
        playerskin.transform.rotation = playerskin.transform.parent.rotation;
        SetRefProbe(playerskin);
    }

    void SetRefProbe(GameObject obj)
    {
        if (obj.GetComponent<MeshRenderer>()) {
            obj.GetComponent<MeshRenderer>().probeAnchor = player.transform.GetChild(3);
        }
        foreach (Transform child in obj.transform)
        {
            SetRefProbe(child.gameObject);
        }
    }
}

struct Faction
{
    public string Name;
    public Sprite Emblem;
    public string Ally;
    public string Enemy;
}