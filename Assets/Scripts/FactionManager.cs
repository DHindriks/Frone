using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class FactionManager : MonoBehaviour {
    //faction Menu
    [SerializeField]
    GameObject FactionMenu;

    [SerializeField]
    Text FacTitle;

    [SerializeField]
    Text FacDescription;

    [SerializeField]
    GameObject RepBar;


    //Drone menu
    [SerializeField]
    GameObject DroneMenu;

    [SerializeField]
    Text DroneTitle;

    [SerializeField]
    Text DroneDescription;

    [SerializeField]
    Text DroneButtonText;


    [SerializeField]
    GameObject ShopPos;

    Dictionary<string, int> ShopItems = new Dictionary<string, int>();
    List<GameObject> ItemsInShop = new List<GameObject>();

    string SelectedFaction;
    GameObject SelectedPos;

    Vector3 CamPos;
    Quaternion CamRot;
    Vector3 DefaultPos;
    Quaternion DefaultRot;
    bool zoomed = false; //prevents player from clicking on another base when already zoomed in.
    bool Inshop = false; 

	// Use this for initialization
	void Start () {
        DefaultPos = this.gameObject.transform.position;
        CamPos = DefaultPos;
        DefaultRot = this.gameObject.transform.rotation;
        CamRot = DefaultRot;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, CamPos, 0.10f);
        this.gameObject.transform.rotation = Quaternion.Lerp(this.gameObject.transform.rotation, CamRot, 0.10f);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = this.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "Faction" && zoomed == false)
                {
                    CamPos = hit.transform.root.GetChild(0).position;
                    CamRot = hit.transform.root.GetChild(0).rotation;
                    SelectedPos = hit.transform.root.GetChild(0).gameObject;
                    SetText(hit.transform.root.gameObject.name);
                    FactionMenu.SetActive(true);
                    zoomed = true;
                }

                if (hit.collider.gameObject.tag == "Preview" && Inshop == true)
                {
                    CamPos = new Vector3(hit.transform.position.x, ShopPos.transform.position.y, ShopPos.transform.position.z);
                    DroneMenu.SetActive(true);
                    DroneTitle.text = hit.collider.gameObject.name.Replace("(Clone)", "");
                    DroneDescription.text = "How am I going to get this text?";

                    if (PlayerPrefs.GetInt(hit.collider.gameObject.name.Replace("(Clone)", "") + "Unlocked", 0) == 0)
                    {
                        DroneButtonText.text = ShopItems[hit.collider.gameObject.name.Replace("(Clone)", "")].ToString() + " " + SelectedFaction;
                        DroneButtonText.transform.parent.GetComponent<Button>().onClick.RemoveAllListeners();
                        DroneButtonText.transform.parent.GetComponent<Button>().interactable = true;
                        DroneButtonText.transform.parent.GetComponent<Button>().onClick.AddListener(delegate { BuyDrone(hit.collider.gameObject.name.Replace("(Clone)", "")); });
                    }else
                    {
                        DroneButtonText.transform.parent.GetComponent<Button>().interactable = false;
                        DroneButtonText.text = "Unlocked";
                    }
                }
            }
        }

	}

    void SetText(string FactionName)
    {
        SelectedFaction = FactionName;
        Setshop(FactionName);
        if (FactionName == "Null")
        {
            FacTitle.text = "Null";
            FacDescription.text = "Test Description for Faction: Null";
        }

        if (FactionName == "Spy")
        {
            FacTitle.text = "Idk";
            FacDescription.text = "Test Description For Idk, Testing description box length, I wonder if this whole story will fit since this box doesn't scale automatically and its width is a bit too low in my opinion, not sure if this is enjoyable to read.";
        }

        if (FactionName == "TestFac3")
        {
            FacTitle.text = "TestVac3";
            FacDescription.text = "Debug faction";
        }



        RepBar.transform.localScale = new Vector3((float)PlayerPrefs.GetInt(FactionName + "Reputation") / 1000, 1, 1);
            Debug.Log(PlayerPrefs.GetInt(FactionName + "Reputation"));
    }

    public void ToShop()
    {
        this.gameObject.transform.position = ShopPos.transform.position;
        this.gameObject.transform.rotation = ShopPos.transform.rotation;
        CamPos = ShopPos.transform.position;
        CamRot = ShopPos.transform.rotation;
        FactionMenu.SetActive(false);
        Inshop = true;
    }

    public void BuyDrone(string Drone)
    {
        GameObject Prompt = Instantiate(Resources.Load("UI/Prompt"), DroneMenu.transform) as GameObject;
        PromptScript promptScript = Prompt.GetComponent<PromptScript>();

        if (PlayerPrefs.GetInt(SelectedFaction + "Reputation", 0) < ShopItems[Drone])
        {
            promptScript.Message.text = "You don't have enough reputation to buy this drone.";
            Destroy(promptScript.CancelButton.gameObject);
            promptScript.ConfirmButton.GetComponentInChildren<Text>().text = "Ok";
        }else
        {
            promptScript.Message.text = "Do you want to buy " + Drone + " for " + (int)ShopItems[Drone] + " " + SelectedFaction + " reputation";
            PlayerPrefs.SetInt(SelectedFaction + "Reputation", PlayerPrefs.GetInt(SelectedFaction + "Reputation") - (int)ShopItems[Drone]);
            promptScript.ConfirmButton.onClick.AddListener(delegate { GameManager.Instance.UnlockDrone(Drone); });
        }

    }

    public void SupportFaction(bool Confirmed = false)
    {
        if (Confirmed == false)
        {

            if (PlayerPrefs.GetString("SupportingFaction", "None") == "None")
            {
                GameObject Prompt = Instantiate(Resources.Load("UI/Prompt"), FactionMenu.transform) as GameObject;
                Prompt.GetComponent<PromptScript>().Message.text = "Do you want to support " + SelectedFaction + "?";
                Prompt.GetComponent<PromptScript>().ConfirmButton.onClick.AddListener(delegate { SupportFaction(true); });
            }
            else if (SelectedFaction == PlayerPrefs.GetString("SupportingFaction"))
            {
                GameObject Prompt = Instantiate(Resources.Load("UI/Prompt"), FactionMenu.transform) as GameObject;
                Prompt.GetComponent<PromptScript>().Message.text = "You are already supporting " + PlayerPrefs.GetString("SupportingFaction", "None") + ". Do you want to stop supporting " + PlayerPrefs.GetString("SupportingFaction", "None");
                Prompt.GetComponent<PromptScript>().ConfirmButton.onClick.AddListener(delegate { StopSupport(); });
            }
            else
            {
                GameObject Prompt = Instantiate(Resources.Load("UI/Prompt"), FactionMenu.transform) as GameObject;
                Prompt.GetComponent<PromptScript>().Message.text = "You can only support one faction at a time, stop supporting " + PlayerPrefs.GetString("SupportingFaction", "None") + " and support " + SelectedFaction + "?";
                Prompt.GetComponent<PromptScript>().ConfirmButton.onClick.AddListener(delegate { SupportFaction(true); });
            }

        }
        else
        {
            PlayerPrefs.SetString("SupportingFaction", SelectedFaction);
            //playerprefs.setstring("enemyfaction", enemy);
            //playerprefs.setstring("allyfaction", ally);
        }


    }
    //adds drones to the shops with prices.
    //TODO: this could be a switch statement? YES IT CAN.
    void Setshop(string Faction)
    {
        if (Faction == "Null")
        {
            ShopItems.Add("Orb", 400);
            ShopItems.Add("Dropper", 800);
            ShopItems.Add("Version1", 500);
            ShopItems.Add("Cyberdrone", 500);
            ShopItems.Add("Slicer", 450);
            ShopItems.Add("Legend", 100);
            ShopItems.Add("RocketProto", 100);

        } else if (Faction == "Spy")
        {
            ShopItems.Add("Gunner", 350);
            ShopItems.Add("Interceptor", 30);
            ShopItems.Add("Juggernaut", 600);
        } else if (Faction == "TestFac3")
        {
            ShopItems.Add("Interceptor", 30);
        }

        foreach(string Value in ShopItems.Keys)
        {
            GameObject ShopItem = Instantiate(Resources.Load("Drones/" + Value)) as GameObject;
            ShopItem.transform.position = new Vector3(2 * ItemsInShop.Count, -19.25f, 0);
            ShopItem.transform.Rotate(new Vector3(0, 20, 0));
            ShopItem.tag = "Preview";
            ShopItem.AddComponent<SphereCollider>().radius = 1f;
            ItemsInShop.Add(ShopItem);
        }


    }

    void StopSupport()
    {
        PlayerPrefs.DeleteKey("SupportingFaction");
    }
    //sets camera back to default position and rotation
    public void ResetCam()
    {

        if (Inshop == true)
        {
            this.gameObject.transform.position = SelectedPos.transform.position;
            this.gameObject.transform.rotation = SelectedPos.transform.rotation;
            CamPos = SelectedPos.transform.position;
            CamRot = SelectedPos.transform.rotation;
            DroneMenu.SetActive(false);
            FactionMenu.SetActive(true);
            Inshop = false;
        }
        else if (zoomed == true && Inshop == false)
        {
            //clear positions
            CamPos = DefaultPos;
            CamRot = DefaultRot;
            SelectedPos = null;
            
            //clear shop
            foreach (GameObject Item in ItemsInShop)
            {
                Destroy(Item);
            }
            ItemsInShop.Clear();
            ShopItems.Clear();

            //clear zoom variables
            FactionMenu.SetActive(false);
            SelectedFaction = null;
            zoomed = false;
        }
        else if (zoomed == false && Inshop == false)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

}
